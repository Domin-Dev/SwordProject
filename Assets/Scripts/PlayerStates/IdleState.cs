
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
        controller.attackModule.Aim();
        

        if(Input.GetMouseButtonDown(0) && controller.attackModule.canAttack)
        {
            if (!controller.attackModule.isGun)
            {
                heroStateMachine.ChangeState(controller.attackState);
            }
            else
            {
                heroStateMachine.ChangeState(controller.attackState);
            }
        }

        controller.GetMovementInput();
    }

    public override void FrameFixedUpdate()
    {
        controller.UpdateMovement();
    }

}

