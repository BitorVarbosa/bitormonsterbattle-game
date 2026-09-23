using UnityEngine;

namespace BitorTools.FSM
{
    public abstract class StateAction : ScriptableObject
    {
        public abstract void Execute(BaseStateMachine stateMachine);
    }
}
