using System;
using MacKay.Data;
using UnityEngine;

namespace MacKay.UI
{
    public class UITouchButton_ActionHandler : MonoBehaviour
    {
        #region Singleton
        private static UITouchButton_ActionHandler _instance;
        public static UITouchButton_ActionHandler Instance => _instance;

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

        public event Action<CelestailObjectData, int> OnSplashWarpToSplashPlanet;
        public event Action OnSplashWarpRequest;
        public event Action OnSplashWarpToSplashOrbit;
        public event Action OnSplashPlanetBack;
        
        public event Action OnSplashOrbitToSplashWarp;
        public event Action OnSplashOrbitEnterOrbit;
        public event Action OnSplashOrbitLeaveOrbit;

        public event Action<int> OnSetOrbitingPlanetNumber;
        
        [SerializeField] private MonoBehaviour[] _listeners;
        
        private void Start()
        {
            // Work around for load order without dealing with script execution order
            foreach (MonoBehaviour listener in _listeners)
            {
                if (!listener.enabled) listener.enabled = true;
            }
        }
        
        public void UIWarpSwitchToPlanet(CelestailObjectData target, int planetNumber)
        {
            OnSplashWarpToSplashPlanet?.Invoke(target, planetNumber);
        }

        public void UISplashWarpRequest()
        {
            OnSplashWarpRequest?.Invoke();
        }
        
        public void UISplashWarpToSplashOrbit()
        {
            OnSplashWarpToSplashOrbit?.Invoke();
        }

        public void UISplashPlanetBack()
        {
            OnSplashPlanetBack?.Invoke();
        }
        
        public void UISplashOrbitToSplashWarp()
        {
            OnSplashOrbitToSplashWarp?.Invoke();
        }
        
        public void UISplashOrbitEnterOrbit()
        {
            OnSplashOrbitEnterOrbit?.Invoke();
        }
        
        public void UISplashOrbitLeaveOrbit()
        {
            OnSplashOrbitLeaveOrbit?.Invoke();
        }

        public void UISetOrbitingPlanetNumber(int number)
        {
            OnSetOrbitingPlanetNumber?.Invoke(number);
        }
        
    }
}