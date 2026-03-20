using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StateMachineFlow : MonoBehaviour
{
    TemplateStateMachine currentState;
    private void Start()
    {
        GetinitialState(out currentState);   
        if (currentState != null) 
        {
            currentState.Enter();
        }
    }
    private void Update()
    {
        if (currentState != null)
        {
            currentState.UpdateLogic();

        }
    }
    private void LateUpdate()
    {
        if (currentState != null)
        {
            currentState.UpdatePhysics();

        }
    }
    protected virtual void GetinitialState(out TemplateStateMachine _stateMachine) 
    { 
        _stateMachine = null;
    }
    public void ChangeState(TemplateStateMachine _newState) 
    {
        if (currentState != null) 
        {
            Debug.Log($"Cambiando de {currentState.name} a {_newState.name}");
            currentState.Exit();
            currentState=_newState;
            currentState.Enter();
        }
    }
    public TMP_Text stateName;

}
