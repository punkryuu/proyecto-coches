using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : Normal
{
    private FSMManager _fsm;
    
    public Idle(FSMManager _stateMachineFlow) : base("Idle",_stateMachineFlow)
    {
        _fsm = _stateMachineFlow;
    }
    public override void Enter()
    {
        base.Enter();
        _fsm.stateName.text = name;

    }
    public override void UpdateLogic()
    {

        base.UpdateLogic();
        if (_fsm.accelerateInput)
        {
            Debug.Log("Idle → accelerateInput true, cambiando a acceleratingState");
            stateMachineFlow.ChangeState(((FSMManager)stateMachineFlow).acceleratingState);
        }
        else if (_fsm.brakeInput)
        {
            Debug.Log("Idle → brakeInput true, cambiando a brakingState");
            stateMachineFlow.ChangeState(((FSMManager)stateMachineFlow).brakingState);
        }

    }
   
}
