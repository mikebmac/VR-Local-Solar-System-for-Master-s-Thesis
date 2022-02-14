using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using MacKay.XR.Animation;
using System;

namespace MacKay.PlayerController
{
    [RequireComponent(typeof(ActionBasedController), typeof(HandController))]
    public class PlayerXRController : MonoBehaviour
    {

        [Header("Debug")]
        public bool enableDebug;

        [Header("Player State")]
        public ControllerState currentState = ControllerState.FREE;

        [Header("Physical Controller")]
        [SerializeField]
        InputActionReference xrControllerMoveAction;
        [SerializeField]
        private PlayerXRInteractable_Joystick joystickToControl;
        [SerializeField]
        private bool inverseX = false;
        [SerializeField]
        private bool inverseY = false;

        [Header("Virtual Controller")]
        [SerializeField]
        private ActionBasedController xrController;
        public PlayerXRInteractable currentInteractable;
        private PlayerXRInteractable_Joystick currentJoystickInteractable;
        [SerializeField]
        private Collider interactionCollider;
        private bool inTrigger = false;

        [Header("Animations")]
        [SerializeField]
        private HandController handController;

        #region Unity Methods
        private void Awake()
        {
            if (!xrController)
            {
                LogWarning("XR [Action-based Controller] not found.");
            }

            if (!interactionCollider)
            {
                LogWarning("[Collider] not found.");
            } else if(!interactionCollider.isTrigger)
            {
                LogWarning("Trigger [Collider] was not a trigger, setting to trigger.");
                interactionCollider.isTrigger = true;
            }

            if (!handController)
            {
                LogWarning("XR Animation [Hand Controller] not found.");
            }
        }

        private void OnEnable()
        {
            xrControllerMoveAction.action.performed += OnPhysicalJoystickPerform;
            xrControllerMoveAction.action.canceled += OnPhysicalJoystickCancel;
        }

        private void OnDisable()
        {
            xrControllerMoveAction.action.performed -= OnPhysicalJoystickPerform;
            xrControllerMoveAction.action.canceled -= OnPhysicalJoystickCancel;
        }

        private void Update()
        {

            float selectValue = xrController.selectAction.action.ReadValue<float>();
            float activateValue = xrController.activateAction.action.ReadValue<float>();

            switch (currentState)
            {
                case ControllerState.NONE:
                    break;
                case ControllerState.FREE:

                    handController.SetMoveJoystick(false);
                    handController.SetGrip(selectValue);
                    handController.SetTrigger(activateValue);

                    break;
                case ControllerState.INTERACTABLE:

                    break;
                case ControllerState.VIRTUALJOYSTICK:

                    handController.SetJoystickGrip(selectValue);
                    if (selectValue > 0.1f)
                    {
                        currentJoystickInteractable.SetJoystickState(PlayerXRInteractable_Joystick.State.VIRTUAL);
                        handController.SetMoveJoystick(true);
                    }
                    else
                    {
                        currentJoystickInteractable.SetJoystickState(PlayerXRInteractable_Joystick.State.NONE);
                        handController.SetMoveJoystick(false);
                    }

                    break;
                case ControllerState.PHYSICALJOYSTICK:

                    break;
                default:
                    break;
            }
            
        }

        private void OnTriggerEnter(Collider other)
        {
            if (currentState != ControllerState.FREE) return;

            if (other.tag == "Interactable")
            {
                currentInteractable = other.GetComponent<PlayerXRInteractable>();
                inTrigger = true;

                if (currentInteractable.Type == PlayerXRInteractable.InteractableType.BASIC)
                {
                    currentState = ControllerState.INTERACTABLE;
                }
                else if (currentInteractable.Type == PlayerXRInteractable.InteractableType.JOYSTICK)
                {
                    currentState = ControllerState.VIRTUALJOYSTICK;
                    currentJoystickInteractable = other.GetComponent<PlayerXRInteractable_Joystick>();

                    currentJoystickInteractable.SetInteractingController(transform);

                    handController.SetParent(currentInteractable.GrabPoint, currentInteractable.GrabOffsetPosition, currentInteractable.GrabOffsetRotation);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Interactable")
            {
                if (currentState == ControllerState.VIRTUALJOYSTICK)
                {
                    StartCoroutine(CorReleaseGrib());
                    inTrigger = false;
                } else
                {
                    InteractableDisconnect();
                }
            }
        }
        #endregion

        #region Private Methods
        private IEnumerator CorReleaseGrib()
        {
            yield return new WaitUntil(() => xrController.selectAction.action.ReadValue<float>() < 0.1f);

            if (!inTrigger)
            {
                handController.ReturnParent();
                InteractableDisconnect();
            }
            
        }

        private void OnPhysicalJoystickPerform(InputAction.CallbackContext context)
        {
            if (currentState == ControllerState.FREE)
            {
                currentState = ControllerState.PHYSICALJOYSTICK;
                joystickToControl.SetJoystickState(PlayerXRInteractable_Joystick.State.PHYSICAL);
            }
            else if (currentState != ControllerState.PHYSICALJOYSTICK)
            {
                return;
            }

            Vector2 direction = context.ReadValue<Vector2>();

            if (inverseX) direction.x *= -1;
            if (inverseY) direction.y *= -1;

            joystickToControl.SetJoystickAxis(direction);
        }

        private void OnPhysicalJoystickCancel(InputAction.CallbackContext context)
        {
            if (currentState == ControllerState.PHYSICALJOYSTICK)
            {
                currentState = ControllerState.FREE;
                joystickToControl.SetJoystickState(PlayerXRInteractable_Joystick.State.NONE);
            }

        }
        private void InteractableDisconnect()
        {
            if (currentInteractable)
            {
                currentInteractable = null;
            }
            currentState = ControllerState.FREE;
        }
        private void Log(string _msg)
        {
            if (!enableDebug) return;

            Debug.Log("[XR Controller]: " + _msg, this);
        }

        private void LogWarning(string _msg)
        {
            if (!enableDebug) return;

            Debug.LogWarning("[XR Controller]: " + _msg, this);
        }
        #endregion


    }
}