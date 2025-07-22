using System;
using System.Collections.Generic;
using UnityEngine;

namespace BitorMonsterBattle.Core
{
    public class BattleCharacter : MonoBehaviour
    {
        // TODO: Remove from the inspector and setup with battle information from player settings and level design, inspector setup is for testing only
        [Header("References")]
        [SerializeField] private CharacterData _characterData;
        [SerializeField] private Team _team;

        // Current stats (can be modified during battle)
        public int CurrentLife { get; private set; }
        public int CurrentEnergy { get; private set; }
        public int CurrentSpeed { get; private set; }
        public int CurrentAttack { get; private set; }

        // Max stats (base + buffs)
        public int MaxLife { get; private set; }
        public int MaxEnergy { get; private set; }

        // Ongoing StatusEffects
        private List<StatusEffect> _activeStatusEffects = new List<StatusEffect>();

        public Action<BattleCharacter> OnInitialized;
        public Action<BattleCharacter> OnCharacterDeath;
        public Action<BattleCharacter, int> OnDamageTaken;
        public Action<BattleCharacter, int> OnHealed;

        public CharacterData CharacterData => _characterData;
        public Team Team => _team;
        public bool IsAlive => CurrentLife > 0;

        void Start()
        {
            InitializeStats();
        }

        void InitializeStats()
        {
            if (CharacterData == null) return;

            MaxLife = CharacterData.BaseLife;
            MaxEnergy = CharacterData.BaseEnergy;
            CurrentLife = MaxLife;
            CurrentEnergy = MaxEnergy;
            CurrentSpeed = CharacterData.BaseSpeed;
            CurrentAttack = CharacterData.BaseAttack;
            OnInitialized?.Invoke(this);
        }

        public void TakeDamage(int damage, BattleCharacter attacker)
        {
            if (!IsAlive) return;

            // TODO: Reduce damage through a defense stat
            int actualDamage = Mathf.Max(1, damage);
            CurrentLife = Mathf.Max(0, CurrentLife - actualDamage);

            OnDamageTaken?.Invoke(this, actualDamage);

            if (CurrentLife <= 0)
            {
                Die();
            }
        }

        public void Heal(int amount)
        {
            if (!IsAlive) return;

            int actualHeal = Mathf.Min(amount, MaxLife - CurrentLife);
            CurrentLife += actualHeal;

            OnHealed?.Invoke(this, actualHeal);
        }

        public void ConsumeEnergy(int amount)
        {
            CurrentEnergy = Mathf.Max(0, CurrentEnergy - amount);
        }

        public void RestoreEnergy(int amount)
        {
            CurrentEnergy = Mathf.Min(MaxEnergy, CurrentEnergy + amount);
        }

        public bool CanUseMove(MoveData move)
        {
            return IsAlive && CurrentEnergy >= move.EnergyCost;
        }

        public void ApplyStatusEffect(StatusEffect effect)
        {
            // Check if effect already exists and handle stacking
            var existingEffect = _activeStatusEffects.Find(e => e.GetType() == effect.GetType());
            if (existingEffect != null)
            {
                existingEffect.Refresh(effect);
            }
            else
            {
                _activeStatusEffects.Add(effect);
                effect.OnApplied(this);
            }
        }

        public void ProcessStatusEffects()
        {
            for (int i = _activeStatusEffects.Count - 1; i >= 0; i--)
            {
                var effect = _activeStatusEffects[i];
                effect.Process(this);

                if (effect.IsExpired())
                {
                    effect.OnRemoved(this);
                    _activeStatusEffects.RemoveAt(i);
                }
            }
        }

        public void RecalculateStats()
        {
            // Reset to base stats
            CurrentSpeed = CharacterData.BaseSpeed;
            CurrentAttack = CharacterData.BaseAttack;

            // Apply status effect modifiers
            foreach (var effect in _activeStatusEffects)
            {
                effect.ModifyStats(this);
            }
        }

        void Die()
        {
            OnCharacterDeath?.Invoke(this);
            // Clear all status effects
            _activeStatusEffects.Clear();
        }

        public void StartTurn()
        {
            ProcessStatusEffects();
            RecalculateStats();
            RestoreEnergy(CharacterData.ENERGY_RECOVERY); // Restore some energy each turn
        }

        public void EndTurn()
        {
            // Any end of turn processing
        }
    }
}
