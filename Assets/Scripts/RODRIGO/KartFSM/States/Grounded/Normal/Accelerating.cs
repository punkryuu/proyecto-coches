using UnityEngine;

public class Accelerating : Normal 
{
    private FSMManager _fsm;

    public Accelerating(FSMManager _stateMachineFlow) : base("Accelerating", _stateMachineFlow)
    {
        _fsm = _stateMachineFlow;
    }
    public override void Enter()
    {
        base.Enter();
        _fsm.stateName.text = name;
        _fsm.fordwardSpeed = _fsm.acceleration;


    }
    public override void UpdateLogic()
    {

        base.UpdateLogic();
        _fsm.accelerateInput = _fsm.GetInputActions().Driving.Accelerate.IsPressed();
        _fsm.brakeInput = _fsm.GetInputActions().Driving.Stop.IsPressed();
        _fsm.steerInput = _fsm.GetInputActions().Driving.Steer.ReadValue<float>();
        if (_fsm.brakeInput)
        {
            stateMachineFlow.ChangeState(((FSMManager)stateMachineFlow).brakingState);
        }
        else if (!_fsm.brakeInput && ! _fsm.accelerateInput)
        {
            stateMachineFlow.ChangeState(((FSMManager)stateMachineFlow).idleState);
        }
        /*
        if (!_fsm.CheckGrounded())
        {
            stateMachineFlow.ChangeState(((FSMManager)stateMachineFlow).fallingState);
        }
        */
    }
    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        _fsm.FordwardMovement();
    }
}
