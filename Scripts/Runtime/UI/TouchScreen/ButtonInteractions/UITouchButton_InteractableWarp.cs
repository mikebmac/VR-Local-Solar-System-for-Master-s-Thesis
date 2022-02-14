using System;
using MacKay.Audio;
using MacKay.Data;
using MacKay.PlayerController;
using UnityEngine;
using AudioType = UnityEngine.AudioType;

namespace MacKay.UI
{
    public class UITouchButton_InteractableWarp : UITouchButton_Interactable
    {
        public enum InteractableType
        {
            Mercury,
            Venus,
            Earth,
            Mars,
            Jupiter,
            Saturn,
            Uranus,
            Neptune,
            EnterOrbit
        }

        [SerializeField] private InteractableType _targetInteractableType;
        [SerializeField] private CelestailObjectData _planet;

        private void Start()
        {
            switch (_targetInteractableType)
            {
                case InteractableType.Mercury:
                    _planet = GameManager.Instance.Planets[0];
                    break;
                case InteractableType.Venus:
                    _planet = GameManager.Instance.Planets[1];
                    break;
                case InteractableType.Earth:
                    _planet = GameManager.Instance.Planets[2];
                    break;
                case InteractableType.Mars:
                    _planet = GameManager.Instance.Planets[3];
                    break;
                case InteractableType.Jupiter:
                    _planet = GameManager.Instance.Planets[4];
                    break;
                case InteractableType.Saturn:
                    _planet = GameManager.Instance.Planets[5];
                    break;
                case InteractableType.Uranus:
                    _planet = GameManager.Instance.Planets[6];
                    break;
                case InteractableType.Neptune:
                    _planet = GameManager.Instance.Planets[7];
                    break;
            }
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (String.Equals(other.tag, "PlayerTouch"))
            {
                if (_targetInteractableType == InteractableType.EnterOrbit)
                {
                    UITouchButton_ActionHandler.Instance.UISplashWarpToSplashOrbit();
                }
                else
                {
                    UITouchButton_ActionHandler.Instance.UIWarpSwitchToPlanet(_planet, (int)_targetInteractableType);
                }
                
                AudioController.Instance.PlayAudio(Audio.AudioType.SFX_UI_SELECT);

                PlayerHapticsController.Controller controller = other.name.Contains("Right")
                    ? PlayerHapticsController.Controller.RIGHT
                    : PlayerHapticsController.Controller.LEFT;
                PlayerHapticsController.Instance.VibrateController(controller, 0.75f, 
                    0.15f);
            }
        }
    }
}

