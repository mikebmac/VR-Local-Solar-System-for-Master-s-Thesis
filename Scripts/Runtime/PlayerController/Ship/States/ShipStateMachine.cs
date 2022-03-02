using MacKay.Animations;
using SpaceGraphicsToolkit;
using UnityEngine;

namespace MacKay.PlayerController.Ship
{
    public class ShipStateMachine : SgtCameraMove
    {
        [Header("Debug")] 
        public bool _enableDebug;
        
        #region Ship State Members
        [Header("Ship State Base")] 
        [SerializeField] private ShipBaseState _currentState;
        public ShipBaseState CurrentState
        {
            get => _currentState;
            set => _currentState = value;
        }
        
        private ShipStateFactory _states;
        [SerializeField] private SgtFloatingCamera _playerPosition;
        public SgtFloatingCamera PlayerPosition => _playerPosition;
        
        #endregion
        
        
        #region Ship Drive Members
        [Header("Ship Drive State")]
        [SerializeField] private PlayerXRInteractable_Joystick _throttleJoystick;
        public PlayerXRInteractable_Joystick ThrottleJoystick => _throttleJoystick;
        [SerializeField] private bool _enablePitch = false;
        public bool EnablePitch { 
            get => _enablePitch;
            set => _enablePitch = value;
        }
        [SerializeField] private bool _enableYaw = false;
        public bool EnableYaw { 
            get => _enableYaw;
            set => _enableYaw = value;
        }
        [SerializeField] private bool _enableThrottle = false;
        public bool EnableThrottle { 
            get => _enableThrottle;
            set => _enableThrottle = value;
        }
        [SerializeField] private bool _enableStrafing = false;
        public bool EnableStrafing { 
            get => _enableStrafing;
            set => _enableStrafing = value;
        }
        [SerializeField] private float _throttleResponse = 10f;
        public float ThrottleResponse => _throttleResponse;
        [SerializeField] private AnimationCurve _throttleCurve;
        public AnimationCurve ThrottleCurve => _throttleCurve;
        [SerializeField] private Vector2 _currentThrottle;
        public Vector2 CurrentThrottle
        {
            get => _currentThrottle;
            set => _currentThrottle = value;
        }
        [SerializeField] private PlayerXRInteractable_Joystick _rotationJoystick;
        public PlayerXRInteractable_Joystick RotationJoystick => _rotationJoystick;
        [SerializeField] private float _rotationMultiplier = 15f;
        public float RotationMultiplier => _rotationMultiplier;
        [SerializeField] private float _rotationDamping = 2f;
        public float RotationDamping => _rotationDamping;
        [SerializeField] private AnimationCurve _rotationCurve;
        public AnimationCurve RotationCurve => _rotationCurve;
        private float _baseSpeedMin;
        public float BaseSpeedMin => _baseSpeedMin;
        private float _baseSpeedMax;
        public float BaseSpeedMax => _baseSpeedMax;
        [SerializeField] private AnimationCurve _speedDampingCurve;
        public AnimationCurve SpeedDampingCurve => _speedDampingCurve;
        [SerializeField] private LayerMask planetLayer;
        public LayerMask PlanetLayer => planetLayer;
        #endregion
        
        #region Ship Warp Members

        [Header("Ship Warp State")]
        [SerializeField] private double _warpSpeed = 100f;
        /// <summary>
        /// Get and set the warp speed.
        /// Measured in the speed of light.
        /// If value = 1, speed would be 1 x the speed of light.
        /// </summary>
        public double WarpSpeed
        {
            get => _warpSpeed * Constants.SpeedOfLight;
            set => _warpSpeed = value;
        }
        [SerializeField] private double _minWarpDistance = 10000000000d;
        public double MinWarpDistance => _minWarpDistance;
        [SerializeField] private SgtFloatingWarpSmoothstep _warpSgtController;
        public SgtFloatingWarpSmoothstep WarpSgtController
        {
            get
            {
                if (!_warpSgtController)
                {
                    _warpSgtController = GetComponent<SgtFloatingWarpSmoothstep>();
                    if (!_warpSgtController)
                    {
                        _warpSgtController = gameObject.AddComponent<SgtFloatingWarpSmoothstep>();
                    }
                }
                return _warpSgtController;
            }
        }
        [SerializeField] private PlayerAnimationController _playerAnimationController;
        public PlayerAnimationController PlayerAnimations
        {
            get
            {
                if (!_playerAnimationController)
                {
                    _playerAnimationController = GetComponent<PlayerAnimationController>();
                    if (!_playerAnimationController)
                    {
                        _playerAnimationController = gameObject.AddComponent<PlayerAnimationController>();
                    }
                }
                return _playerAnimationController;
            }
        }
        [SerializeField] private Audio.AudioController _audioController;
        public Audio.AudioController AudioController => _audioController;
        [SerializeField] private SgtLens _warpLens;
        public SgtLens WarpLens => _warpLens;
        private Vector3 _warpStartPosition;
        public Vector3 WarpStartPosition => _warpStartPosition;
        [SerializeField] private float _warpBubbleOuterEndSize = 1f;
        public float WarpBubbleOuterEndSize => _warpBubbleOuterEndSize;
        [SerializeField] private float _warpBubbleHoleEndSize = 1.1f;
        public float WarpBubbleHolEndSize => _warpBubbleHoleEndSize;
        [SerializeField] private float _warTransDuration = 3.365f;
        public float WarTransDuration => _warTransDuration;
        #endregion
        
        #region Ship Autopilot Members
        [Header("Autopilot")]
        [SerializeField] private SgtFloatingOrbit _autoOrbit;
        public SgtFloatingOrbit AutoOrbit => _autoOrbit;
        #endregion
        
        #region Unity Methods
        private void Awake()
        {
            _states = new ShipStateFactory(this);

            _baseSpeedMin = SpeedMin;
            _baseSpeedMax = SpeedMax;
            
        }

        private void Start()
        {
            if (_audioController == null) _audioController = Audio.AudioController.Instance;
            if (_playerPosition == null) _playerPosition = GetComponent<SgtFloatingCamera>();
            if (_autoOrbit == null) _autoOrbit = GetComponent<SgtFloatingOrbit>();
            
            if (_warpLens == null)
            {
                _warpLens = FindObjectOfType<SgtLens>();
                _warpLens.gameObject.SetActive(false);
            }

            _warpStartPosition = _warpLens.transform.localPosition;
            
            _currentState = _states.Drive();
            _currentState.EnterState();
        }

        protected override void Update()
        {
            base.Update();
            _currentState.UpdateStates();
        }
        #endregion
        
        #region Public Methods
        public void DestroyGameObject(GameObject go)
        {
            Destroy(go);
        }
        #endregion
        
        #region Private Methods
        protected override Vector3 GetDelta(float deltaTime)
        {
            var delta = default(Vector3);

            delta.x = _currentThrottle.x * deltaTime * DepthControls.KeySensitivity;
            delta.z = _currentThrottle.y * deltaTime * DepthControls.KeySensitivity;

            return delta;
        }
        #endregion
    }
}