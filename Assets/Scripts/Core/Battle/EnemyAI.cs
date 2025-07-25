using System.Collections.Generic;
using System.Linq;

namespace BitorMonsterBattle.Core
{
    public class EnemyAI
    {
        public static BattleAction DecideBattleAction(BattleController controller)
        {
            BattleCharacter character = controller.CurrentCharacter;
            var availableMoves = character.CharacterData.GetAvailableMoves()
                .Where(m => character.CanUseMove(m))
                .ToList();

            if (availableMoves.Count > 0)
            {
                var selectedMove = availableMoves[UnityEngine.Random.Range(0, availableMoves.Count)];
                var targets = controller.GetValidTargets(character, selectedMove.TargetType);

                if (targets.Count > 0)
                {
                    var selectedTargets = new List<BattleCharacter> { targets[UnityEngine.Random.Range(0, targets.Count)] };
                    var action = new BattleAction(character, selectedMove, selectedTargets);
                    return action;
                }
                else
                {
                    // No valid targets
                    return null;
                }
            }
            else
            {
                // No available moves
                return null;
            }
        }
    }
}
