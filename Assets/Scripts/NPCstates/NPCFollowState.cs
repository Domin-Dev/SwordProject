
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
        Debug.Log("follow");
        if (controller.isTarget)
        {
            controller.Follow();
            if(controller.GetDistance() < 0.4f)
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

