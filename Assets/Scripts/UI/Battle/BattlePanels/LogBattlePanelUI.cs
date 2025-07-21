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

        void Awake()
        {
            _controller.OnActionExecuted += HandleActionExecuted;
        }

        private void HandleActionExecuted(BattleAction action)
        {
            _loggedActions.Add(action);

            // If the maximum number of logs is set to zero, just ignore the limit altogether
            if (_maxLoggedActions > 0 && _loggedActions.Count > _maxLoggedActions)
            {
                int rangeToRemove = _loggedActions.Count - _maxLoggedActions;
                _loggedActions.RemoveRange(0, rangeToRemove);
            }
        }

        public override void OpenPanel()
        {
            _logText.gameObject.SetActive(true);
            _rightDescription.gameObject.SetActive(true);

            _rightDescription.text = "Showing log";
            PopulateLogText();
        }

        private void PopulateLogText()
        {
            _logText.text = "";

            for (int i = 0; i < _loggedActions.Count; i++)
            {
                BattleAction action = _loggedActions[i];
                string targets = "";
                for (int target = 0; target < action.Targets.Count; target++)
                {
                    targets += action.Targets[target].CharacterData.CharacterName;
                    if (target + 1 < action.Targets.Count)
                        targets += ",";
                }

                _logText.text += $"\n{action.Actor.CharacterData.CharacterName} used {action.Move.MoveName} on {targets}";
            }
        }

        public override void ClosePanel()
        {
            _logText.gameObject.SetActive(false);
            _rightDescription.gameObject.SetActive(false);
        }
    }
}
