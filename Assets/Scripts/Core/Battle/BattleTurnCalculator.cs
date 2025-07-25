using System.Collections.Generic;
using System.Linq;


namespace BitorMonsterBattle.Core
{
    public class BattleTurnCalculator
    {
        private const float TIME_REQUIREMENT = 100;
        private Dictionary<BattleCharacter, float> _timers = new Dictionary<BattleCharacter, float>();

        public BattleTurnCalculator(List<BattleCharacter> characters)
        {
            foreach (var character in characters)
            {
                if (!_timers.ContainsKey(character))
                    _timers.Add(character, 0);
            }
        }

        public void ProgressTimers(float timePassed)
        {
            BattleCharacter[] keys = _timers.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                _timers[keys[i]] += timePassed * keys[i].CurrentSpeed;
            }
        }

        public bool IsCharacterReady(out BattleCharacter character)
        {
            character = null;
            bool ready = false;
            foreach (var pair in _timers)
            {
                // Ignore dead characters
                if (!pair.Key.IsAlive) continue;

                float currentTime = _timers[pair.Key];
                if (currentTime >= TIME_REQUIREMENT)
                {
                    ready = true;
                    character = character == null || _timers[character] < currentTime ? pair.Key : character;
                }
            }

            if (character != null)
                _timers[character] -= TIME_REQUIREMENT;
            return ready;
        }
    }
}
