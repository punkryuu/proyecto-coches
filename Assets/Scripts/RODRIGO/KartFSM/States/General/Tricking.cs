using UnityEngine;

public class Tricking : TemplateStateMachine {
    private FSMManager _fsm;

    public Tricking(FSMManager _stateMachineFlow) : base("Tricking", (StateMachineFlow)_stateMachineFlow)
    {
        _fsm = _stateMachineFlow;
    }
    public override void Enter()
    {
        base.Enter();
        _fsm.stateName.text = name;//animacion de truco
    }
    public override void UpdateLogic()
    {

        base.UpdateLogic();
        if (_fsm.CheckGrounded())
        {
            _fsm.canTrick = false;
            _fsm.currentBoostType = FSMManager.BoostType.Trick;

            stateMachineFlow.ChangeState(((FSMManager)stateMachineFlow).boostingState);
        }
    }


    
    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        _fsm.ApplyGravity();
    }
}

