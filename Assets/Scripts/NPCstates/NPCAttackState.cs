﻿
using UnityEngine;

public class NPCAttackState : HeroState
{
    NPCController controller;

    public NPCAttackState(NPCController controller, HeroStateMachine heroStateMachine) : base(heroStateMachine)
    {
        this.controller = controller;
    }
    public override void EnterState() 
    {
        controller.attackModule.SetAttackVector(new Vector3(0, 0, 100), new Vector3(0.06f, 0, 0),true);
       
    }
    public override void ExitState() 
    { 
    
    }
    public override void FrameUpdate()
    {
       controller.Follow();
       controller.attackModule.UpdateAttack();
    }
    public override void FrameFixedUpdate()
    {
        base.FrameFixedUpdate();
    }


}
