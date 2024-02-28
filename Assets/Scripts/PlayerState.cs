
using UnityEngine;
public abstract class PlayerState    
{
    protected CharacterController controller;
    protected PlayerStateMachine playerStateMachine;

    public PlayerState(CharacterController controller, PlayerStateMachine playerStateMachine)
    {
        this.controller = controller;
        this.playerStateMachine = playerStateMachine;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void FrameUpdate() { }

}
