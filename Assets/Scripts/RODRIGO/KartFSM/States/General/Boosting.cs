using UnityEngine;
using static FSMManager;

public class Boosting : General 
{
    private FSMManager _fsm;
    private float timer;

    public Boosting(FSMManager _stateMachineFlow) : base("Boosting", _stateMachineFlow)
    {
    _fsm = _stateMachineFlow;
}
public override void Enter()
{
        base.Enter();
        _fsm.stateName.text = name;

        timer = 0f; 

        switch (_fsm.currentBoostType)
        {
            case BoostType.Drift:
                Debug.Log("Boost de drift");
                break;
        }
    }
    public override void UpdateLogic()
    {
        base.UpdateLogic();

  

        timer += Time.deltaTime;

        if (timer >= _fsm.boostDuration)
        {
            stateMachineFlow.ChangeState(_fsm.idleState);
        }
    }
    public override void UpdatePhysics()
{
    base.UpdatePhysics();
    if (!_fsm.CheckGrounded()) _fsm.ApplyGravity();
    _fsm.ApplyBoostSpeed();
 
}
}