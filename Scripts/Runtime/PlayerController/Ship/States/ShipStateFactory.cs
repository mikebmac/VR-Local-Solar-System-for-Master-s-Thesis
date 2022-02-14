using SpaceGraphicsToolkit;

namespace MacKay.PlayerController.Ship
{
    public class ShipStateFactory
    {
        private ShipStateMachine _context;

        public ShipStateFactory(ShipStateMachine currentContext)
        {
            _context = currentContext;
        }

        public ShipBaseState Drive()
        {
            return new ShipDriveState(_context, this);
        }
        
        public ShipBaseState Autopilot(SgtFloatingPoint orbitPoint)
        {
            return new ShipAutopilotState(_context, this, orbitPoint);
        }
        
        public ShipBaseState Warp(SgtFloatingTarget target, double distance)
        {
            return new ShipWarpState(_context, this, target, distance);
        }

        public ShipBaseState WarpStart(SpaceGraphicsToolkit.SgtFloatingTarget target)
        {
            return new ShipEnterWarpSubState(_context, this, target);
        }
        
        public ShipBaseState WarpTravel(SpaceGraphicsToolkit.SgtFloatingTarget target, float warpExitTime)
        {
            return new ShipTravelWarpSubState(_context, this, target, warpExitTime);
        }
        
        public ShipBaseState WarpExit()
        {
            return new ShipExitWarpSubState(_context, this);
        }
    }
}