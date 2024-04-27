
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
    
    }
    public override void ExitState() 
    { 
    
    }
    public override void FrameUpdate()
    {
        controller.attackModule.Aim();

        if(Input.GetMouseButtonDown(0) && controller.attackModule.canAttack)
        {
            controller.itemController.Use();
            heroStateMachine.ChangeState(controller.attackState);
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            controller.attackModule.Reload();
        }

        controller.GetMovementInput();
    }

    public override void FrameFixedUpdate()
    {
        controller.UpdateMovement();
    }

}

