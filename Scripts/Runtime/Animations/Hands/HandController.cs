using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using MacKay.PlayerController;

namespace MacKay.XR.Animation
{
    public class HandController : MonoBehaviour
    {

        [SerializeField] private Hand hand;

        #region Public Methods
        public void SetParent(Transform parent, Vector3 attachPosition, Quaternion attachRotation)
        {
            hand.SetParent(parent, attachPosition, attachRotation);
        }

        public void ReturnParent()
        {
            hand.ReturnParent();
        }

        public void SetGrip(float value)
        {
            hand.SetGrip(value);
        }

        public void SetTrigger(float value)
        {
            hand.SetTrigger(value);

        }

        public void SetJoystickGrip(float value)
        {
            hand.SetJoystickGrip(value);
        }

        public void SetMoveJoystick(bool value)
        {
            hand.SetMovingJoystick(value);

        }
        #endregion
    }
}
