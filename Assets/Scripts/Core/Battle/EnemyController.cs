using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BitorMonsterBattle.Core
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private BattleController _battleController;

        private void Awake() {
            _battleController.OnCharacterTurnStart += HandleTurnStart;
        }

        private void HandleTurnStart(BattleCharacter character)
        {
            if (character.Team == Team.Enemy)
            {
                StartCoroutine(AIDecision());
            }
        }

        System.Collections.IEnumerator AIDecision()
        {
            yield return new WaitForSeconds(1f); // Fake AI thinking time

            // Simple AI: Use first available move on random valid target
            var action = EnemyAI.DecideBattleAction(_battleController);

            _battleController.ExecuteBattleAction(action);
        }
    }
}
