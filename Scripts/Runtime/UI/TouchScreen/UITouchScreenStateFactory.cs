using MacKay.Data;

namespace MacKay.UI
{
    public class UITouchScreenStateFactory
    {
        private UITouchStateMachine _context;

        public UITouchScreenStateFactory(UITouchStateMachine currentContext)
        {
            _context = currentContext;
        }
        
        public UITouchScreenBaseState Tutorial()
        {
            return new UITouchScreenTutorialState(_context, this);
        }
        
        public UITouchScreenBaseState Warp()
        {
            return new UITouchScreenWarpState(_context, this);
        }
        
        public UITouchScreenBaseState Planet(CelestailObjectData planet, int planetNumber)
        {
            return new UITouchScreenPlanetState(_context, this, planet, planetNumber);
        }
        
        public UITouchScreenBaseState Orbit(CelestailObjectData planet, int planetNumber)
        {
            return new UITouchScreenOrbitState(_context, this, planet, planetNumber);
        }
    }
}