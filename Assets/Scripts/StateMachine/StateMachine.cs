using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A stack based finite state machine.
/// </summary>
public class StateMachine {
    private List<IState> _stack;

    public StateMachine() {
        _stack = new List<IState>();
    }

    public void Update() {
        IState currState = GetCurrentState();
        if (currState != null) {
            currState.Update();
        }
    }

    public void FixedUpdate() {
        IState currState = GetCurrentState();
        if (currState != null) {
            currState.FixedUpdate();
        }
    }

    public void PopState() {
        IState toRemoveState = _stack[_stack.Count - 1];
        toRemoveState.Exit();
        _stack.RemoveAt(_stack.Count - 1);

        IState newState = GetCurrentState();
        if (newState != null) {
            newState.Enter();
        }
    }

    public void PushState(IState state) {
        IState currState = GetCurrentState();
        if (currState != null &&
            (currState.GetType().Equals(state.GetType()))) {
            Debug.LogError("Adjacent duplicate states not allowed.");
            return;
        }

        _stack.Add(state);
        if (_stack.Count == 1) {
            // State was first one in the stack,
            // run its enter method.
            GetCurrentState().Enter();
        }
    }

    public IState GetCurrentState() {
        return _stack.Count > 0 ? _stack[_stack.Count - 1] : null;
    }

    public void PrintStack() {
        Debug.Log("--STACK--");
        for (int i = _stack.Count - 1; i >= 0; i--) {
            Debug.Log(_stack[i].GetType().Name);
        }
    }

    public void PrintCurrentState() {
        IState currState = GetCurrentState();
        if (currState != null) {
            Debug.Log("STATE: " + currState.GetType().Name);
        }
    }
}