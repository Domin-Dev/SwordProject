
using UnityEngine;

public class ReloadingState : HeroState
{
    private CharacterController controller;
    public ReloadingState(CharacterController controller, HeroStateMachine heroStateMachine) : base(heroStateMachine)
    {
        this.controller = controller;
    }
    public override void EnterState() 
    {
        controller.handsController.Reload();
        isReady = false;
    }
    public override void ExitState() 
    { 
    
    }
    bool isReady;
    public override void FrameUpdate()
    {
        if (!isReady)
        {
            isReady = controller.handsController.UpdateRotation();
            if(isReady)  controller.handsController.SetGunShells();
        }
        else
        {
            controller.handsController.updaterReload.Update();
        }

    //    controller.handsController.Aim();
        controller.GetMovementInput();

        if (Input.GetMouseButtonDown(0))
        {
            controller.heroStateMachine.ChangeState(controller.idleState);
        }
    }

    public override void FrameFixedUpdate()
    {
        controller.UpdateMovement();
    }

}

