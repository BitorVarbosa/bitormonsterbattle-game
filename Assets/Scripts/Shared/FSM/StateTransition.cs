using UnityEngine;

namespace BitorTools.FSM
{
    [CreateAssetMenu(menuName = "FSM/Transition")]
    public sealed class StateTransition : ScriptableObject
    {
        [SerializeField] private Decision _decision;
        [SerializeField] private BaseState _trueState;
        [SerializeField] private BaseState _falseState;

        public void Execute(BaseStateMachine stateMachine)
        {
            if (_decision.Decide(stateMachine) && !(_trueState is RemainInState))
                stateMachine.ChangeState(_trueState);
            else if (!(_falseState is RemainInState))
                stateMachine.ChangeState(_falseState);
        }
    }
}
