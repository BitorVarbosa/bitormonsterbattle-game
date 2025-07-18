using UnityEngine;

namespace BitorMonsterBattle.Core
{
    public class TurnEntry
    {
        public BattleCharacter Character { get; private set; }
        public float TimeToTurn { get; private set; }
        public int TurnNumber { get; private set; }

        public TurnEntry(BattleCharacter character, float timeToTurn, int turnNumber)
        {
            this.Character = character;
            this.TimeToTurn = timeToTurn;
            this.TurnNumber = turnNumber;
        }
    }
}
