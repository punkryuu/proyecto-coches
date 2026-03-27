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
    //ocurria un jitter raro asi que modifique el lateUpdate por FixedUpdate dado que todas las funciones son cosas de físicas
    private void FixedUpdate()
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
