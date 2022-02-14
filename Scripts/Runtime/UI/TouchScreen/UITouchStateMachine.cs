using System;
using UnityEngine;
using UnityEngine.UI;

namespace MacKay.UI
{
    public class UITouchStateMachine : MonoBehaviour
    {
        [Header("Debug")] public bool _enableDebug;
        
        [Header("State General")] 
        [SerializeField] private UITouchScreenBaseState _currentState;

        [SerializeField] private InterfaceAnimManager _tutorialManager;
        public InterfaceAnimManager TutorialManager => _tutorialManager;
        [SerializeField] private InterfaceAnimManager _warpManager;
        public InterfaceAnimManager WarpManager => _warpManager;
        [SerializeField] private InterfaceAnimManager _planetManager;
        public InterfaceAnimManager PlanetManager => _planetManager;
        [SerializeField] private Text _planetInformationText;
        public Text PlanetInformation => _planetInformationText;
        [SerializeField] private MeshRenderer _planetMeshRenderer;
        public MeshRenderer PlanetMeshRenderer => _planetMeshRenderer;
        [SerializeField] private Material[] _planetMaterials;
        public Material[] PlanetMaterials => _planetMaterials;
        [SerializeField] private InterfaceAnimManager _orbitManager;
        public InterfaceAnimManager OrbitManager => _orbitManager;
        [SerializeField] private MeshRenderer _orbitMeshRenderer;
        public MeshRenderer OrbitMeshRenderer => _orbitMeshRenderer;
        [SerializeField] private Text _orbitPlanetText;
        public Text OrbitPlanetText => _orbitPlanetText;
        [SerializeField] private GameObject _orbitButtonLeave;
        public GameObject OrbitButtonLeave => _orbitButtonLeave;
        [SerializeField] private GameObject _orbitButtonOrbit;
        public GameObject OrbitButtonOrbit => _orbitButtonOrbit;
        [SerializeField] private Image _orbitProgressImage;
        public Image OrbitProgressImage => _orbitProgressImage;
        [SerializeField] private int _orbitingPlanetNumber;
        public int OrbitingPlanetNumber
        {
            get => _orbitingPlanetNumber;
            set => _orbitingPlanetNumber = value;
        }

        public UITouchScreenBaseState CurrentState
        {
            get => _currentState;
            set => _currentState = value;
        }
        private UITouchScreenStateFactory _states;

        private void Awake()
        {
            _states = new UITouchScreenStateFactory(this);

            _currentState = _states.Orbit(GameManager.Instance.Planets[2], 2);
            _currentState.EnterState();
        }

        private void Start()
        {
            UITouchButton_ActionHandler.Instance.OnSetOrbitingPlanetNumber += HandleSetOrbitingPlanetNumber;
        }

        private void OnDisable()
        {
            UITouchButton_ActionHandler.Instance.OnSetOrbitingPlanetNumber -= HandleSetOrbitingPlanetNumber;
        }

        protected void Update()
        {
            _currentState.UpdateStates();
        }

        private void HandleSetOrbitingPlanetNumber(int num)
        {
            OrbitingPlanetNumber = num;
        }
    }
}