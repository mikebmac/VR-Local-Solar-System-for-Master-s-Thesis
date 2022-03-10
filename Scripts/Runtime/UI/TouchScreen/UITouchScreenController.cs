using System.Collections;
using MacKay.Audio;
using MacKay.PlayerController.Ship;
using SpaceGraphicsToolkit;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using AudioType = MacKay.Audio.AudioType;

namespace MacKay.UI
{
    public class UITouchScreenController : MonoBehaviour
    {
        #region Singleton
        private static UITouchScreenController _instance;
        public static UITouchScreenController Instance => _instance;

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

        [SerializeField] private SgtFloatingSpeedometer _speedometer;
        [SerializeField] private Image[] _volumeBlips;
        [SerializeField] private Image _musicProgress;
        [SerializeField] private Image _playImage;
        [SerializeField] private Sprite _playSprite;
        [SerializeField] private Sprite _stopSprite;
        [SerializeField] private Color _volumeBlipOnColor;
        [SerializeField] private Color _volumeBlipOffColor;
        [SerializeField] private Text _songTitle;
        [SerializeField] private Text _statsText;
        [SerializeField] private bool _isPlayingMusic = false;
        [SerializeField] private AudioType[] _musicTracks;
        [SerializeField] private int _currentTrack = -1;
        [SerializeField] private AudioMixer _musicMixer;
        [SerializeField] private SgtFloatingObject _distanceTracker;
        [SerializeField] private SgtFloatingCamera _player;
        private double _totalTravelDistance = 0d;
        private string _currentPlanet;
        private double _currentDistanceToPlanet = 0f;
        private float _currentMusicVolume = 0.6f;
        private IEnumerator _musicCurrentProgress;
        private IEnumerator _updateStatsText;
        private IEnumerator _trackDistance;

        private void Start()
        {
            ShipActionHandler.Instance.OnCurrentPlanetUpdate += HandleCelestialObjectUpdate;
            _musicCurrentProgress = Cor_MusicTrackProgress();
            _updateStatsText = Cor_UpdateStatsText();
            _trackDistance = Cor_CalculateDistance();

            _distanceTracker = new GameObject("Distance Tracker").AddComponent<SgtFloatingObject>();
            _distanceTracker.transform.SetParent(FindObjectOfType<SgtFloatingRoot>().transform);
            _distanceTracker.SetPosition(_player.Position);

            StartCoroutine(_trackDistance);
            StartCoroutine(_updateStatsText);
        }

        private void OnDisable()
        {
            ShipActionHandler.Instance.OnCurrentPlanetUpdate -= HandleCelestialObjectUpdate;
            if(_trackDistance != null) StopCoroutine(_trackDistance);
            if(_musicCurrentProgress != null) StopCoroutine(_musicCurrentProgress);
            if(_updateStatsText != null) StopCoroutine(_updateStatsText);
        }

        public void NextTrack()
        {
            if (_currentTrack < 0)
            {
                _currentTrack = 0;
            }
            else
            {
                _currentTrack++;
                if (_currentTrack >= _musicTracks.Length) _currentTrack = 0;
            }
            
            PlaySong();
        }

        public void PreviousTrack()
        {
            if (_currentTrack >= _musicTracks.Length)
            {
                _currentTrack = _musicTracks.Length - 1;
            }
            else
            {
                _currentTrack--;
                if (_currentTrack < 0) _currentTrack = _musicTracks.Length - 1;
            }
            
            PlaySong();
        }
        
        public void MusicIncreaseVolume()
        {
            _currentMusicVolume += 0.1f;
            if (_currentMusicVolume > 1f)
            {
                _currentMusicVolume = 1f;
            }

            SetMusicVolume(_currentMusicVolume);
        }

        public void MusicLowerVolume()
        {
            _currentMusicVolume -= 0.1f;
            if (_currentMusicVolume <= 0f)
            {
                _currentMusicVolume = 0.0001f;
            }

            SetMusicVolume(_currentMusicVolume);
        }

