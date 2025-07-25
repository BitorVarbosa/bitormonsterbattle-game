using System;
using System.Collections.Generic;
using BitorMonsterBattle.Core;
using TMPro;
using UnityEngine;

namespace BitorMonsterBattle.UI
{
    public class MovesBattlePanelUI : BattlePanelUI
    {
        // When this panel opens, show the skills on the left, show description on right and change text to "select a move"
        // When the log closes, disable both text panels

        [SerializeField] private BattleController _controller;
        [SerializeField] private MoveButtonUI _moveButtonPrefab;
        [SerializeField] private Transform _moveButtonContainer;
        [SerializeField] private TextMeshProUGUI _leftDescription;
        [SerializeField] private TextMeshProUGUI _rightDescription;

        private MoveData _selectedMove;
        private Queue<MoveButtonUI> _displayedMoves = new Queue<MoveButtonUI>();

        protected override void OnPanelOpen()
        {
            ClearMoveButtonList();

            _moveButtonContainer.gameObject.SetActive(true);

            _rightDescription.gameObject.SetActive(true);
            _rightDescription.text = "Select a Move to use.";

            _leftDescription.gameObject.SetActive(false);
            PopulateMoveButtonList();
        }

        private void PopulateMoveButtonList()
        {
            CharacterData character = _controller.CurrentCharacter.CharacterData;

            // If the current character is a player, instance buttons for each move
            if (_controller.CurrentCharacter.Team == Team.Player)
            {
                IReadOnlyList<MoveData> availableMoves = character.GetAvailableMoves();

                for (int i = 0; i < availableMoves.Count; i++)
                {
                    MoveButtonUI button = GetAvailableMoveButton();
                    button.gameObject.SetActive(true);
                    _displayedMoves.Enqueue(button);

                    MoveData move = availableMoves[i];
                    button.Setup(move.MoveName, move.MoveIcon, move);
                    button.OnClick += HandleMoveClicked;
                    button.OnHover += HandleMoveButtonHover;
                }
            }
            else
            {
                _leftDescription.gameObject.SetActive(true);
                _leftDescription.text = "The turn owner is an enemy.";
            }
        }

        private MoveButtonUI GetAvailableMoveButton()
        {
            // TODO: Transform into actual pool for minimum performance save when a turn starts (not frequent)
            return Instantiate<MoveButtonUI>(_moveButtonPrefab, _moveButtonContainer);
        }

        // Select targets and cast the move
        private void HandleMoveClicked(MoveData move)
        {
            _selectedMove = move;
            List<BattleCharacter> targets = _controller.GetValidTargets(_controller.CurrentCharacter, move.TargetType);

            // If the move needs a target to be manually selected, then we will allow the play to click on a valid character
            if (move.TargetType == TargetType.SingleAlly || move.TargetType == TargetType.SingleEnemy || move.TargetType == TargetType.SingleAny)
            {
                foreach (var character in targets)
                {
                    character.GetComponent<BattleCharacterButtonUI>().EnableSelection(false, HandleSingleTargetSelected);
                }
            }
            // Otherwise, we can just execute the action with the valid targets
            else
            {
                _controller.ExecuteBattleAction(new BattleAction(_controller.CurrentCharacter, _selectedMove, targets));
            }
        }

        private void HandleSingleTargetSelected(BattleCharacter target)
        {
            _controller.ExecuteBattleAction(new BattleAction(_controller.CurrentCharacter, _selectedMove, new List<BattleCharacter>() { target }));

            foreach (var character in _controller.GetAllCharacters())
            {
                character.GetComponent<BattleCharacterButtonUI>().DisableSelection();
            }
        }

        // When hovering a move button, show description on the right of the screen
        private void HandleMoveButtonHover(MoveData move)
        {
            _rightDescription.text = move.Description;
        }

        private void ClearMoveButtonList()
        {
            while (_displayedMoves.Count > 0)
            {
                MoveButtonUI button = _displayedMoves.Dequeue();
                Destroy(button.gameObject);
            }
        }

        protected override void OnPanelHide()
        {
            _rightDescription.gameObject.SetActive(false);
            _leftDescription.gameObject.SetActive(false);
            ClearMoveButtonList();
        }
    }
}
