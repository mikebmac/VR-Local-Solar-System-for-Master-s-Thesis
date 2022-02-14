using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MacKay.PlayerController
{
    public class PlayerXRInteractable_Joystick : PlayerXRInteractable
    {
        public enum State
        {
            NONE,
            VIRTUAL,
            PHYSICAL
        }

        [Header("Debug")]
        public bool enableDebug;
        public override InteractableType Type {
            get { return InteractableType.JOYSTICK; } 
        }

        [SerializeField]
        private Transform grabPoint;
        public override Transform GrabPoint
        {
            get
            {
                return grabPoint;
            }
        }

        [SerializeField]
        private Vector3 grabOffset;
        public override Vector3 GrabOffsetPosition
        {
            get
            {
                return grabOffset;
            }
        }

        [SerializeField]
        private Quaternion grabOffsetRotation;
        public override Quaternion GrabOffsetRotation
        {
            get
            {
                return grabOffsetRotation;
            }
        }

        [Header("Joystick")]
        private bool firstCall = true;
        [SerializeField]
        private Transform joystickBase;
        private Quaternion joystickStartRotation;
        [SerializeField]
        private Vector3 controllerStartPosition;
        [SerializeField]
        private bool enableForwardBack = true;
        [SerializeField]
        private bool enableLeftRight = true;
        [SerializeField]
        private float joystickMaxVisibleAngle = 50f;
        [SerializeField]
        private Vector2 joystickAngle = new Vector2(); 
        public Vector2 JoystickAngle
        {
            get
            {
                return joystickAngle;
            }
        }
        [SerializeField]
        private float snapbackSpeed = 10f;

        [Header("Interacting Controller")]
        [SerializeField]
        private Transform controller;

        [SerializeField]
        private bool inUse = false;
        public override bool InUse
        {
            get
            {
                return inUse;
            }
            set
            {
                inUse = value;
            }
        }

        private State currentState = State.NONE;
        public State CurrentState
        {
            get { 
                return currentState; 
            }
            set
            {
                currentState = value;
            }
        }

        #region Unity Methods
        private void Awake()
        {
            joystickStartRotation = joystickBase.localRotation;
        }

        private void LateUpdate()
        {
            if (currentState == State.VIRTUAL)
            {
                if (firstCall) Initialize();

                if (enableForwardBack)
                {
                    float distanceY = controller.localPosition.z - controllerStartPosition.z;

                    if (distanceY >= 0)
                    {
                        joystickAngle.y = Mathf.InverseLerp(0f, 0.125f, distanceY);
                    } else
                    {
                        joystickAngle.y = -Mathf.InverseLerp(0f, -0.125f, distanceY);
                    }
                } 

                if (enableLeftRight)
                {
                    float distanceY = controller.localPosition.x - controllerStartPosition.x;

                    if (distanceY >= 0)
                    {
                        joystickAngle.x = Mathf.InverseLerp(0f, 0.125f, distanceY);
                    }
                    else
                    {
                        joystickAngle.x = -Mathf.InverseLerp(0f, -0.125f, distanceY);
                    }
                }
                SetJoystickAngles(joystickAngle);

            } 
            else if (currentState == State.PHYSICAL)
            {
                SetJoystickAngles(joystickAngle);
            }
            else if (currentState == State.NONE && joystickAngle != Vector2.zero)
            {
                firstCall = true;
                joystickBase.localRotation = Quaternion.Slerp(joystickBase.rotation, joystickStartRotation, Time.deltaTime * snapbackSpeed);
                joystickAngle = Vector2.Lerp(joystickAngle, Vector2.zero, Time.deltaTime * snapbackSpeed);
            }
        }
        #endregion

        #region Public Methods
        public void SetInteractingController(Transform controller)
        {
            this.controller = controller;
        }

        public void SetJoystickAxis(Vector2 joystickVectors)
        {
            if (currentState != State.PHYSICAL) currentState = State.PHYSICAL;
            joystickAngle = joystickVectors;
        }

        public void SetJoystickState(State state)
        {
            currentState = state;
        }
        #endregion

        #region Private Methods
        private void Initialize()
        {
            firstCall = false;
            controllerStartPosition = controller.localPosition;
        }

        private void SetJoystickAngles(Vector2 angles)
        {
            joystickBase.localEulerAngles = new Vector3(angles.x * joystickMaxVisibleAngle, 0f, angles.y * joystickMaxVisibleAngle);
        }

        private void Log(string _msg)
        {
            if (!enableDebug) return;

            Debug.Log("[Interactable Joystick]: " + _msg, this);
        }

        private void LogWarning(string _msg)
        {
            if (!enableDebug) return;

            Debug.LogWarning("[Interactable Joystick]: " + _msg, this);
        }
        #endregion
    }
}
