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
       // _fsm.stateName.text = name;
    }
    public override void UpdateLogic()
    {
        base.UpdateLogic();
        _fsm.horizontalInput = _fsm.GetInputActions().Driving.Steer.ReadValue<float>();

        // Si está en el aire y puede hacer truco, activa el flag
        if (_fsm.canTrick && _fsm.GetInputActions().Driving.Trick.IsPressed())
        {
            _fsm.isTricking = true;
            _fsm.canTrick = false;               
            _fsm.SetAndPlayAudioClip(4);         
        }

        if (_fsm.CheckGrounded())
        {
            // Si estaba haciendo un truco, aplica boost al aterrizar
            if (_fsm.isTricking)
            {
                _fsm.currentBoostType = FSMManager.BoostType.Trick;
                _fsm.boostDuration = _fsm.trickBoostDuration;  
                _fsm.isTricking = false;
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
        _fsm.ApplyGravity();
        _fsm.ApplySteer();

    }
}