        public void TogglePlayButton()
        {
            Debug.Log($"Before: {_isPlayingMusic}");
            if (_isPlayingMusic)
            {
                PauseSong();
            }
            else
            {
                UnpauseSong();
            }
            Debug.Log($"After: {_isPlayingMusic}");
        }

        private void SetMusicVolume(float percent)
        {
            _musicMixer.SetFloat("MusicVolume", Mathf.Log10(percent) * 20f);
            SetMusicVolumeBlips(percent);
        }

        private void SetMusicVolumeBlips(float percent)
        {
            if (percent < 0.1f)
            {
                percent = 0f;
            }
            int percentToBlips = (int)(percent * 10f);

            for (int i = 0; i < _volumeBlips.Length; i++)
            {
                _volumeBlips[i].color = i < percentToBlips ? _volumeBlipOnColor : _volumeBlipOffColor;
            }
        }

        private void PlaySong()
        {
            _isPlayingMusic = true;
            AudioController.Instance.PlayAudio(_musicTracks[_currentTrack]);
            _songTitle.text = _musicTracks[_currentTrack].ToString();
            CheckPlayState();
            
            StartMusicTrackProgress();
        }
        
        private void UnpauseSong()
        {

            if (_currentTrack < 0)
            {
                _currentTrack = 0;
                PlaySong();
                return;
            }
            
            _isPlayingMusic = true;

            CheckPlayState();
            AudioController.Instance.UnpauseAudio(_musicTracks[_currentTrack]);
            
            StartMusicTrackProgress();
        }

        private void PauseSong()
        {
            _isPlayingMusic = false;
            
            StopCoroutine(_musicCurrentProgress);
            AudioController.Instance.PauseAudio(_musicTracks[_currentTrack]);
            
            CheckPlayState();
        }

        private void CheckPlayState()
        {
            _playImage.sprite = _isPlayingMusic ? _stopSprite : _playSprite;
        }

        private void HandleCelestialObjectUpdate(string planet, double distance)
        {
            _currentPlanet = planet;
            _currentDistanceToPlanet = distance / 1000d;
        }

        private void StartMusicTrackProgress()
        {
            if (_musicCurrentProgress != null) StopCoroutine(_musicCurrentProgress);

            _musicCurrentProgress = Cor_MusicTrackProgress();
            StartCoroutine(_musicCurrentProgress);

        }
        
        private IEnumerator Cor_MusicTrackProgress()
        {
            AudioSource audioSource = AudioController.Instance.tracks[0].audioSource;
            yield return new WaitUntil(() => audioSource != null);
            float clipDuration = audioSource.clip.length;

            while (audioSource.isPlaying)
            {
                _musicProgress.fillAmount = audioSource.time / clipDuration;
                yield return new WaitForSeconds(1f);
            }
            
            NextTrack();
        }

        private IEnumerator Cor_UpdateStatsText()
        {
            while (Application.isPlaying)
            {
                string totalDistance = $"{_totalTravelDistance:n0}";
                string planetDistance = $"{_currentDistanceToPlanet:n0}";
                string text = $"<b>DISTANCE TRAVELLED:</b>\n{totalDistance} km\n\n" +
                       "<b>CLOSET CELESTIAL OBJECT</b>\n" +
                       $"<b><b>Name:</b> {_currentPlanet}</b>\n" +
                       $"<b><b>Distance:</b> {planetDistance} km</b>";
                
                _statsText.text = text;
                
                yield return new WaitForSeconds(2f);
            }
        }
        
        private IEnumerator Cor_CalculateDistance()
        {
            while (Application.isPlaying)
            {
                _totalTravelDistance += SgtPosition.Distance(_distanceTracker.Position, _player.Position) / 1000d;
                _distanceTracker.Position = _player.Position;

                yield return new WaitForSeconds(2f);
            }
        }
    }
}