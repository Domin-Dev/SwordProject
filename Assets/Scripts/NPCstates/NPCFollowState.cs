
using UnityEngine;

public class NPCFollowState : HeroState
{
    NPCController controller;

    public NPCFollowState(NPCController controller, HeroStateMachine heroStateMachine) : base(heroStateMachine)
    {
        this.controller = controller;
    }
    public override void EnterState() 
    { 

    }
    public override void ExitState() 
    {
        controller.StopFollow();
    }
    public override void FrameUpdate()
    {

        if (controller.isTarget)
        {
            controller.Aim();
            controller.Follow();
            if(controller.GetDistance() < 0.4f && controller.canAttack)
            {
                controller.canAttack = false;
                heroStateMachine.ChangeState(controller.attackState);              
            }

            if (Input.GetMouseButtonDown(0))
            {
                heroStateMachine.ChangeState(controller.attackState);
            }
        }
        else
        {
            heroStateMachine.ChangeState(controller.idleState);
        }

    }
    public override void FrameFixedUpdate()
    {
        base.FrameFixedUpdate();
    }

}

