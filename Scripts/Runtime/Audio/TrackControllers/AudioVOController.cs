using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace MacKay.Audio
{
    public class AudioVOController : MonoBehaviour
    {
        #region Singleton
        private static AudioVOController _instance;
        public static AudioVOController Instance => _instance;
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

        [SerializeField] AudioType[] _welcomeVO;
        [SerializeField] AudioType[] _orbitVO;
        [SerializeField] AudioType[] _tutorialVO;
        private IEnumerator _corOrbitVO;

        public void PlayWelcomeVO(int planetNumber)
        {
            AudioController.Instance.PlayAudio(_welcomeVO[planetNumber]);
        }
        
        public void PlayOrbitVO(int planetNumber)
        {
            if (_corOrbitVO != null) StopCoroutine(_corOrbitVO);
            
            _corOrbitVO = Cor_PlayOrbitVO(planetNumber);
            StartCoroutine(_corOrbitVO);
        }
        
        public void StopOrbitVO(int planetNumber)
        {
            if (_corOrbitVO != null) StopCoroutine(_corOrbitVO);
            
            AudioController.Instance.StopAudio(_orbitVO[planetNumber]);
        }
        
        public void PlayTutorialVO(int number)
        {
            AudioController.Instance.PlayAudio(_orbitVO[number]);
        }
        
        public void StopTutorialVO(int number)
        {
            AudioController.Instance.StopAudio(_orbitVO[number]);
        }

        private IEnumerator Cor_PlayOrbitVO(int planetNumber)
        {
            AudioController.Instance.PlayAudio(AudioType.VO_ENTERING_ORBIT);

            yield return new WaitForSeconds(2.5f);
            
            AudioController.Instance.PlayAudio(_orbitVO[planetNumber]);
        }
    }
}