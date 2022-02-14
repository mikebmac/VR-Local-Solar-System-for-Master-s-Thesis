using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace MacKay.PlayerController
{
    public class PlayerHapticsController : MonoBehaviour
    {
        [Header("Debug")]
        public bool enableDebug;

        private static PlayerHapticsController _instance;
        public static PlayerHapticsController Instance { get { return _instance; } }

        [Header("Controllers")]
        [SerializeField]
        private ActionBasedController leftController;
        [SerializeField]
        private ActionBasedController rightController;

        public enum Controller {
            LEFT,
            RIGHT,
            BOTH
        }

        #region Unity Methods
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }
        #endregion

        #region Public Methods
        #endregion

        #region Private Methods
        public void VibrateController (Controller controller, float amplitude, float duration)
        {
            switch (controller)
            {
                case Controller.LEFT:
                    leftController.SendHapticImpulse(amplitude, duration);
                    break;
                case Controller.RIGHT:
                    rightController.SendHapticImpulse(amplitude, duration);
                    break;
                case Controller.BOTH:
                    leftController.SendHapticImpulse(amplitude, duration);
                    rightController.SendHapticImpulse(amplitude, duration);
                    break;
                default:
                    break;
            }
            
        }
        #endregion
    }
}