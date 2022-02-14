using System.Collections;
using System.Collections.Generic;
using MacKay.Data;
using MacKay.UI;
using SpaceGraphicsToolkit;
using UnityEngine;

namespace MacKay.PlayerController.Ship
{
    public class ShipDriveState : ShipBaseState
    {
        private Vector2 _targetThrottle;
        private Vector2 _targetRotationAxis;
        private Vector2 _currentRotationAxis;
        private float _previousTime = -1f;
        private SgtFloatingObject _currentPlanet;
        private double _currentPlanetDistance;
        private List<SgtFloatingObject> _currentCelestialObjects = new List<SgtFloatingObject>();
        private IEnumerator _lowPriorityTick;
        private IEnumerator _engineAudioTick;
        private IEnumerator _spawnerInstancesWait;

        public ShipDriveState(ShipStateMachine currentContext, ShipStateFactory shipStateFactory) : base
            (currentContext, shipStateFactory)
        {
            IsRootState = true;
            InitializeSubState();
        }
        
        public override void EnterState()
        {
            ShipActionHandler.Instance.OnWarpRequest += HandleWarpRequest;
            Ctx.PlayerPosition.OnPositionChanged += HandlePositionChange;
            UITouchButton_ActionHandler.Instance.OnSplashOrbitEnterOrbit += HandleOrbitRequest;

            _spawnerInstancesWait = Cor_GetNearbyCelestialBodies();
            Ctx.StartCoroutine(_spawnerInstancesWait);

            _lowPriorityTick = Cor_ShipDriveLowPriorityTick();
            _engineAudioTick = Cor_ShipEngineAudioTick();
            
            Ctx.StartCoroutine(_lowPriorityTick);
            Ctx.StartCoroutine(_engineAudioTick);
        }

        public override void UpdateState()
        {
            CheckSwitchStates();

            if (_targetThrottle != Ctx.ThrottleJoystick.JoystickAngle)
            {
                float x = 0;
                if (Ctx.EnableStrafing)
                {
                    x = Ctx.ThrottleCurve.Evaluate(Mathf.Abs(Ctx.ThrottleJoystick.JoystickAngle.x));
                    if (Ctx.ThrottleJoystick.JoystickAngle.x < 0) x *= -1;
                }
                
                float y = Ctx.ThrottleCurve.Evaluate(Mathf.Abs(Ctx.ThrottleJoystick.JoystickAngle.y));
                if (Ctx.ThrottleJoystick.JoystickAngle.y < 0) y *= -1;
                
                UIController.Instance.SpeedometerController.SetThrottleDisplay(Ctx.ThrottleJoystick.JoystickAngle.y);
                
                _targetThrottle = new Vector2(x, y);
            }

            if (_targetRotationAxis != Ctx.RotationJoystick.JoystickAngle)
            {
                float x = Ctx.RotationCurve.Evaluate(Mathf.Abs(Ctx.RotationJoystick.JoystickAngle.x));
                float y = Ctx.RotationCurve.Evaluate(Mathf.Abs(Ctx.RotationJoystick.JoystickAngle.y));
                if (Ctx.RotationJoystick.JoystickAngle.x < 0) x *= -1;
                if (Ctx.RotationJoystick.JoystickAngle.y < 0) y *= -1;
                
                _targetRotationAxis = new Vector2(x, y);
            }

            if (Ctx.CurrentThrottle != _targetThrottle)
            {
                Ctx.CurrentThrottle = Vector2.MoveTowards(Ctx.CurrentThrottle, _targetThrottle, Time.deltaTime * 
                Ctx.ThrottleResponse);
            }

            if (_currentRotationAxis != _targetRotationAxis)
            {
                _currentRotationAxis = _targetRotationAxis;
                
                float pitch = Ctx.RotationJoystick.JoystickAngle.y * Ctx.RotationMultiplier;
                float yaw = Ctx.RotationJoystick.JoystickAngle.x * Ctx.RotationMultiplier;

                Quaternion localRotation = Ctx.transform.localRotation;
                Quaternion rotation = localRotation * Quaternion.Euler(pitch, yaw, 0f);
                Ctx.transform.localRotation = Quaternion.Slerp(localRotation, rotation, Time.deltaTime * 
                Ctx.RotationDamping);
            }
        }

