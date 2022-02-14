using UnityEngine;
using SpaceGraphicsToolkit;
using MacKay.PlayerController.Ship;

namespace MacKay.XR.Animation
{
    public class ThrusterController : MonoBehaviour
    {
        #region Singleton
        private static ThrusterController _instance;
        public static ThrusterController Instance => _instance;
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
        
        #region Fields
        [SerializeField] private ShipStateMachine _shipController;
        [SerializeField] private SgtThruster[] _thrusters;
        [SerializeField] private float _animationSmoothing = 10f;
        private float thrusterCurrent;
        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            ShipActionHandler.Instance.OnShipDriveLowPriorityTick += HandleThrusters;
        }

        private void OnDisable()
        {
            ShipActionHandler.Instance.OnShipDriveLowPriorityTick -= HandleThrusters;
        }

        #endregion
        
        #region Public Methods

        public void SetThrusters(float targetValue)
        {
            foreach (var thruster in _thrusters)
            {
                thruster.Throttle = targetValue;
            }
        }
        #endregion

        #region Private Methods
        private void HandleThrusters(float deltaTime)
        {
            thrusterCurrent = Mathf.MoveTowards(thrusterCurrent, _shipController.CurrentThrottle.y, deltaTime * 
            _animationSmoothing);
            foreach (var thruster in _thrusters)
            {
                thruster.Throttle = thrusterCurrent;
            }
        }
        #endregion
    }
}