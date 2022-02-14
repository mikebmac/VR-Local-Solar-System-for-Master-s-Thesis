using MacKay.Data;
using MacKay.UI;

public class UITouchScreenWarpState : UITouchScreenBaseState
{
    public UITouchScreenWarpState(UITouchStateMachine currentContext, UITouchScreenStateFactory shipStateFactory) : base(currentContext, shipStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        UITouchButton_ActionHandler.Instance.OnSplashWarpToSplashPlanet += SwitchToPlanetSplash;
        UITouchButton_ActionHandler.Instance.OnSplashWarpToSplashOrbit += SwitchToOrbitSplash;

        Ctx.WarpManager.startAppear();
    }

    public override void UpdateState()
    {
    }

    public override void ExitState()
    {
        UITouchButton_ActionHandler.Instance.OnSplashWarpToSplashPlanet -= SwitchToPlanetSplash;
        UITouchButton_ActionHandler.Instance.OnSplashWarpToSplashOrbit -= SwitchToOrbitSplash;
        
        Ctx.WarpManager.startDisappear();
    }

    public override void CheckSwitchStates()
    {
    }

    public override void InitializeSubState()
    {
    }
    
    private void SwitchToPlanetSplash(CelestailObjectData planet, int planetNumber)
    {
        SwitchState(Factory.Planet(planet, planetNumber));
    }

    private void SwitchToOrbitSplash()
    {
        SwitchState(Factory.Orbit(GameManager.Instance.Planets[Ctx.OrbitingPlanetNumber], Ctx.OrbitingPlanetNumber));
    }
}
