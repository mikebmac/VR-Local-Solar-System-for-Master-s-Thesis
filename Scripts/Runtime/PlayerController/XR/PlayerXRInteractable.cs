using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MacKay.PlayerController
{
    public abstract class PlayerXRInteractable : MonoBehaviour
    {
        public enum InteractableType
        {
            NONE,
            BASIC,
            JOYSTICK
        }

        public abstract InteractableType Type { get; }
        public abstract Transform GrabPoint { get; }
        public abstract Vector3 GrabOffsetPosition { get; }
        public abstract Quaternion GrabOffsetRotation { get; }
        public abstract bool InUse { get; set; }
    }
}