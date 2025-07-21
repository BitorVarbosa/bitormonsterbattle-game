using UnityEngine;
using BitorTools;
using BitorMonsterBattle.Core;
using System;

namespace BitorMonsterBattle.UI
{
    public class BattleCharacterLifeBarUI : ProgressBarUI
    {
        [SerializeField] private BattleCharacter _character;

        void Awake()
        {
            _character.OnDamageTaken += HandleDamageTaken;
            _character.OnInitialized += HandleInitialized;
        }

        private void HandleInitialized(BattleCharacter character)
        {
            UpdateBar();
        }

        private void HandleDamageTaken(BattleCharacter character, int arg2)
        {
            UpdateBar();
        }

        protected override float GetCurrentValue()
        {
            return _character.CurrentLife;
        }

        protected override float GetMaxValue()
        {
            return _character.MaxLife;
        }
    }
}
