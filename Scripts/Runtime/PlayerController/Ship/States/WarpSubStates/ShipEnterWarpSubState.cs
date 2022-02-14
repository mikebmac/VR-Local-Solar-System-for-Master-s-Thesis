using DG.Tweening;
using SpaceGraphicsToolkit;
using UnityEngine;

namespace MacKay.PlayerController.Ship
{
    public class ShipEnterWarpSubState : ShipBaseState
    {
        private SgtFloatingTarget _warpTarget;
        private float _warpOuterSizeReset = 0.001f;
        private float _warpHoleSizeReset = 0.99f;
        public ShipEnterWarpSubState(ShipStateMachine currentContext, ShipStateFactory shipStateFactory, 
        SgtFloatingTarget warpTarget) : base(currentContext, shipStateFactory)
        {
            _warpTarget = warpTarget;
        }

        public override void EnterState()
        {
            Ctx.AudioController.PlayAudio(Audio.AudioType.SFX_WARP_ENTER);
            Ctx.transform.DOLookAt(_warpTarget.transform.position, 2f).SetEase(Ease.InOutQuart)
            .OnComplete(StartWarpSequence);
        }

        public override void UpdateState()
        {
        }

        public override void ExitState()
        {
        }

        public override void CheckSwitchStates()
        {
        }

        public override void InitializeSubState()
        {
        }
        
        #region State Methods

        private void StartWarpSequence()
        {
            Ctx.WarpLens.transform.localPosition = Ctx.WarpStartPosition;
            Ctx.WarpLens.WarpOuter = _warpOuterSizeReset;
            Ctx.WarpLens.HoleSize = _warpHoleSizeReset;
            Ctx.WarpLens.gameObject.SetActive(true);
            float warpExitDuration = (float)Ctx.WarpSgtController.WarpTime - Ctx.WarTransDuration;
            Ctx.WarpLens.UpdateCubemap();
            
            // Bubble
            DOTween.To(() => Ctx.WarpLens.WarpOuter, x => Ctx.WarpLens.WarpOuter = x, Ctx.WarpBubbleOuterEndSize, Ctx
            .WarTransDuration)
                .SetEase(Ease.InExpo);

            // Entering Warp
            Ctx.WarpLens.transform.DOLocalMoveX(1.25f, 0.4f)
                .SetDelay(Ctx.WarTransDuration)
                .SetEase(Ease.InBack);
            
            // Hole
            DOTween.To(() => Ctx.WarpLens.HoleSize, x => Ctx.WarpLens.HoleSize = x, Ctx.WarpBubbleHolEndSize, 0.4f)
                .SetDelay(Ctx.WarTransDuration)
                .SetEase(Ease.InQuart)
                .OnComplete(() => { SwitchState(Factory.WarpTravel(_warpTarget, warpExitDuration)); });

        }
        #endregion
    }
}