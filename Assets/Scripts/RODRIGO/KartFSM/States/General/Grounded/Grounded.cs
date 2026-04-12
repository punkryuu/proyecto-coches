using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grounded : General
{
    private FSMManager _fsm;

    public Grounded(string name, FSMManager _stateMachineFlow) : base(name, _stateMachineFlow)
    {
        _fsm = _stateMachineFlow;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        _fsm.trickInput = _fsm.GetInputActions().Driving.Trick.IsPressed();
        if (!_fsm.CheckGrounded())
        {
            if (_fsm.canTrick&&_fsm.trickInput)
            {
                stateMachineFlow.ChangeState(((FSMManager)stateMachineFlow).trickingState);
            }
            else { stateMachineFlow.ChangeState(((FSMManager)stateMachineFlow).fallingState); }
        }
    }

}
