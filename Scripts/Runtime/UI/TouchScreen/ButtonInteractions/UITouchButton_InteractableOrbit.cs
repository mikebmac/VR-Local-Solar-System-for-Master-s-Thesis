using System;
using MacKay.Audio;
using MacKay.PlayerController;
using UnityEngine;

namespace MacKay.UI
{
    public class UITouchButton_InteractableOrbit : UITouchButton_Interactable
    {
        public enum InteractableType
        {
            SplashWarp,
            EnterOrbit,
            LeaveOrbit
        }
        [SerializeField] private InteractableType interactableType;
        
        protected override void OnTriggerEnter(Collider other)
        {
            if (String.Equals(other.tag, "PlayerTouch"))
            {
                if (interactableType == InteractableType.SplashWarp)
                {
                    UITouchButton_ActionHandler.Instance.UISplashOrbitToSplashWarp();
                }
                else if (interactableType == InteractableType.EnterOrbit)
                {
                    UITouchButton_ActionHandler.Instance.UISplashOrbitEnterOrbit();
                }
                else if (interactableType == InteractableType.LeaveOrbit)
                {
                    UITouchButton_ActionHandler.Instance.UISplashOrbitLeaveOrbit();
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