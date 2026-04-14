using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemplateStateMachine
{
    // Start is called before the first frame update
    public string name;
    protected StateMachineFlow stateMachineFlow;
    public TemplateStateMachine (string _name, StateMachineFlow _stateMachineFlow) 
    {
        this.name =_name;
        this.stateMachineFlow = _stateMachineFlow;
    }

   public virtual void Enter() { }
   public virtual void UpdateLogic() { }
   public virtual void UpdatePhysics() { }
   public virtual void Exit() { }



}
