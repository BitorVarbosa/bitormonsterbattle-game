using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BitorMonsterBattle.Core
{
    public class BattleAction
    {
        private List<BattleCharacter> _targets;

        public BattleCharacter Actor { get; private set; }
        public MoveData Move { get; private set; }
        public IReadOnlyList<BattleCharacter> GetTargets() => _targets.AsReadOnly();

        public BattleAction(BattleCharacter actor, MoveData move, List<BattleCharacter> targets)
        {
            this.Actor = actor;
            this.Move = move;
            this._targets = targets;
        }

        public bool CanExecute()
        {
            if (!Actor.IsAlive || !Actor.CanUseMove(Move))
                return false;

            return Move.GetMoveEffects().All(effect => effect.CanExecute(Actor, _targets));
        }

        public void Execute(BattleController battleController)
        {
            if (!CanExecute()) return;

            Actor.ConsumeEnergy(Move.EnergyCost);

            foreach (var effect in Move.GetMoveEffects())
            {
                effect.Execute(Actor, _targets, battleController);
            }
        }
    }
}
