using System.Collections.Generic;
using UnityEngine;

namespace BitorMonsterBattle.Core
{
    public abstract class MoveEffect : ScriptableObject
    {
        public abstract void Execute(BattleCharacter caster, List<BattleCharacter> targets, BattleController battleController);
        public abstract bool CanExecute(BattleCharacter caster, List<BattleCharacter> targets);
        public abstract string GetDescription();
    }
}
