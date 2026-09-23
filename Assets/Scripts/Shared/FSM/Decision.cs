using UnityEngine;

namespace BitorTools.FSM
{
    public abstract class Decision : ScriptableObject
    {
        public abstract bool Decide(BaseStateMachine state);
    }
}
