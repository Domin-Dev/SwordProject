
using System;
using UnityEngine;

public class BuildingState : HeroState
{
    private CharacterController controller;

    public BuildingState(CharacterController controller, HeroStateMachine heroStateMachine) : base(heroStateMachine)
    {
        this.controller = controller;
    }
    public override void EnterState() 
    {
        BuildingManager.instance.StartBuildingMode(controller.handsController.selectedItem.itemID);
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
        Debug.Log("s");
    }

    public override void FrameFixedUpdate()
    {
        controller.UpdateMovement();
    }

}

