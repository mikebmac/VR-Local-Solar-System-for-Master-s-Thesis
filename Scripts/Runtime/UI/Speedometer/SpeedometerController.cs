using MacKay.Audio;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using AudioType = MacKay.Audio.AudioType;

namespace MacKay.UI
{
    public class SpeedometerController : MonoBehaviour
    {
        [SerializeField] private bool enableDebug = false;

        [Header(("UI Elements"))] [SerializeField]
        private Image lightspeedImage;

        [SerializeField] private Image sublightspeedImage;
        [SerializeField] private RectTransform throttleRect;
        [SerializeField] private TextMeshProUGUI numberDisplay;
        [SerializeField] private TextMeshProUGUI suffixDisplay;

        private UIController.SpeedSuffix currentSpeedSuffix = UIController.SpeedSuffix.None;

        #region Unity Methods

        private void Awake()
        {
            if (!lightspeedImage) LogWarning($"LightspeedImage was not found.", true);
            if (!sublightspeedImage) LogWarning($"SublightspeedImage was not found.", true);
            if (!throttleRect) LogWarning($"ThrottleRect was not found.", true);
            if (!numberDisplay) LogWarning($"NumberDisplay was not found.", true);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set the lightspeed visual display.
        /// </summary>
        /// <param name="target">Between 0 and 1.</param>
        public void SetLighspeedDisplay(float target)
        {
            lightspeedImage.fillAmount = Mathf.Lerp(lightspeedImage.fillAmount, target, Time.deltaTime);
        }

        /// <summary>
        /// Set the sub-lightspeed visual display.
        /// </summary>
        /// <param name="target">Between 0 and 1.</param>
        public void SetSublighspeedDisplay(float target)
        {
            sublightspeedImage.fillAmount = Mathf.Lerp(sublightspeedImage.fillAmount, target, Time.deltaTime);
        }

        /// <summary>
        /// Set the throttle visual display.
        /// </summary>
        /// <param name="value">Between 0 and 1.</param>
        public void SetThrottleDisplay(float value)
        {
            float min = 0f;
            float max = 135f;
            float degrees = Mathf.LerpAngle(min, max, Mathf.Abs(value));
            if (value > 0) degrees *= -1f;
            throttleRect.localRotation = Quaternion.Euler(0f, 0f, degrees);
        }

        /// <summary>
        /// Set the numbers to visually display.
        /// </summary>
        /// <param name="speed">The speed simplified for display.</param>
        public void SetNumberDisplay(UIController.SpeedUnits speed)
        {
            string speedString = speed.speed.ToString();
            numberDisplay.text = speedString;
            
            if (speed.units == UIController.SpeedSuffix.Light)
            {
                SetLighspeedDisplay(Mathf.Clamp(speed.speed, 0f, 1f));
                SetSublighspeedDisplay(1f);
            }
            else
            {
                SetLighspeedDisplay(0f);
                float subLightSpeed = 0f;
                if (speed.units == UIController.SpeedSuffix.Millions)
                {
                    subLightSpeed = (speed.speed * 1000000f) / 10793000f;
                }
                else if (speed.units == UIController.SpeedSuffix.Thousands)
                {
                    subLightSpeed = (speed.speed * 1000f) / 10793000f;
                }
                else
                {
                    subLightSpeed = speed.speed / 10793000f;
                }
                SetSublighspeedDisplay(subLightSpeed);
            }
            
            if (currentSpeedSuffix != speed.units) SetSpeedSuffix(speed.units);
        }
        
        /// <summary>
        /// Set the numbers to visually display.
        /// </summary>
        /// <param name="value">The string to display directly. Make sure it is formatted.</param>
        public void SetNumberDisplay(string value)
        {
            numberDisplay.text = value;
        }

        #endregion

        #region Private Methods

        private void SetSpeedSuffix(UIController.SpeedSuffix suffix)
        {
            switch (suffix)
            {
                case (UIController.SpeedSuffix.Light):
                    suffixDisplay.text = "Light";
                    break;
                case (UIController.SpeedSuffix.Millions):
                    suffixDisplay.text = "M Km/Hr";
                    break;
                case (UIController.SpeedSuffix.Thousands):
                    suffixDisplay.text = "K Km/Hr";
                    break;
                case (UIController.SpeedSuffix.None):
                    suffixDisplay.text = "Km/Hr";
                    break;
            }
        }

        private void Log(string msg)
        {
            if (!enableDebug) return;

            Debug.Log($"<color=green>[{this.GetType().ToString()}]</color>: {msg}", this);
        }

        private void LogWarning(string msg, bool overrideSettings = false)
        {
            if (!enableDebug && !overrideSettings) return;
            Debug.LogWarning($"<color=red>[{this.GetType().ToString()}]</color>: {msg}", this);
        }

        #endregion
    }
}