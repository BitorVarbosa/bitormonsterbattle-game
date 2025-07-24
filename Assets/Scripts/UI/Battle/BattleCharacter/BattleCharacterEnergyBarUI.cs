using UnityEngine;
using BitorTools;
using BitorMonsterBattle.Core;
using System;

namespace BitorMonsterBattle.UI
{
    public class BattleCharacterEnergyBarUI : ProgressBarUI
    {
        [SerializeField] private BattleCharacter _character;

        void Awake()
        {
            _character.OnInitialized += HandleInitialized;
            _character.OnEnergyConsumed += HandleEnergyChanged;
        }

        private void HandleEnergyChanged(BattleCharacter character, int arg2)
        {
            UpdateBar();
        }

        private void HandleInitialized(BattleCharacter character)
        {
            UpdateBar();
        }

        protected override float GetCurrentValue()
        {
            return _character.CurrentEnergy;
        }

        protected override float GetMaxValue()
        {
            return _character.MaxEnergy;
        }
    }
}
