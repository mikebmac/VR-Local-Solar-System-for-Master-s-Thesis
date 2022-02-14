using System;
using SpaceGraphicsToolkit;
using UnityEngine;

namespace MacKay.UI
{
    public class UIController : MonoBehaviour
    {
        #region Singleton

        private static UIController _instance;
        public static UIController Instance => _instance;

        #endregion
        
        public struct SpeedUnits
        {
            public SpeedSuffix units;
            public float speed;

            public SpeedUnits(SpeedSuffix units, float speed)
            {
                this.units = units;
                this.speed = speed;
            }
        }
        public enum SpeedSuffix
        {
            Light,
            Millions,
            Thousands,
            None
        }
        
        [SerializeField] private SgtFloatingSpeedometer sgtSpeedometerController;
        public SgtFloatingSpeedometer SgtSpeedometerController => sgtSpeedometerController;
        [SerializeField] private SpeedometerController speedometerController;
        public SpeedometerController SpeedometerController => speedometerController;
        private SpeedUnits speedUnits;
        public SpeedUnits CurrentSpeed => speedUnits;

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

        private void OnEnable()
        {
            sgtSpeedometerController.OnSpeedUpdate += HandleSpeedUpdate;
        }
        
        private void OnDisable()
        {
            sgtSpeedometerController.OnSpeedUpdate -= HandleSpeedUpdate;
        }

        #endregion

        #region Private Methods

        private void HandleSpeedUpdate(double speed)
        {
            speedUnits = SanitizeSpeed(speed);
            speedometerController.SetNumberDisplay(speedUnits);
        }

        private SpeedUnits SanitizeSpeed(double speed)
        {
            float fSpeed = 0;
            SpeedSuffix units = SpeedSuffix.None;
            if (speed > (Constants.SpeedOfLight * 0.01))
            {
                fSpeed = (float) speed / Constants.SpeedOfLight;
                if (fSpeed >= 100f)
                {
                    fSpeed = Mathf.Round(fSpeed);
                } 
                else if (fSpeed >= 10f)
                {
                    fSpeed *= 10f;
                    fSpeed = Mathf.Round(fSpeed) / 10f;
                }
                else
                {
                    fSpeed *= 100f;
                    fSpeed = Mathf.Round(fSpeed) / 100f;
                }
                
                units = SpeedSuffix.Light;
            }
            else
            {
                // Convert to KM/Hr
                fSpeed = (float) speed * 3.6f;
                if (fSpeed > 1000000f)
                {
                    fSpeed /= 1000000f;
                    units = SpeedSuffix.Millions;
                }
                else if (fSpeed > 1000f)
                {
                    fSpeed /= 1000f;
                    units = SpeedSuffix.Thousands;
                }
                else
                {
                    units = SpeedSuffix.None;
                }
                fSpeed = Mathf.Round(fSpeed);
            }
            return new SpeedUnits(units, fSpeed);
        }

        #endregion
    }
}