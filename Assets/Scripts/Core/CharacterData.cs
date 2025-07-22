using System.Collections.Generic;
using UnityEngine;

namespace BitorMonsterBattle.Core
{
    [CreateAssetMenu(fileName = "New Character Data", menuName = "BitorMonsterBattle/Character Data")]
    public class CharacterData : ScriptableObject
    {
        public const int ENERGY_RECOVERY = 5;

        [Header("Basic Info")]
        [SerializeField] private string _characterName;
        [SerializeField] private Sprite _characterSprite;

        [Header("Base Stats")]
        [SerializeField] private int _baseLife = 100;
        [SerializeField] private int _baseEnergy = 50;
        [SerializeField] private int _baseSpeed = 10;
        [SerializeField] private int _baseAttack = 15;

        [Header("Moves")]
        [SerializeField] private List<MoveData> _availableMoves = new List<MoveData>();

        // Public references from inspector variables
        public string CharacterName => _characterName;
        public Sprite CharacterSprite => _characterSprite;
        public int BaseLife => _baseLife;
        public int BaseEnergy => _baseEnergy;
        public int BaseSpeed => _baseSpeed;
        public int BaseAttack => _baseAttack;

        public IReadOnlyList<MoveData> GetAvailableMoves() => _availableMoves.AsReadOnly();
    }
}
