using UnityEngine;
using System.Collections.Generic;

public class Stunned : General
{
    private FSMManager _fsm;
    private float timer;

    public Stunned(FSMManager _stateMachineFlow) : base("Stunned", _stateMachineFlow)
    {
        _fsm = _stateMachineFlow;
    }
    public override void Enter()
    {
        base.Enter();

        timer = 0f;

        _fsm.stateName.text = name;
        _fsm.StartStun();
        _fsm.SetStars(true);
    }
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        timer += Time.deltaTime;
        _fsm.StayStunned();

        if (timer >= _fsm.stunDuration)
        {
            stateMachineFlow.ChangeState(_fsm.idleState);
        }

    }
    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        _fsm.ApplySlowDown();
        if (!_fsm.CheckGrounded()) _fsm.ApplyGravity();
    }
    public override void Exit()
    {
        base.Exit();
        _fsm.SetStars(false);

        _fsm.StartRestoreRotation();



    }
}
