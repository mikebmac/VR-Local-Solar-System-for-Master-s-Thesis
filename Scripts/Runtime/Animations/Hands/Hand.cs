using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MacKay.XR.Animation
{
    [RequireComponent(typeof(Animator))]
    public class Hand : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private string animatorGripParam = "Grip";
        [SerializeField] private string animatorTriggerParam = "Trigger";
        [SerializeField] private string animatorJoystickGripParam = "JoystickGrip";
        [SerializeField] private string animatorJoystickGrabParam = "isGrabbingJoystick";
        [SerializeField] private string animatorJoystickMovingParam = "isMovingJoystick";
        private Animator animator;
        private float gripTarget;
        private float gripCurrent;
        private float triggerTarget;
        private float triggerCurrent;
        private float joystickGripTarget;
        private float joystickGripCurrent;
        private bool grabJoystick = false;
        private bool moveJoystick = false;

        [SerializeField] private Transform baseParent;
        private Transform currentParent;
        private Vector3 basePosition;
        private Quaternion baseRotation;

        #region Private Methods
        private void Awake()
        {
            animator = GetComponent<Animator>();

            currentParent = baseParent;
            basePosition = transform.localPosition;
            baseRotation = transform.localRotation;
        }

        private void Update()
        {
            AnimateHand();
        }

        internal void SetGrip(float v)
        {
            gripTarget = v;
        }

        internal void SetTrigger(float v)
        {
            triggerTarget = v;
        }

        internal void SetJoystickGrip(float v)
        {
            joystickGripTarget = v;
        }

        internal void SetMovingJoystick(bool v)
        {
            moveJoystick = v;
        }

        internal void SetParent(Transform parent, Vector3 attachPosition, Quaternion attachRotation)
        {
            currentParent = parent;
            transform.parent = currentParent;
            transform.localPosition = attachPosition;
            transform.localRotation = attachRotation;

            grabJoystick = true;
        }

        internal void ReturnParent()
        {
            currentParent = baseParent;
            transform.parent = currentParent;
            transform.localPosition = basePosition;
            transform.localRotation = baseRotation;

            grabJoystick = false;
        }

        private void AnimateHand ()
        {
            animator.SetBool(animatorJoystickGrabParam, grabJoystick);
            animator.SetBool(animatorJoystickMovingParam, moveJoystick);

            if (joystickGripCurrent != joystickGripTarget)
            {
                joystickGripCurrent = Mathf.MoveTowards(joystickGripCurrent, joystickGripTarget, Time.deltaTime * speed);
                animator.SetFloat(animatorJoystickGripParam, joystickGripCurrent);
            }

            if (gripCurrent != gripTarget)
            {
                gripCurrent = Mathf.MoveTowards(gripCurrent, gripTarget, Time.deltaTime * speed);
                animator.SetFloat(animatorGripParam, gripCurrent);
            }

            if (triggerCurrent != triggerTarget)
            {
                triggerCurrent = Mathf.MoveTowards(triggerCurrent, triggerTarget, Time.deltaTime * speed);
                animator.SetFloat(animatorTriggerParam, triggerCurrent);
            }
        }
        #endregion

    }
}
