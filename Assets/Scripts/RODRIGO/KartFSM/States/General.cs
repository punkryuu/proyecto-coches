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
        _fsm.RotateHitbox();
    }
}
