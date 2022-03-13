using System.Collections;
using UnityEngine;

namespace MacKay.PlayerController.Ship
{
    public class ShipExitWarpSubState : ShipBaseState
    {
        private IEnumerator cor_ExitState;
        public ShipExitWarpSubState(ShipStateMachine currentContext, ShipStateFactory shipStateFactory) : base
        (currentContext, shipStateFactory)
        {
        }

        public override void EnterState()
        {
            Ctx.WarpLens.gameObject.SetActive(false);
            WarpSpeedController.Instance.StopWarpEffect();
            Ctx.AudioController.PlayAudio(Audio.AudioType.ENGINE_DECEL_FAST_02);
            Ctx.AudioController.PlayAudio(Audio.AudioType.SFX_WARP_EXIT);
            
            cor_ExitState = Cor_ExitStateAfterSeconds(Ctx.WarTransDuration);
            Ctx.StartCoroutine(cor_ExitState);
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


        private IEnumerator Cor_ExitStateAfterSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);

            SwitchState(Factory.Drive());
        }
    }
}