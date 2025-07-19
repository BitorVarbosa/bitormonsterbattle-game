using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace BitorMonsterBattle.Core
{
    [CreateAssetMenu(fileName = "Damage Effect", menuName = "BitorMonsterBattle/Effects/Damage Effect")]
    public class DamageEffect : MoveEffect
    {
        // TODO: Add support for elemental damage and piercing damage

        [Header("Damage Settings")]
        [SerializeField] private int _baseDamage = 20;
        [SerializeField] private bool _useAttackStat = true;
        [SerializeField] private float _attackMultiplier = 1.0f;

        public override void Execute(BattleCharacter caster, List<BattleCharacter> targets, BattleController battleManager)
        {
            foreach (var target in targets)
            {
                int damage = _baseDamage;
                if (_useAttackStat)
                    damage += Mathf.RoundToInt(caster.CurrentAttack * _attackMultiplier);

                target.TakeDamage(damage, caster);
            }
        }

        public override bool CanExecute(BattleCharacter caster, List<BattleCharacter> targets)
        {
            return targets.Count > 0 && targets.All(t => t.IsAlive);
        }

        public override string GetDescription()
        {
            return _useAttackStat ? $"Deal {_baseDamage} damage plus ATK * {_attackMultiplier}" : $"Deal {_baseDamage} damage";
        }
    }
}
