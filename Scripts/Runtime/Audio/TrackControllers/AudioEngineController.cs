using System;
using MacKay.PlayerController.Ship;
using MacKay.UI;
using UnityEngine;

namespace MacKay.Audio
{
    public class AudioEngineController : MonoBehaviour
    {
        #region Singleton
        private static AudioEngineController _instance;
        public static AudioEngineController Instance => _instance;
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
        
        private AudioType _currentAudioType = AudioType.NONE;

        private void OnEnable()
        {
            ShipActionHandler.Instance.OnShipEngineAudioTick += CheckEngineAudio;
        }

        private void OnDisable()
        {
            ShipActionHandler.Instance.OnShipEngineAudioTick -= CheckEngineAudio;
        }

        public void SetEngineAudio(AudioType audioType)
        {
            if (_currentAudioType == audioType) return;

            _currentAudioType = audioType;
            AudioController.Instance.PlayAudio(audioType, 0f, 0.1f, 0f, true);
        }
        
        public void StopEngineAudio()
        {
            AudioController.Instance.StopAudio(_currentAudioType);
            _currentAudioType = AudioType.NONE;
        }

        public void CheckEngineAudio(UIController.SpeedUnits speedUnits)
        {
            if (speedUnits.units == UIController.SpeedSuffix.Millions && speedUnits.speed > 4f)
            {
                SetEngineAudio(AudioType.ENGINE_2X_LOOP);
            }
            else if (speedUnits.units == UIController.SpeedSuffix.Millions)
            {
                SetEngineAudio(AudioType.ENGINE_1X_LOOP);
            }
            else if (speedUnits.units == UIController.SpeedSuffix.Thousands)
            {
                SetEngineAudio(AudioType.ENGINE_0X_LOOP);
            }
            else
            {
                if (_currentAudioType != AudioType.NONE)
                {
                    StopEngineAudio();
                }
            }
        }

    }
}