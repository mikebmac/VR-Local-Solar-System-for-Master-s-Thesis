using System;
using MacKay.UI;
using SpaceGraphicsToolkit;
using UnityEngine;

namespace MacKay.PlayerController.Ship
{
    public class ShipActionHandler : MonoBehaviour
    {
        #region Singleton
        private static ShipActionHandler _instance;
        public static ShipActionHandler Instance => _instance;
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }
        #endregion
        
        public event Action<SgtFloatingTarget> OnWarpRequest;
        public event Action<float> OnShipDriveLowPriorityTick;
        public event Action<UIController.SpeedUnits> OnShipEngineAudioTick;
        public event Action<double> OnAutoPilotUIUpdate;
        public event Action<string, double> OnCurrentPlanetUpdate;

        [SerializeField] private MonoBehaviour[] _listeners;
        
        private void Start()
        {
            // Work around for load order without dealing with script execution order
            foreach (MonoBehaviour listener in _listeners)
            {
                if (!listener.enabled) listener.enabled = true;
            }
        }

        public void WarpTo(SgtFloatingTarget target)
        {
            OnWarpRequest?.Invoke(target);
        }

        public void InvokeShipDriveLowPriorityTick(float deltaTime)
        {
            OnShipDriveLowPriorityTick?.Invoke(deltaTime);
        }
        
        public void InvokeShipEngineAudioTick(UIController.SpeedUnits engineThrottle)
        {
            OnShipEngineAudioTick?.Invoke(engineThrottle);
        }

        public void InvokeAutoPilotUIUpdate(double angle)
        {
            OnAutoPilotUIUpdate?.Invoke(angle);
        }

        public void InvokeCurrentPlanetUpdate(string planet, double distance)
        {
            OnCurrentPlanetUpdate?.Invoke(planet, distance);
        }
    }
}