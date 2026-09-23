using System;
using BitorMonsterBattle.Core;
using UnityEngine;

namespace BitorMonsterBattle.UI
{
    public class BattlePanelsControllerUI : MonoBehaviour
    {
        [SerializeField] private BattleController _controller;

        void Awake()
        {
            _controller.OnCharacterTurnStart += HandleCharacterTurnStart;
            _controller.OnActionExecuted += HandleActionExecuted;
        }

        void Start()
        {
            
        }

        private void HandleActionExecuted(BattleAction action)
        {
            
        }

        private void HandleCharacterTurnStart(BattleCharacter character)
        {
            // If it's a player's turn, show the Moves available
            if (character.Team == Team.Player)
            {
                
            }

            // If it's the enemy's turn, change to log panel
            else
            {
                
            }
        }
    }
}
