using UnityEngine;

public class Falling: TemplateStateMachine {
    private FSMManager _fsm;

    public Falling( FSMManager _stateMachineFlow) : base("Falling", (StateMachineFlow)_stateMachineFlow)
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
        if (_fsm.CheckGrounded())
        {
            stateMachineFlow.ChangeState(((FSMManager)stateMachineFlow).idleState);
        }


    }
    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        _fsm.Gravity();
    }
}
