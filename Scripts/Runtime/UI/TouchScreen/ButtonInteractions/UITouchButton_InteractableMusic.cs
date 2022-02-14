using System;
using MacKay.Audio;
using MacKay.PlayerController;
using UnityEngine;

namespace MacKay.UI
{
    public class UITouchButton_InteractableMusic : UITouchButton_Interactable
    {
        public enum InteractableType
        {
            NextSong,
            PreviousSong,
            TogglePlaySong,
            IncreaseVolume,
            LowerVolume
        }
        [SerializeField] private InteractableType interactableType;
        
        protected override void OnTriggerEnter(Collider other)
        {
            if (String.Equals(other.tag, "PlayerTouch"))
            {
                if (interactableType == UITouchButton_InteractableMusic.InteractableType.NextSong)
                {
                    UITouchScreenController.Instance.NextTrack();
                }
                else if (interactableType == UITouchButton_InteractableMusic.InteractableType.PreviousSong)
                {
                    UITouchScreenController.Instance.PreviousTrack();
                }
                else if (interactableType == UITouchButton_InteractableMusic.InteractableType.TogglePlaySong)
                {
                    UITouchScreenController.Instance.TogglePlayButton();
                }
                else if (interactableType == UITouchButton_InteractableMusic.InteractableType.IncreaseVolume)
                {
                    UITouchScreenController.Instance.MusicIncreaseVolume();
                }
                else if (interactableType == UITouchButton_InteractableMusic.InteractableType.LowerVolume)
                {
                    UITouchScreenController.Instance.MusicLowerVolume();
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