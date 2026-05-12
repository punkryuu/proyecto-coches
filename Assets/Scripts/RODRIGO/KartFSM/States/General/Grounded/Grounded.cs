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
        if (_fsm.triggerBoost)
        {
            _fsm.triggerBoost = false;

            _fsm.currentBoostType = FSMManager.BoostType.Trigger;

            // usar la duración enviada por el trigger
            _fsm.boostDuration = _fsm.triggerBoostDuration;

            stateMachineFlow.ChangeState(_fsm.boostingState);
            return;
        }
        if (!_fsm.CheckGrounded())
        {
            stateMachineFlow.ChangeState(((FSMManager)stateMachineFlow).fallingState);         }
    }

}
