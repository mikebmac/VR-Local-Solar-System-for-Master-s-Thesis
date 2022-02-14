using System;
using System.Collections;
using UnityEngine;

namespace MacKay.Audio
{
    #region Audio Class Structures
    [Serializable] public class AudioObject
    {
        public AudioType type;
        public AudioClip clip;
    }

    [Serializable] public class AudioTrack
    {
        public AudioSource audioSource;
        public AudioSource crossFadeAudioSource;
        public AudioObject[] audio;
    }
    #endregion

    public class AudioController : MonoBehaviour
    {
        private static AudioController _instance;
        public static AudioController Instance => _instance;

        [Header("Debug")]
        [SerializeField] private bool _enableDebug;

        [Header("Audio")]
        public AudioTrack[] tracks;

        private Hashtable _audioTable;
        private Hashtable _jobTable;

        private class AudioJob
        {
            public AudioAction action;
            public AudioType type;
            public float fade;
            public float crossFade;
            public float delay;
            public bool loop;

            public AudioJob(AudioAction action, AudioType type, float fade, float crossFade, float delay, bool loop)
            {
                this.action = action;
                this.type = type;
                this.fade = fade;
                this.crossFade = crossFade;
                this.delay = delay;
                this.loop = loop;
            }
        }

        private enum AudioAction
        {
            Start,
            Stop,
            Restart,
            Pause,
            Unpause
        }

        #region Unity Methods
        private void Awake() {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Configure();
            }
        }

        private void OnDisable()
        {
            Dispose();
        }
        #endregion

        #region Public Methods
        public void PlayAudio(AudioType type, float fade = 0f, float crossFade = 0f, float delay = 0f, bool loop = false) 
        {
            AddJob(new AudioJob(AudioAction.Start, type, fade, crossFade, delay, loop));
        }

        public void StopAudio(AudioType type, float fade = 0f, float crossFade = 0f, float delay = 0f, bool loop = false) 
        { 
            AddJob(new AudioJob(AudioAction.Stop, type, fade, crossFade, delay, loop));
        }

        public void RestartAudio(AudioType type, float fade = 0f, float crossFade = 0f, float delay = 0f, bool loop = false) 
        {
            AddJob(new AudioJob(AudioAction.Restart, type, fade, crossFade, delay, loop));
        }
        
        public void PauseAudio(AudioType type, float fade = 0f, float crossFade = 0f, float delay = 0f, bool loop = false) 
        {
            AddJob(new AudioJob(AudioAction.Pause, type, fade, crossFade, delay, loop));
        }
        
        public void UnpauseAudio(AudioType type, float fade = 0f, float crossFade = 0f, float delay = 0f, bool loop = false) 
        {
            AddJob(new AudioJob(AudioAction.Unpause, type, fade, crossFade, delay, loop));
        }

        #endregion

        #region Private Methods
        private void Configure()
        {
            _instance = this;

            _audioTable = new Hashtable();
            _jobTable = new Hashtable();
            PopulateAudioTable();
        }

        private void Dispose()
        {
            foreach (DictionaryEntry entry in _jobTable)
            {
                IEnumerator job = (IEnumerator)entry.Value;
                StopCoroutine(job);
            }
        }

        private void PopulateAudioTable()
        {
            foreach (AudioTrack track in tracks)
            {
                foreach (AudioObject audioObject in track.audio)
                {
                    if(_audioTable.ContainsKey(audioObject.type))
                    {
                        LogWarning("You are trying to audio [" + audioObject.type + "] that is already registered.");
                    } else
                    {
                        _audioTable.Add(audioObject.type, track);
                        Log("Registering audio [" + audioObject.type + "].");
                    }
                }
            }
        }

