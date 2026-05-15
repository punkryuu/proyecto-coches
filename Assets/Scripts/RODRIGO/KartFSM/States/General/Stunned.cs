using UnityEngine;

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
        _fsm.stateName.text = name;
    }
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        timer += Time.deltaTime;

        if (timer >= _fsm.stunDuration)
        {
            stateMachineFlow.ChangeState(_fsm.idleState);
        }

    }
    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        _fsm.ApplySlowDown();
    }
}
