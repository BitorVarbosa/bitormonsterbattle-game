using System.Collections.Generic;
using UnityEngine;

namespace BitorMonsterBattle.Core
{
    [CreateAssetMenu(fileName = "New Move Data", menuName = "BitorMonsterBattle/Move Data")]
    public class MoveData : ScriptableObject
    {
        [Header("Basic Info")]
        [SerializeField] private string _moveName;
        [SerializeField] private string _description;
        [SerializeField] private Sprite _moveIcon;

        [Header("Properties")]
        [SerializeField] private int _energyCost = 10;
        [SerializeField] private TargetType _targetType = TargetType.SingleEnemy;

        [Header("Effects")]
        public List<MoveEffect> Effects = new List<MoveEffect>();

        // Public references from inspector variables
        public string MoveName => _moveName;
        public string Description => _description;
        public Sprite MoveIcon => _moveIcon;
        public int EnergyCost => _energyCost;
        public TargetType TargetType => _targetType;
    }
}
