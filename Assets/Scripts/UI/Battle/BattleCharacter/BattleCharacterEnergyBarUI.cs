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
            // TODO: Subscribe to energy spend moments
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
