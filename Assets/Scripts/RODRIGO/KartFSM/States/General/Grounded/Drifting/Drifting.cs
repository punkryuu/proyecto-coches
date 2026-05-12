using UnityEngine;
using static FSMManager;

public class Drifting : General 
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
        _fsm.UpdateDriftLevel();

        _fsm.driftInput = _fsm.GetInputActions().Driving.Drifting.IsPressed();
        _fsm.horizontalInput = _fsm.GetInputActions().Driving.Steer.ReadValue<float>();

        if (!_fsm.driftInput)
        {
            if (_fsm.CanBoost())
            {
                _fsm.currentBoostType = BoostType.Drift;
                stateMachineFlow.ChangeState(((FSMManager)stateMachineFlow).boostingState);
            }
            else
            {

                stateMachineFlow.ChangeState(((FSMManager)stateMachineFlow).idleState);
            }
            
        }
    }
    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        _fsm.ApplyDriftMovement();
        if (!_fsm.CheckGrounded()) _fsm.ApplyGravity();
    }
    public override void Exit()
    {
        base.Exit();
        _fsm.EndDrift();


    }

}
