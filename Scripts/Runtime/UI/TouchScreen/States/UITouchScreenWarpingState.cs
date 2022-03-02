using System;
using MacKay.Audio;
using MacKay.Data;
using MacKay.PlayerController.Ship;

namespace MacKay.UI
{
    public class UITouchScreenWarpingState : UITouchScreenBaseState
    {
        private CelestailObjectData _planetData;
        private int _planetNumber = 1;
        public UITouchScreenWarpingState(UITouchStateMachine currentContext, UITouchScreenStateFactory 
        shipStateFactory, CelestailObjectData planetData, int planetNumber) : base(currentContext, shipStateFactory)
        {
            IsRootState = true;
            _planetData = planetData;
            _planetNumber = planetNumber;
        }

        public override void EnterState()
        {
        }

        public override void UpdateState() { }

        public override void ExitState()
        {
            AudioVOController.Instance.PlayWelcomeVO(_planetNumber);
        }

        public override void CheckSwitchStates()
        {
            if (Ctx.ShipStateMachine.CurrentState is not ShipWarpState)
            {
                SwitchState(Factory.Orbit(_planetData, _planetNumber));
            }
        }

        public override void InitializeSubState()
        {
        }
    }
}