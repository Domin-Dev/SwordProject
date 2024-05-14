﻿using UnityEngine;
public class AttackState : HeroState
{
    CharacterController controller;
    public AttackState(CharacterController controller, HeroStateMachine heroStateMachine) : base(heroStateMachine)
    {
        this.controller = controller;
    }

    public override void EnterState() 
    {
        if (!controller.handsController.isGun)
        {
            controller.handsController.SetAttackVector(new Vector3(0, 0, 100), new Vector3(0.06f, 0, 0));
        }
        else
        {
            if ((controller.handsController.selectedItem as RangedWeaponItem).HasAmmo())
            {
                controller.handsController.Shot();
                controller.handsController.SetAttackVector(new Vector3(0, 0, -60), new Vector3(-0.08f, 0, 0));
            }
            else
            {
                Sounds.instance.Empty();
                //controller.heroStateMachine.ChangeState(controller.idleState);
                controller.handsController.SetDefaultState();
            }
        }
    }
    public override void ExitState() 
    {

    }
    public override void FrameUpdate() 
    {
        controller.GetMovementInput();
        controller.handsController.updaterAttack.Update();
    }
    public override void FrameFixedUpdate()
    {
        controller.UpdateMovement();
    }
}

