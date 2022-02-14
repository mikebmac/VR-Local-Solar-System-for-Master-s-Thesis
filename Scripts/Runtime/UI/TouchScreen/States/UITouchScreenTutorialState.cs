using System.Collections;
using System.Collections.Generic;
using MacKay.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class UITouchScreenTutorialState : UITouchScreenBaseState
{
    public UITouchScreenTutorialState(UITouchStateMachine currentContext, UITouchScreenStateFactory shipStateFactory) : base(currentContext, shipStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Ctx.StartCoroutine(Cor_WaitForInit());
    }

    public override void UpdateState()
    {
        if (Keyboard.current.backspaceKey.wasReleasedThisFrame) SwitchState(Factory.Warp());
    }

    public override void ExitState()
    {
        Ctx.TutorialManager.startDisappear();
    }

    public override void CheckSwitchStates()
    {
    }

    public override void InitializeSubState()
    {
    }

    // TODO: Add a check to see if ready
    private IEnumerator Cor_WaitForInit()
    {
        yield return new WaitForSeconds(1f);
        
        Ctx.TutorialManager.startAppear();
    }
}
