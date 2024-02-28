

using UnityEngine.UIElements;

public class IdleState : PlayerState
{
    public IdleState(CharacterController controller, PlayerStateMachine playerStateMachine) : base(controller, playerStateMachine)
    {

    }

    public override void EnterState() 
    { 
    
    }
    public override void ExitState() 
    { 
    
    }
    public override void FrameUpdate() 
    {
        controller.Aim();
    }
}

