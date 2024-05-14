
using System;
using UnityEngine;

public class BuildingState : HeroState
{
    private CharacterController controller;


    bool isHit = true;
    public BuildingState(CharacterController controller, HeroStateMachine heroStateMachine) : base(heroStateMachine)
    {
        this.controller = controller;
        BuildingManager.instance.hammerBlow += (object sender, EventArgs e) => 
        {
            controller.handsController.SetAttackVector(new Vector3(0, 0, 100), new Vector3(0.06f, 0, 0));
            isHit = true;
        };
    }


    public override void EnterState() 
    {
        BuildingManager.instance.StartBuildingMode();
    }
    public override void ExitState() 
    {
        BuildingManager.instance.EndBuildingMode();
    }
    public override void FrameUpdate()
    {
        controller.GetMovementInput();
        controller.handsController.Aim();
        controller.UpdateFlip();
        controller.UpdateCharacterSprites();

        if(isHit)
        {
            controller.handsController.updaterAttack.Update();
        }

    }

    public override void FrameFixedUpdate()
    {
        controller.UpdateMovement();
    }

}

