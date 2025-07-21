using System;
using BitorMonsterBattle.Core;
using UnityEngine;
using UnityEngine.UI;

namespace BitorMonsterBattle.UI
{
    [RequireComponent(typeof(BattleCharacter))]
    public class BattleCharacterButtonUI : MonoBehaviour
    {
        [SerializeField] private Color _positiveButtonColor;
        [SerializeField] private Color _negativeButtonColor;
        [SerializeField] private Image _buttonImage;
        [SerializeField] private Button _button;

        private BattleCharacter _battleCharacter;
        private Action<BattleCharacter> _characterSelectCallback;

        void Start()
        {
            _battleCharacter = GetComponent<BattleCharacter>();
            _button.gameObject.SetActive(false);
            _button.onClick.AddListener(HandleClick);
        }

        public void EnableSelection(bool positiveHighlight, Action<BattleCharacter> onSelectAction)
        {
            _button.gameObject.SetActive(true);
            _characterSelectCallback = onSelectAction;
            _buttonImage.color = positiveHighlight ? _positiveButtonColor : _negativeButtonColor;
        }

        public void DisableSelection()
        {
            _characterSelectCallback = null;
            _button.gameObject.SetActive(false);
        }

        private void HandleClick()
        {
            _characterSelectCallback?.Invoke(_battleCharacter);
        }
    }
}
