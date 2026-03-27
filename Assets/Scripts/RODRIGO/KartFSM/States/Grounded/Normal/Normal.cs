using UnityEngine;

public class Normal : Grounded
{
    private FSMManager _fsm;

    public Normal(string name, FSMManager _stateMachineFlow) : base(name, _stateMachineFlow)
    {
        _fsm = _stateMachineFlow;
    }
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        _fsm.steerInput = _fsm.GetInputActions().Driving.Steer.ReadValue<float>();
    }
    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        _fsm.ApplySteer();
        _fsm.ApplyLateralFriction();
    }
}
