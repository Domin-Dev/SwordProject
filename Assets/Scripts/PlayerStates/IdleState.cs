
using UnityEngine;

public class IdleState : HeroState
{
    private CharacterController controller;
    public IdleState(CharacterController controller, HeroStateMachine heroStateMachine) : base(heroStateMachine)
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
        controller.GetMovementInput();
        controller.handsController.Aim();
        controller.UpdateFlip();
        controller.UpdateCharacterSprites();


        if(Input.GetMouseButtonDown(0) && controller.handsController.canAttack)
        {
            controller.handsController.Use();
            heroStateMachine.ChangeState(controller.attackState);
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            heroStateMachine.ChangeState(controller.reloadingState);
        }
    }

    public override void FrameFixedUpdate()
    {
        controller.UpdateMovement();
    }

}

