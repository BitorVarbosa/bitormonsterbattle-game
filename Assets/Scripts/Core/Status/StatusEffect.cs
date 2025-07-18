using UnityEngine;

namespace BitorMonsterBattle.Core
{
    public abstract class StatusEffect
    {
        protected int Duration;
        protected int TurnsRemaining;

        public StatusEffect(int duration)
        {
            this.Duration = duration;
            this.TurnsRemaining = duration;
        }

        public abstract void OnApplied(BattleCharacter character);
        public abstract void Process(BattleCharacter character);
        public abstract void OnRemoved(BattleCharacter character);
        public abstract void ModifyStats(BattleCharacter character);

        public virtual void Refresh(StatusEffect newEffect)
        {
            TurnsRemaining = Duration; // Reset duration
        }

        public bool IsExpired() => TurnsRemaining <= 0;

        public void DecrementDuration() => TurnsRemaining--;
    }
}
