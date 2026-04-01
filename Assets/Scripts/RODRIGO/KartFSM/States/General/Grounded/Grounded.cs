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
    private float groundTimer = 0f;

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        if (!_fsm.CheckGrounded())
        {
            groundTimer += Time.deltaTime;

            if (groundTimer > 0.1f)
            {
                stateMachineFlow.ChangeState(((FSMManager)stateMachineFlow).fallingState);
            }
        }
        else
        {
            groundTimer = 0f;
        }
    }

}