        public override void ExitState()
        {
            ShipActionHandler.Instance.OnWarpRequest -= HandleWarpRequest;
            Ctx.PlayerPosition.OnPositionChanged -= HandlePositionChange;
            UITouchButton_ActionHandler.Instance.OnSplashOrbitEnterOrbit -= HandleOrbitRequest;
            
            if(_lowPriorityTick != null) Ctx.StopCoroutine(_lowPriorityTick);
            if(_engineAudioTick != null) Ctx.StopCoroutine(_engineAudioTick);
            if(_spawnerInstancesWait != null) Ctx.StopCoroutine(_spawnerInstancesWait);
        }

        public override void CheckSwitchStates()
        {
            // autopilot condition
        }

        public override void InitializeSubState()
        {
            // silence
        }

        #region State Methods
        private void HandleOrbitRequest()
        {
            if (_currentPlanet != null) SwitchState(Factory.Autopilot(_currentPlanet));
        }
        
        private void HandlePositionChange()
        {
            foreach (SgtFloatingObject celestialObject in _currentCelestialObjects)
            {
                float distance = (float)SgtPosition.Distance(Ctx.PlayerPosition.Position, celestialObject.Position);
                float scale = celestialObject.transform.localScale.x;
                float ratio = (distance - scale * 1.05f) / scale;

                if (_currentPlanet == celestialObject) _currentPlanetDistance = distance;

                if (ratio >= 1f) continue;
                
                bool hit = Physics.Raycast(Ctx.transform.position, Ctx.transform.forward, distance * 2f, Ctx.PlanetLayer);

                float damping = 1f;
                if (hit) damping = Ctx.SpeedDampingCurve.Evaluate(ratio);

                Ctx.SpeedMin = Ctx.BaseSpeedMin * damping;
                Ctx.SpeedMax = Ctx.BaseSpeedMax * damping;
            }
        }
        
        private void HandleWarpRequest(SgtFloatingTarget target)
        {
            double distance = SgtPosition.Distance(Ctx.PlayerPosition.Position, target.CachedPoint.Position);
            
            if (distance < Ctx.MinWarpDistance) return;
            
            SwitchState(Factory.Warp(target, distance));
        }
        
        private IEnumerator Cor_GetNearbyCelestialBodies()
        {
            double closetPlanetDistance = 1500000000000000d;

            for (int i = 0; i < GameManager.Instance.Planets.Length; i++)
            {
                SgtFloatingObject floatingObj = GameManager.Instance.Planets[i].celestailObject.GetComponent<SgtFloatingObject>();
                double distance = SgtPosition.Distance(Ctx.PlayerPosition.Position, floatingObj.Position);
                if (distance < closetPlanetDistance)
                {
                    closetPlanetDistance = distance;
                    _currentPlanet = floatingObj;
                    UITouchButton_ActionHandler.Instance.UISetOrbitingPlanetNumber(i);
                }
                
            }

            _currentCelestialObjects.Add(_currentPlanet);
            CelestailSpawner spawner = _currentPlanet.GetComponent<CelestailSpawner>();

            if (spawner != null)
            {
                if (spawner.Satellites.Count > 0) yield return new WaitUntil(() => spawner.Instances.Count > 0);
                
                foreach (SgtFloatingObject satellite in spawner.Instances)
                {
                    _currentCelestialObjects.Add(satellite);
                }
            }
        }

        private IEnumerator Cor_ShipDriveLowPriorityTick()
        {
            while (Ctx.CurrentState == this)
            {
                float deltaTime = 0f;
                if (_previousTime > 0f)
                {
                    deltaTime = Time.time - _previousTime;
                }
                ShipActionHandler.Instance.InvokeShipDriveLowPriorityTick(deltaTime);
                _previousTime = Time.time;
                
                ShipActionHandler.Instance.InvokeCurrentPlanetUpdate(_currentPlanet.name, _currentPlanetDistance);

                yield return new WaitForSeconds(0.1f);
            }
        }
        
        private IEnumerator Cor_ShipEngineAudioTick()
        {
            yield return new WaitUntil(() => ShipActionHandler.Instance != null);
            
            while (Ctx.CurrentState == this)
            {
                ShipActionHandler.Instance.InvokeShipEngineAudioTick(UIController.Instance.CurrentSpeed);

                yield return new WaitForSeconds(0.15f);
            }
        }
        #endregion
    }
}