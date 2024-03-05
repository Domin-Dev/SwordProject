
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
             controller.UpdateShield();
        }else
             controller.Aim();

        if(Input.GetMouseButtonDown(0))
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