        private IEnumerator RunAudioJob(AudioJob job)
        {

            yield return new WaitForSeconds(job.delay);

            AudioTrack track = (AudioTrack)_audioTable[job.type];
            

            if (job.crossFade > 0f)
            {
                AudioSource newMainAudioSource;
                AudioSource newCrossFadeAudioSource;
                if (track.audioSource.time > 0f)
                {
                    newCrossFadeAudioSource = track.audioSource;
                    newMainAudioSource = track.crossFadeAudioSource;
                }
                else
                {
                    newCrossFadeAudioSource = track.crossFadeAudioSource;
                    newMainAudioSource = track.audioSource;
                }

                track.audioSource = newMainAudioSource;
                track.crossFadeAudioSource = newCrossFadeAudioSource;
                if (job.fade == 0f) job.fade = job.crossFade;
            }
            else
            {
                if (job.fade == 0f) job.fade = job.crossFade;
            }
            
            track.audioSource.clip = GetAudioClipFromAudioTrack(job.type, track);

            track.audioSource.loop = job.loop;

            switch (job.action)
            {
                case AudioAction.Start:
                    track.audioSource.Play();
                    break;
                case AudioAction.Stop:
                    if(job.fade <= 0f)
                    {
                        track.audioSource.Stop();
                    }
                    break;
                case AudioAction.Restart:
                    track.audioSource.Stop();
                    track.audioSource.Play();
                    break;
                case AudioAction.Pause:
                    track.audioSource.Pause();
                    break;
                case AudioAction.Unpause:
                    track.audioSource.UnPause();
                    break;
            }
            
            if (job.fade > 0f)
            {
                float initial = job.action == AudioAction.Start || job.action == AudioAction.Restart ? 0f : 1f;
                float target = initial == 1f ? 0f : 1f;
                float timer = 0f;

                while(timer <= job.fade)
                {
                    track.audioSource.volume = Mathf.Lerp(initial, target, timer / job.fade);
                    if (job.crossFade > 0f)
                    {
                        track.crossFadeAudioSource.volume = Mathf.Lerp(target, initial, timer / job.fade);
                    }
                    timer += Time.deltaTime;
                    yield return null;
                }

                track.audioSource.volume = target;
                if (job.crossFade > 0f)
                {
                    track.crossFadeAudioSource.volume = initial;
                    track.crossFadeAudioSource.Stop();
                }

                if (job.action == AudioAction.Stop)
                {
                    track.audioSource.Stop();
                }
            }

            _jobTable.Remove(job.type);

            yield return null;
        }

        private void AddJob(AudioJob job)
        {
            RemoveConflictingJobs(job.type);

            IEnumerator jobRunner = RunAudioJob(job);
            _jobTable.Add(job.type, jobRunner);
            StartCoroutine(jobRunner);

            Log("Starting job on [" + job.type + "] with operation: " + job.action);
        }

        private void RemoveJob(AudioType audioType)
        {
            if (!_jobTable.ContainsKey(audioType))
            {
                LogWarning("Trying to stop a job [" + audioType + "] that is not running.");
                return;
            }

            IEnumerator runningJob = (IEnumerator)_jobTable[audioType];
            StopCoroutine(runningJob);
            _jobTable.Remove(audioType);
        }

        private void RemoveConflictingJobs(AudioType audioType)
        {
            if (_jobTable.ContainsKey(audioType))
            {
                RemoveJob(audioType);
            }

            AudioType conflictingAudio = AudioType.NONE;
            foreach (DictionaryEntry entry in _jobTable)
            {
                AudioType conflictingAudioType = (AudioType)entry.Key;
                AudioTrack audioTrackInUse = (AudioTrack)_audioTable[conflictingAudioType];
                AudioTrack audioTrackNeeded = (AudioTrack)_audioTable[audioType];

                if (audioTrackNeeded.audioSource == audioTrackInUse.audioSource)
                {
                    conflictingAudio = conflictingAudioType;
                }
            }

            if (conflictingAudio != AudioType.NONE)
            {
                RemoveJob(conflictingAudio);
            }
        }

        private AudioClip GetAudioClipFromAudioTrack(AudioType audioType, AudioTrack track)
        {
            foreach (AudioObject obj in track.audio)
            {
                if (obj.type == audioType)
                {
                    return obj.clip;
                }
            }
            return null;
        }

        private void Log(string msg)
        {
            if (!_enableDebug) return;

            Debug.Log($"<color=green>[{GetType()}]</color>: {msg}", this);
        }

        private void LogWarning(string msg, bool overrideSettings = false)
        {
            if (!_enableDebug && !overrideSettings) return;
            Debug.LogWarning($"<color=red>[{GetType()}]</color>: {msg}", this);
        }
        #endregion
    }
}