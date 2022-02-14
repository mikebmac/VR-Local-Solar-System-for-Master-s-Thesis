using System.Collections;
using MacKay.UI;
using UnityEngine;
using UnityEngine.VFX;

public class WarpSpeedController : MonoBehaviour
{
    #region Singleton

    private static WarpSpeedController _instance;
    public static WarpSpeedController Instance => _instance;

    #endregion
    
    [SerializeField] private bool enableDebug = false;

    [SerializeField] private MeshRenderer warpTunnel;
    [SerializeField] private VisualEffect warpSpeedVFX;
    private IEnumerator warpEffectCor;
    private IEnumerator warpTunnelCor;
    private bool isWarping = false;

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

    private void Start()
    {
        warpSpeedVFX.Stop();
    }

    #endregion
    
    #region Public Methods
    public void StartWarpEffect()
    {
        if (warpEffectCor != null) StopCoroutine(warpEffectCor);
        isWarping = true;
        warpEffectCor = Cor_WarpEffect();
        StartCoroutine(warpEffectCor);
    }
    public void StopWarpEffect()
    {
        isWarping = false;
    }
    
    public void StartWarpTunnelEffect(float duration = 1f)
    {
        if (warpTunnelCor != null) StopCoroutine(warpTunnelCor);
        warpTunnelCor = Cor_FadeWarpTunnel(0f, 1f, duration);
        StartCoroutine(warpTunnelCor);
    }
    
    public void StopWarpTunnelEffect(float duration = 1f)
    {
        if (warpTunnelCor != null) StopCoroutine(warpTunnelCor);
        warpTunnelCor = Cor_FadeWarpTunnel(1f, 0f, duration);
        StartCoroutine(warpTunnelCor);
    }
    #endregion
    
    #region Private Methods
    private IEnumerator Cor_WarpEffect()
    {
        warpSpeedVFX.SetFloat("warpSpeed", 0f);
        warpSpeedVFX.Play();
        float warpSpeed = 0f;
        while (isWarping)
        {
            if (UIController.Instance.CurrentSpeed.units == UIController.SpeedSuffix.Light)
            {
                warpSpeed = Mathf.Clamp(UIController.Instance.CurrentSpeed.speed / 100f, 0f, 1f);
            }
            warpSpeedVFX.SetFloat("warpSpeed", warpSpeed);
            yield return new WaitForSeconds(0.1f);
        }
        warpSpeedVFX.Stop();
    }

    private IEnumerator Cor_FadeWarpTunnel(float from, float to, float duration)
    {
        float elapsedTime = 0f;
        float previousTime = -1f;
        while (elapsedTime <= duration)
        {
            float deltaTime = 0f;
            if (previousTime > 0f)
            {
                deltaTime = Time.time - previousTime;
            }
            elapsedTime += deltaTime;
            previousTime = Time.time;

            float currentFadeValue = Mathf.Lerp(from, to, elapsedTime / duration);
            warpTunnel.material.SetFloat("_Active", currentFadeValue);

            yield return new WaitForSeconds(0.1f);
        }
    }
    
    private void Log(string msg)
    {
        if (!enableDebug) return;

        Debug.Log($"<color=green>[{GetType()}]</color>: {msg}", this);
    }

    private void LogWarning(string msg, bool overrideSettings = false)
    {
        if (!enableDebug && !overrideSettings) return;
        Debug.LogWarning($"<color=red>[{GetType()}]</color>: {msg}", this);
    }
    #endregion
}
