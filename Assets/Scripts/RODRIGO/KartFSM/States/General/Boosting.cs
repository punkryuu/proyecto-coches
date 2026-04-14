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
        timer = 0f;

        switch (_fsm.currentBoostType)
        {
            case BoostType.Drift:
                _fsm.boostDuration = _fsm.GetBoostDurationAfterDrift();
                Debug.Log("Boost de drift - Duración: " + _fsm.boostDuration);
                break;
            case BoostType.Trick:
                _fsm.boostDuration = _fsm.trickBoostDuration;
                Debug.Log("Boost de truco - Duración: " + _fsm.boostDuration);
                break;
            default:
                _fsm.boostDuration = 0.5f; 
                Debug.LogWarning("desconocido, usando duración por defecto.");
                break;
        }
        _fsm.PlayTurboParticles();    
    
    }
    public override void UpdateLogic()
    {
        base.UpdateLogic();

  

        timer += Time.deltaTime;

        if (timer >= _fsm.boostDuration)
        {
            _fsm.StopTurboParticles();
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