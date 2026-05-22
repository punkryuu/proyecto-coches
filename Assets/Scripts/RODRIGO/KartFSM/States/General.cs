using UnityEngine;

public class General : TemplateStateMachine {
    private FSMManager _fsm;

    public General(string name, FSMManager _stateMachineFlow) : base(name, (StateMachineFlow)_stateMachineFlow)
    {
        _fsm = _stateMachineFlow;
    }
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        if (_fsm.stunned && _fsm.canBeStunned)
        {
            _fsm.stunned = false;

            // usar la duración enviada por el trigger
            _fsm.stunDuration = _fsm.triggerStunDuration;

            stateMachineFlow.ChangeState(_fsm.stunnedState);
            return;
        }
        _fsm.powerInput = _fsm.GetInputActions().Driving.Power.IsPressed();
        if (_fsm.powerInput) 
        {
            _fsm.UsePower();
        }
    }
    public override void UpdatePhysics()
    {
        if (!_fsm.driftFlag) _fsm.RotateHitbox();
        else _fsm.RotateHitboxDrift();
        _fsm.RotateX();
        _fsm.RotateY();
    }
}
