using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grounded : TemplateStateMachine
{
    private FSMManager _fsm;

    public Grounded(string name, FSMManager _stateMachineFlow) : base(name, (StateMachineFlow)_stateMachineFlow)
    {
        _fsm = _stateMachineFlow;
    }
    public override void UpdateLogic()
    {

        base.UpdateLogic();
        /*
        _fsm.horizontalInput = Input.GetAxis("Horizontal");
        if ((Mathf.Abs(_fsm.horizontalInput) > Mathf.Epsilon) || (Mathf.Abs(_fsm.verticalInput) > Mathf.Epsilon))
        {
            stateMachineFlow.ChangeState(((FSMManager)stateMachineFlow).movingState);
        }
        if (Mathf.Abs(_fsm.horizontalInput) < Mathf.Epsilon && Mathf.Abs(_fsm.verticalInput) < Mathf.Epsilon)
        {
            stateMachineFlow.ChangeState(((FSMManager)stateMachineFlow).idleState);
        }
        */


    }
}
