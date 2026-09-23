using UnityEngine;

namespace BitorTools.FSM
{
    public class BaseState : ScriptableObject
    {
        public virtual void Execute(BaseStateMachine machine) { }

        public virtual void EnterState(BaseStateMachine machine) { }
        
        public virtual void ExitState(BaseStateMachine machine){ }
    }
}
