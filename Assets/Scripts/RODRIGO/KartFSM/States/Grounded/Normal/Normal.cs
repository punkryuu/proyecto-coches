using UnityEngine;

public class Normal : Grounded
{
    private FSMManager _fsm;

    public Normal(string name, FSMManager _stateMachineFlow) : base(name, _stateMachineFlow)
    {
        _fsm = _stateMachineFlow;
    }
}
