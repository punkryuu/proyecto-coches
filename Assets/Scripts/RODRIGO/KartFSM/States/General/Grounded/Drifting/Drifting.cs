using UnityEngine;

public class Drifting : Grounded 
    {
    private FSMManager _fsm;

    public Drifting(FSMManager _stateMachineFlow) : base("Drifting", _stateMachineFlow)
    {
        _fsm = _stateMachineFlow;
    }
    public override void Enter()
    {
        base.Enter();
        _fsm.stateName.text = name;
        _fsm.StartDrift();
    }
    public override void UpdateLogic()
    {

        base.UpdateLogic();
        _fsm.driftInput = _fsm.GetInputActions().Driving.Drifting.IsPressed();

        if (!_fsm.driftInput)
        {
            stateMachineFlow.ChangeState(((FSMManager)stateMachineFlow).idleState);
        }
        _fsm.horizontalInput = _fsm.GetInputActions().Driving.Steer.ReadValue<float>();
    }
    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        _fsm.ApplyDriftMovement();
    }
    public override void Exit()
    {
        base.Exit();
        _fsm.EndDrift();
    }

}
