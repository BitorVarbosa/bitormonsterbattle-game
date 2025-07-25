using System;
using System.Collections.Generic;
using System.Linq;
using BitorMonsterBattle.Core;
using TMPro;
using UnityEngine;

namespace BitorMonsterBattle.UI
{
    public class LogBattlePanelUI : BattlePanelUI
    {
        // When the log opens, show full log on left, show description on right
        // When the log closes, disable both text panels

        [SerializeField] private BattleController _controller;
        [SerializeField] private TextMeshProUGUI _logText;
        [SerializeField] private TextMeshProUGUI _rightDescription;
        [SerializeField] private int _maxLoggedActions;

        private List<BattleAction> _loggedActions = new List<BattleAction>();

        public void RegisterBattleAction(BattleAction action)
        {
            _loggedActions.Add(action);

            // If the maximum number of logs is set to zero, just ignore the limit altogether
            if (_maxLoggedActions > 0 && _loggedActions.Count > _maxLoggedActions)
            {
                int rangeToRemove = _loggedActions.Count - _maxLoggedActions;
                _loggedActions.RemoveRange(0, rangeToRemove);
            }
        }

        protected override void OnPanelOpen()
        {
            _logText.gameObject.SetActive(true);
            _rightDescription.gameObject.SetActive(true);

            _rightDescription.text = "";
            PopulateLogText();
        }

        private void PopulateLogText()
        {
            // Each line of the log text should look like: CreatureA used MoveName on EnemyName, EnemyName, EnemyName
            _logText.text = "";

            for (int i = 0; i < _loggedActions.Count; i++)
            {
                // We will need a list of all target names separated by commas
                string targets = "";
                BattleAction action = _loggedActions[i];
                IReadOnlyList<BattleCharacter> targetList = action.GetTargets();

                for (int target = 0; target < targetList.Count; target++)
                {
                    targets += targetList[target].CharacterData.CharacterName;
                    if (target + 1 < targetList.Count)
                        targets += ",";
                }

                _logText.text += $"\n{action.Actor.CharacterData.CharacterName} used {action.Move.MoveName} on {targets}";
            }
        }

        protected override void OnPanelHide()
        {
            _logText.gameObject.SetActive(false);
            _rightDescription.gameObject.SetActive(false);
        }
    }
}
