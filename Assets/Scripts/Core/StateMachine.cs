using System.Collections.Generic;
using UnityEngine;

public delegate void Callable();

public class State
{
    public Callable update;
    public Callable enter;
    public Callable exit;

    public State(Callable update, Callable enter, Callable exit)
    {
        this.update = update;
        this.enter = enter;
        this.exit = exit;
    }
}

public class StateMachine
{
    private Dictionary<string, State> states = new Dictionary<string, State>();
    public string currentState;

    public void AddState(Callable update, Callable enter, Callable exit)
    {
        State newState = new State(update, enter, exit);
        states.Add(update.Method.Name, newState);
    }

    public void InitialState(Callable state)
    {
        ChangeState(state);
    }

    public void ChangeState(Callable state)
    {
        string stateName = state.Method.Name;
        if (states.ContainsKey(stateName))
        {
            SetState(stateName);
        }
    }

    public void SetState(string stateName)
    {
        if (currentState != null && states[currentState].exit != null) states[currentState].exit();
        //Debug.Log($"{stateName}");
        currentState = stateName;
        if (currentState != null && states[currentState].enter != null) states[currentState].enter();
    }

    public void Update()
    {
        if(currentState != null && states[currentState].update != null) states[currentState].update();
    }
}
