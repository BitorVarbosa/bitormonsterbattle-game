using System;
using BitorMonsterBattle.Core;
using UnityEngine;

namespace BitorMonsterBattle.UI
{
    public class BattlePanelsControllerUI : MonoBehaviour
    {
        [SerializeField] private BattleController _controller;

        [Header("Panels")]
        [SerializeField] private MovesBattlePanelUI _movesPanel;
        [SerializeField] private LogBattlePanelUI _logPanel;

        private BattlePanelUI _currentBattlePanel;

        void Awake()
        {
            _controller.OnCharacterTurnStart += HandleCharacterTurnStart;
            _controller.OnActionExecuted += HandleActionExecuted;
        }

        private void HandleActionExecuted(BattleAction action)
        {
            // Always show the log screen so we can follow what is happening
            _logPanel.OpenPanel();
        }

        private void HandleCharacterTurnStart(BattleCharacter character)
        {
            // If it's a player's turn, show the Moves available
            if (character.Team == Team.Player)
            {
                _movesPanel.OpenPanel();
            }

            // If it's the enemy's turn, change to log panel
            else
            {
                _logPanel.OpenPanel();
            }
        }
    }
}
