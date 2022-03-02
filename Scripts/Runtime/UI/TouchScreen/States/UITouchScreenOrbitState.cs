using System.Collections;
using MacKay.Data;
using UnityEngine;
using DG.Tweening;
using MacKay.Audio;
using MacKay.PlayerController.Ship;

namespace MacKay.UI
{
    public class UITouchScreenOrbitState : UITouchScreenBaseState
    {
        private CelestailObjectData _currentPlanet;
        private int _planetNumber;
        private Collider _leaveButtonCollider;
        private Collider _orbitButtonCollider;
        
        public UITouchScreenOrbitState(UITouchStateMachine currentContext, UITouchScreenStateFactory shipStateFactory, CelestailObjectData planet, int planetNumber) : base(currentContext, shipStateFactory)
        {
            IsRootState = true;
            _currentPlanet = planet;
            _planetNumber = planetNumber;
            Ctx.OrbitingPlanetNumber = _planetNumber;
        }

        public override void EnterState()
        {
            UITouchButton_ActionHandler.Instance.OnSplashOrbitToSplashWarp += HandleSplashOrbitToSplashWarp;
            UITouchButton_ActionHandler.Instance.OnSplashOrbitEnterOrbit += HandleSplashOrbitEnterOrbit;
            UITouchButton_ActionHandler.Instance.OnSplashOrbitLeaveOrbit += HandleSplashOrbitLeaveOrbit;
            ShipActionHandler.Instance.OnAutoPilotUIUpdate += HandleAutoPilotUIUpdate;
            
            Ctx.OrbitMeshRenderer.material = Ctx.PlanetMaterials[_planetNumber];
            Ctx.OrbitPlanetText.text = $"<b>Planet:</b> {_currentPlanet.objectName}";

            _leaveButtonCollider = Ctx.OrbitButtonLeave.GetComponentInChildren<Collider>();
            _orbitButtonCollider = Ctx.OrbitButtonOrbit.GetComponentInChildren<Collider>();
            
            Ctx.StartCoroutine(Cor_WaitForInit());
        }

        public override void UpdateState()
        {
            
            
        }

        public override void ExitState()
        {
            if (Ctx.OrbitProgressImage.fillAmount > 0f)
            {
                _orbitButtonCollider.enabled = true;
                _leaveButtonCollider.enabled = false;
                Ctx.OrbitButtonOrbit.transform.localScale = Vector3.zero;
                Ctx.OrbitProgressImage.fillAmount = 0f;
                
                Ctx.OrbitButtonLeave.SetActive(false);
                Ctx.OrbitButtonOrbit.SetActive(true);
            }
            
            UITouchButton_ActionHandler.Instance.OnSplashOrbitToSplashWarp -= HandleSplashOrbitToSplashWarp;
            UITouchButton_ActionHandler.Instance.OnSplashOrbitEnterOrbit -= HandleSplashOrbitEnterOrbit;
            UITouchButton_ActionHandler.Instance.OnSplashOrbitLeaveOrbit -= HandleSplashOrbitLeaveOrbit;
            ShipActionHandler.Instance.OnAutoPilotUIUpdate -= HandleAutoPilotUIUpdate;
            
            Ctx.OrbitManager.startDisappear();
        }

        public override void CheckSwitchStates()
        {
            
        }

        public override void InitializeSubState()
        {
            
        }
        
        private IEnumerator Cor_WaitForInit()
        {
            yield return new WaitForSeconds(1f);
        
            Ctx.OrbitManager.startAppear();
        }
        
        private void HandleSplashOrbitToSplashWarp()
        {
            SwitchState(Factory.Warp());
        }
        
        private void HandleSplashOrbitEnterOrbit()
        {
            _orbitButtonCollider.enabled = false;
            _leaveButtonCollider.enabled = false;
            Ctx.OrbitButtonLeave.transform.localScale = Vector3.zero;
            Ctx.OrbitProgressImage.fillAmount = 0f;
            
            AudioVOController.Instance.PlayOrbitVO(_planetNumber);

            Ctx.OrbitButtonOrbit.transform.DOScale(0f, 0.2f).SetEase(Ease.OutBounce)
                .OnComplete(() =>
                {
                    Ctx.OrbitButtonOrbit.SetActive(false);
                    Ctx.OrbitButtonLeave.SetActive(true);
                    Ctx.OrbitButtonLeave.transform.DOScale(1f, 0.2f).SetEase(Ease.InBounce).SetDelay(1f)
                        .OnComplete(() => _leaveButtonCollider.enabled = true);
                });
        }
        
        private void HandleSplashOrbitLeaveOrbit()
        {
            _orbitButtonCollider.enabled = false;
            _leaveButtonCollider.enabled = false;
            Ctx.OrbitButtonOrbit.transform.localScale = Vector3.zero;
            Ctx.OrbitProgressImage.fillAmount = 0f;
            
            AudioVOController.Instance.StopOrbitVO(_planetNumber);

            Ctx.OrbitButtonLeave.transform.DOScale(0f, 0.2f).SetEase(Ease.OutBounce)
                .OnComplete(() =>
                {
                    Ctx.OrbitButtonLeave.SetActive(false);
                    Ctx.OrbitButtonOrbit.SetActive(true);
                    Ctx.OrbitButtonOrbit.transform.DOScale(1f, 0.2f).SetEase(Ease.InBounce).SetDelay(1f)
                        .OnComplete(() => _orbitButtonCollider.enabled = true);
                });
        }

        private void HandleAutoPilotUIUpdate(double angle)
        {
            if (angle > 360d) angle = 360d;
            float fill = Mathf.InverseLerp(0f, 360f, (float) angle);
            
            Ctx.OrbitProgressImage.fillAmount = fill;
        }
    }
}