
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
        if (Input.GetMouseButton(1))
        {
             controller.attackModule.UpdateShield();
        }else
             controller.attackModule.Aim();

        if(Input.GetMouseButtonDown(0) && controller.attackModule.canAttack)
        {
            heroStateMachine.ChangeState(controller.attackState);
        }

        controller.GetMovementInput();
    }

    public override void FrameFixedUpdate()
    {
        controller.UpdateMovement();
    }

}

