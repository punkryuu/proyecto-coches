using UnityEngine;

public class General: TemplateStateMachine 
    {
    private FSMManager _fsm;

    public General(string name, FSMManager _stateMachineFlow) : base(name, (StateMachineFlow)_stateMachineFlow)
    {
        _fsm = _stateMachineFlow;
    }
    public override void UpdatePhysics()
    {
        if (!_fsm.driftFlag) _fsm.RotateHitbox();
        else _fsm.RotateHitboxDrift();
        _fsm.RotateWheelParentsInX();
        _fsm.SteerFrontWheels();
    }
}
