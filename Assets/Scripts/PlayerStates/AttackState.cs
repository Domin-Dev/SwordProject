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
        if (Input.GetMouseButton(1))
        {
            controller.attackModule.SetAttackVector(new Vector3(0, 0, 10), new Vector3(-0.06f, 0, 0), false);
        }
        else
        {
            controller.attackModule.SetAttackVector(new Vector3(0, 0, 100), new Vector3(0.06f, 0, 0), true);
        } 
    }
    public override void ExitState() 
    {

    }
    public override void FrameUpdate() 
    {
        controller.attackModule.UpdateAttack();
        controller.GetMovementInput();
    }

    public override void FrameFixedUpdate()
    {
        controller.UpdateMovement();
    }
}

