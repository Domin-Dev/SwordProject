using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroStateMachine
{
    public HeroState currentState;
    public void RunMachine(HeroState startState)
    {
        currentState = startState;
        currentState.EnterState();
    }

    public void ChangeState(HeroState newState)
    {
        currentState.ExitState();
        currentState = newState;
        currentState.EnterState();
    }

}
