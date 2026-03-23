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
    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        if (!_fsm.CheckGrounded())
        {
            stateMachineFlow.ChangeState(((FSMManager)stateMachineFlow).fallingState);
        }
    }
}
