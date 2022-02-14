using System.Collections;
using MacKay.Audio;
using MacKay.UI;
using MacKay.XR.Animation;
using SpaceGraphicsToolkit;
using UnityEngine;
using AudioType = MacKay.Audio.AudioType;

namespace MacKay.PlayerController.Ship
{
    public class ShipAutopilotState: ShipBaseState
    {
        private SgtFloatingPoint _orbitPoint;
        private Vector3 _startingPos;
        private float _startingRotation;
        private float _orbitDistanceMultiplier = 1.03f;
        private double _orbitTargetDistance;
        private SgtFloatingOrbit _orbitLead;
        private IEnumerator _rotateForwardsOrbitPoint;
        private IEnumerator _orbitRadiusEase;
        private IEnumerator _updateUI;

        public ShipAutopilotState(ShipStateMachine currentContext, ShipStateFactory shipStateFactory, 
        SgtFloatingPoint orbitPoint) : base
            (currentContext, shipStateFactory)
        {
            IsRootState = true;
            _orbitPoint = orbitPoint;
            _orbitTargetDistance = _orbitPoint.transform.localScale.x * _orbitDistanceMultiplier;
            InitializeSubState();
        }

        public override void EnterState()
        {
            UITouchButton_ActionHandler.Instance.OnSplashOrbitLeaveOrbit += HandleLeaveOrbitRequest;
            ShipActionHandler.Instance.OnWarpRequest += HandleWarpRequest;

            _rotateForwardsOrbitPoint = Cor_RotateForwardsOrbit();
            _orbitRadiusEase = Cor_OrbitFixedRadius();
            _updateUI = Cor_UpdateUI();

            StartOrbit();
            
            // TODO remove once better distance created
            ShipActionHandler.Instance.InvokeCurrentPlanetUpdate(_orbitPoint.name, _orbitTargetDistance - 
                _orbitPoint.transform.localScale.x);

            AudioController.Instance.PlayAudio(AudioType.VO_ENTERING_ORBIT);
            AudioEngineController.Instance.SetEngineAudio(AudioType.ENGINE_1X_LOOP);
            ThrusterController.Instance.SetThrusters(0.4f);
        }

        public override void UpdateState()
        {
            CheckSwitchStates();
        }

        public override void ExitState()
        {
            UITouchButton_ActionHandler.Instance.OnSplashOrbitLeaveOrbit -= HandleLeaveOrbitRequest;
            ShipActionHandler.Instance.OnWarpRequest -= HandleWarpRequest;
            
            if (_rotateForwardsOrbitPoint != null) Ctx.StopCoroutine(_rotateForwardsOrbitPoint);
            if (_orbitRadiusEase != null) Ctx.StopCoroutine(_orbitRadiusEase);
            if (_updateUI != null) Ctx.StopCoroutine(_updateUI);
            
            if (_orbitLead != null) Ctx.DestroyGameObject(_orbitLead.gameObject);

            Ctx.AutoOrbit.enabled = false;

            AudioEngineController.Instance.StopEngineAudio();
            ThrusterController.Instance.SetThrusters(0);
        }

        public override void CheckSwitchStates()
        {

        }

        public override void InitializeSubState()
        {

        }
        
        private void HandleLeaveOrbitRequest()
        {
            SwitchState(Factory.Drive());
        }
        
        private void HandleWarpRequest(SgtFloatingTarget target)
        {
            double distance = SgtPosition.Distance(Ctx.PlayerPosition.Position, target.CachedPoint.Position);
            
            if (distance < Ctx.MinWarpDistance) return;
            
            SwitchState(Factory.Warp(target, distance));
        }

        private void CreateOrbitLead(double distance, float angle)
        {
            if (_orbitLead != null) return;
            
            GameObject go = new GameObject("Autopilot Orbit Lead");
            go.AddComponent<SgtFloatingObject>();
            _orbitLead = go.AddComponent<SgtFloatingOrbit>();
            _orbitLead.ParentPoint = _orbitPoint;
            _orbitLead.Radius = distance;
            _orbitLead.Angle = angle + 10f;
            _orbitLead.DegreesPerSecond = 1d;
            _orbitLead.enabled = true;
        }

        private void StartOrbit()
        {
            Vector3 targetDirection = Ctx.transform.position - _orbitPoint.transform.position;
            float angle = Vector3.Angle(targetDirection, _orbitPoint.transform.forward);
            float isRightOfPlanet = Vector3.Dot(targetDirection, _orbitPoint.transform.right);
            if (isRightOfPlanet < 0)
            {
                angle *= -1f;
            }

            double distance = SgtPosition.Distance(_orbitPoint.Position, Ctx.PlayerPosition.Position);
            _startingRotation = angle;
            
            CreateOrbitLead(distance, angle);
            
            Ctx.AutoOrbit.ParentPoint = _orbitPoint;
            Ctx.AutoOrbit.Radius = distance;
            Ctx.AutoOrbit.Angle = angle;
            Ctx.AutoOrbit.DegreesPerSecond = 1d;
            Ctx.AutoOrbit.enabled = true;

            Ctx.StartCoroutine(_updateUI);
            Ctx.StartCoroutine(_rotateForwardsOrbitPoint);
            Ctx.StartCoroutine(_orbitRadiusEase);
        }

        private IEnumerator Cor_RotateForwardsOrbit()
        {
            while (Ctx.CurrentState == this)
            {
                yield return new WaitForEndOfFrame();
                Vector3 direction = _orbitLead.transform.position - Ctx.transform.position;
                Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

                Ctx.transform.localRotation = Quaternion.Lerp(Ctx.transform.localRotation, rotation, Time.deltaTime *
                 1.5f);
            }
        }
        
        private IEnumerator Cor_OrbitFixedRadius()
        {
            float timeElapsed = 0f;
            double startingDistance = Ctx.AutoOrbit.Radius;
            float duration = (float)startingDistance / 1000000f;
            float leadDuration = duration / 2f;
            
            while (timeElapsed < duration)
            {
                Ctx.AutoOrbit.Radius = Mathf.Lerp((float)startingDistance, (float)_orbitTargetDistance, timeElapsed / 
                duration);
                
                if (timeElapsed < leadDuration) _orbitLead.Radius = Mathf.Lerp((float) startingDistance, (float) _orbitTargetDistance, timeElapsed /
                    leadDuration);
                
                timeElapsed += Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator Cor_UpdateUI()
        {
            while (Ctx.CurrentState == this)
            {
                ShipActionHandler.Instance.InvokeAutoPilotUIUpdate(Ctx.AutoOrbit.Angle - _startingRotation);
                // TODO More accurate Distance to planet here
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}