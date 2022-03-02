using MacKay.Data;
using MacKay.PlayerController.Ship;
using MacKay.UI;

public class UITouchScreenPlanetState : UITouchScreenBaseState
{
    private CelestailObjectData _currentPlanet;
    private int _planetNumber;
    public UITouchScreenPlanetState(UITouchStateMachine currentContext, UITouchScreenStateFactory shipStateFactory, 
    CelestailObjectData planet, int planetNumber) : base(currentContext, shipStateFactory)
    {
        IsRootState = true;
        _currentPlanet = planet;
        _planetNumber = planetNumber;
    }

    public override void EnterState()
    {
        UITouchButton_ActionHandler.Instance.OnSplashPlanetBack += SwitchToWarpSplash;
        UITouchButton_ActionHandler.Instance.OnSplashWarpRequest += WarpRequest;
        string text = $"<b>Planet:</b> {_currentPlanet.objectName}\n\n" +
                      $"<b>Natural Satellites</b> {_currentPlanet.satellites.Length}\n" +
                      $"<b>Radius:</b> {_currentPlanet.diameter / 2}\n" +
                      $"<b>Mass:</b> {_currentPlanet.mass}\n" +
                      $"<b>Density:</b> {_currentPlanet.density}\n" +
                      $"<b>Rotation Period:</b> {_currentPlanet.rotationPeriod} Earth Days\n" +
                      $"<b>Orbit Period:</b> {_currentPlanet.orbitPeriod} Earth Years\n";
        Ctx.PlanetInformation.text = text;
        Ctx.PlanetMeshRenderer.material = Ctx.PlanetMaterials[_planetNumber];
        
        Ctx.PlanetManager.startAppear();
    }

    public override void UpdateState()
    {
    }

    public override void ExitState()
    {
        UITouchButton_ActionHandler.Instance.OnSplashPlanetBack -= SwitchToWarpSplash;
        UITouchButton_ActionHandler.Instance.OnSplashWarpRequest -= WarpRequest;
        
        Ctx.PlanetManager.startDisappear();
    }

    public override void CheckSwitchStates()
    {
    }

    public override void InitializeSubState()
    {
    }

    private void SwitchToWarpSplash()
    {
        SwitchState(Factory.Warp());
    }
    
    private void WarpRequest()
    {
        ShipActionHandler.Instance.WarpTo(_currentPlanet.warpPoint.target);
        
        SwitchState(Factory.Warping(_currentPlanet, _planetNumber));
    }
    
}
