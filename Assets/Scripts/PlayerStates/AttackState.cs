using UnityEngine;
public class AttackState : HeroState
{
    CharacterController controller;
    public AttackState(CharacterController controller, HeroStateMachine heroStateMachine) : base(heroStateMachine)
    {
        this.controller = controller;
    }

    public override void EnterState() 
    {
        controller.SetAttackVector(new Vector3(0,0,100),new Vector3(0.06f, 0, 0));
    }
    public override void ExitState() 
    {

        controller.GetMovementInput();
    }
    public override void FrameUpdate() 
    {
        controller.UpdateAttack();
        controller.UpdateMovement();
    }
}

