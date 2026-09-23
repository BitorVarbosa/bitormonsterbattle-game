using System.Collections.Generic;
using UnityEngine;

namespace BitorTools.FSM
{
    [CreateAssetMenu(menuName = "FSM/State")]
    public sealed class State : BaseState
    {
        public List<StateAction> Action = new List<StateAction>();
        public List<StateTransition> Transitions = new List<StateTransition>();

        public override void Execute(BaseStateMachine machine)
        {
            foreach (var action in Action)
                action.Execute(machine);

            foreach (var transition in Transitions)
                transition.Execute(machine);
        }
    }
}
