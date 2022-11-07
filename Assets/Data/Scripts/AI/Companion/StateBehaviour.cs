using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBehaviour<StateType> : MonoBehaviour where StateType : System.Enum
{
    [field: SerializeField]
    public StateType State
    {
        get; protected set;
    }

    public delegate void StateChangeEvent(StateType OldState, StateType NewState);
    public StateChangeEvent OnStateChange;

    public virtual void ChangeState(StateType NewState)
    {
        OnStateChange?.Invoke(State, NewState);
        State = NewState;
    }
}
