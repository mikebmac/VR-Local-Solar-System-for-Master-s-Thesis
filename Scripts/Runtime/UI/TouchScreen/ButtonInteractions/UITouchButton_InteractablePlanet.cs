using System;
using MacKay.Audio;
using MacKay.PlayerController;
using MacKay.PlayerController.Ship;
using MacKay.UI;
using UnityEngine;

namespace MacKay.UI
{
    public class UITouchButton_InteractablePlanet : UITouchButton_Interactable
    {
        public enum InteratableType
        {
            Warp,
            Back
        }
        [SerializeField] private InteratableType _interatableType;

        protected override void OnTriggerEnter(Collider other)
        {
            if (String.Equals(other.tag, "PlayerTouch"))
            {
                if (_interatableType == InteratableType.Back)
                {
                    UITouchButton_ActionHandler.Instance.UISplashPlanetBack();
                }
                else if (_interatableType == InteratableType.Warp)
                {
                    UITouchButton_ActionHandler.Instance.UISplashWarpRequest();
                }

                AudioController.Instance.PlayAudio(MacKay.Audio.AudioType.SFX_UI_SELECT);

                PlayerHapticsController.Controller controller = other.name.Contains("Right")
                    ? PlayerHapticsController.Controller.RIGHT
                    : PlayerHapticsController.Controller.LEFT;
                PlayerHapticsController.Instance.VibrateController(controller, 0.75f,
                    0.15f);
            }
        }
    }
}
