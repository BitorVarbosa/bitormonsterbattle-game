using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BitorMonsterBattle.Core
{
    public class BattleAction
    {
        public BattleCharacter Actor { get; private set; }
        public MoveData Move { get; private set; }
        public List<BattleCharacter> Targets;

        public BattleAction(BattleCharacter actor, MoveData move, List<BattleCharacter> targets)
        {
            this.Actor = actor;
            this.Move = move;
            this.Targets = targets;
        }

        public bool CanExecute()
        {
            if (!Actor.IsAlive || !Actor.CanUseMove(Move))
                return false;

            return Move.Effects.All(effect => effect.CanExecute(Actor, Targets));
        }

        public void Execute(BattleController battleController)
        {
            if (!CanExecute()) return;

            Actor.ConsumeEnergy(Move.EnergyCost);

            foreach (var effect in Move.Effects)
            {
                effect.Execute(Actor, Targets, battleController);
            }
        }
    }
}
