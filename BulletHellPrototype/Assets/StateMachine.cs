using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    public StateMachine owner;
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Think() { }
}

public class StateMachine : MonoBehaviour
{
    bool changingState;


    public State currentState;
    public State globalState;
    public State previousState;

    private IEnumerator coroutine;

    public int updatesPerSecond = 5;

    private void OnEnable()
    {
        StartCoroutine(Think());
    }


    public void ChangeStateDelayed(State newState, float delay)
    {
        coroutine = ChangeStateCoRoutine(newState, delay);
        StartCoroutine(coroutine);
    }

    public void CancelDelayedStateChange()
    {
        if (coroutine != null && changingState)
        {
            StopCoroutine(coroutine);
        }
    }

    IEnumerator ChangeStateCoRoutine(State newState, float delay)
    {
        changingState = true;
        yield return new WaitForSeconds(delay);
        ChangeState(newState);
        changingState = false;
    }

    public void RevertToPreviousState()
    {
        ChangeState(previousState);
    }

    public void ChangeState(State newState)
    {
        previousState = currentState;
        if (currentState != null)
        {
            currentState.Exit();
        }
        currentState = newState;
        currentState.owner = this;
        Debug.Log(currentState.GetType());
        currentState.Enter();
    }

    System.Collections.IEnumerator Think()
    {
        yield return new WaitForSeconds(Random.Range(0, 0.5f));
        while (true)
        {
            if (globalState != null)
            {
                globalState.Think();
            }
            if (currentState != null)
            {
                currentState.Think();
            }

            yield return new WaitForSeconds(1.0f / (float)updatesPerSecond);
        }
    }
}