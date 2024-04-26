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
        if (!controller.attackModule.isGun)
        {
            controller.attackModule.SetAttackVector(new Vector3(0, 0, 100), new Vector3(0.06f, 0, 0), true);
        }
        else
        {
            if ((controller.selectedItem as RangedWeaponItem).HasAmmo())
            {
                controller.attackModule.Shot();
                controller.Shot();
                controller.attackModule.SetAttackVector(new Vector3(0, 0, -60), new Vector3(-0.08f, 0, 0), true);
            }
            else
            {
                Sounds.instance.Empty();
                controller.heroStateMachine.ChangeState(controller.idleState);
            }
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

