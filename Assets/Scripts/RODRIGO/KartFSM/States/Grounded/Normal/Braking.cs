using UnityEngine;

public class Braking : Normal 
{
    private FSMManager _fsm;

    public Braking(FSMManager _stateMachineFlow) : base("Braking", _stateMachineFlow)
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
        if (_fsm.accelerateInput)
        {
            stateMachineFlow.ChangeState(((FSMManager)stateMachineFlow).acceleratingState);
        }
        else if (!_fsm.brakeInput && !_fsm.accelerateInput)
        {
            stateMachineFlow.ChangeState(((FSMManager)stateMachineFlow).idleState);
        }
    }
    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        _fsm.ApplyBrake(_fsm.brakePower);
    }
}
