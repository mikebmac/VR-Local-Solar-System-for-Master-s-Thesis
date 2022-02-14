using MacKay.Audio;
using SpaceGraphicsToolkit;

namespace MacKay.PlayerController.Ship
{
    public class ShipWarpState: ShipBaseState
    {
        private SgtFloatingTarget _warpTarget;
        private int _planetNumber;

        public ShipWarpState(ShipStateMachine currentContext, ShipStateFactory shipStateFactory, SgtFloatingTarget 
        target, double distance) :
         base
            (currentContext, shipStateFactory)
        {
            double warpTime = distance / Ctx.WarpSpeed;
            // Minimum Warp Time of 10 seconds
            if (warpTime < 10d) warpTime = 10d;
            Ctx.WarpSgtController.WarpTime = warpTime;
            
            _warpTarget = target;
            IsRootState = true;
            InitializeSubState();
        }

        public override void EnterState()
        {
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
            SetSubState(Factory.WarpStart(_warpTarget));
            CurrentSubState.EnterState();
        }
    }
}