using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

        _fsm.accelerateInput = _fsm.GetInputActions().Driving.Accelerate.IsPressed();
        _fsm.brakeInput = _fsm.GetInputActions().Driving.Stop.IsPressed();
        _fsm.horizontalInput = _fsm.GetInputActions().Driving.Steer.ReadValue<float>();
        if (_fsm.accelerateInput)
        {
            stateMachineFlow.ChangeState(((FSMManager)stateMachineFlow).acceleratingState);
        }
        else if (_fsm.brakeInput)
        {
            stateMachineFlow.ChangeState(((FSMManager)stateMachineFlow).brakingState);
        }
    }
    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        _fsm.ApplySlowDown();
    }

}
