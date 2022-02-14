using System.Collections;
using SpaceGraphicsToolkit;
using UnityEngine;

namespace MacKay.PlayerController.Ship
{
    public class ShipTravelWarpSubState : ShipBaseState
    {
        private SgtFloatingTarget _warpTarget;
        private float _warpExitTime;
        private float _warpTime;
        private IEnumerator _corCheckProgress;
        private float _previousTime = -1f;

        public ShipTravelWarpSubState(ShipStateMachine currentContext, ShipStateFactory shipStateFactory,
        SgtFloatingTarget warpTarget, float warpExitTime) : base
        (currentContext, shipStateFactory)
        {
            _warpTarget = warpTarget;
            _warpTime = (float) Ctx.WarpSgtController.WarpTime;
            _warpExitTime = warpExitTime;
        }

        public override void EnterState()
        {
            Ctx.WarpSgtController.WarpTo(_warpTarget);
            WarpSpeedController.Instance.StartWarpEffect();

            Ctx.AudioController.PlayAudio(Audio.AudioType.ENGINE_3X_LOOP, 0.4f, 0f, 0f, true);

            Ctx.PlayerAnimations.ShipRumble.StartRumbling(_warpTime, 0.015f);

            PlayerHapticsController.Instance.VibrateController(PlayerHapticsController.Controller.BOTH, 0.75f, 
                _warpTime);

            _corCheckProgress = Cor_CheckWarpProgress();
            Ctx.StartCoroutine(_corCheckProgress);
        }

        public override void UpdateState()
        {
            
        }

        public override void ExitState()
        {
            if (_corCheckProgress != null) Ctx.StopCoroutine(_corCheckProgress);
        }

        public override void CheckSwitchStates()
        {
            
        }

        public override void InitializeSubState()
        {
            
        }

        private IEnumerator Cor_CheckWarpProgress()
        {
            float elapsedTime = 0f;
            while (elapsedTime < _warpExitTime)
            {
                float deltaTime = 0f;
                if (_previousTime > 0f)
                {
                    deltaTime = Time.time - _previousTime;
                }

                elapsedTime += deltaTime;
                _previousTime = Time.time;
                
                yield return new WaitForSeconds(0.1f);
            }

            SwitchState(Factory.WarpExit());
        }
    }
}