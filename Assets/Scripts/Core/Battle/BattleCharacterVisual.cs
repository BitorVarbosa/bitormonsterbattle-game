using System;
using UnityEngine;

namespace BitorMonsterBattle.Core
{
    [RequireComponent(typeof(BattleCharacter))]
    public class BattleCharacterVisual : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _characterModel;
        private BattleCharacter _battleCharacter;

        void Awake()
        {
            _battleCharacter = GetComponent<BattleCharacter>();
            _battleCharacter.OnInitialized += HandleInitialized;
        }

        private void HandleInitialized(BattleCharacter character)
        {
            _characterModel.sprite = character.CharacterData.CharacterSprite;
        }
    }
}
