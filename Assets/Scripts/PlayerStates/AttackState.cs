

using UnityEngine;

public class AttackState : PlayerState
{
    public AttackState(CharacterController controller, PlayerStateMachine playerStateMachine) : base(controller, playerStateMachine)
    {

    }

    public override void EnterState() 
    {
        controller.SetAttackVector(new Vector3(0,0,100),new Vector3(0.06f, 0, 0));
    }
    public override void ExitState() 
    { 
    
    }
    public override void FrameUpdate() 
    {
        controller.UpdateAttack();
    }
}

