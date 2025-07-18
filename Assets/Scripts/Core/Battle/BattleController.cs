using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BitorMonsterBattle.Core
{
    public class BattleController : MonoBehaviour
    {
        [Header("Battle Setup")]
        public List<BattleCharacter> PlayerTeam = new List<BattleCharacter>();
        public List<BattleCharacter> EnemyTeam = new List<BattleCharacter>();

        [Header("Battle State")]
        public BattleState CurrentState { get; private set; }

        [Header("Turn Settings")]
        [SerializeField] private int _turnsToCalculate = 15;
        [SerializeField] private float _baseSpeedValue = 10f; // Base speed reference for turn calculation

        // Turn management
        private List<TurnEntry> _calculatedTurns = new List<TurnEntry>();
        private int _currentTurnIndex = 0;
        private BattleCharacter _currentCharacter;

        // Events
        public event Action<BattleCharacter> OnCharacterTurnStart;
        public event Action<BattleAction> OnActionExecuted;
        public event Action<Team> OnBattleEnd;
        public event Action<List<TurnEntry>> OnTurnOrderUpdated; // For UI to show turn order

        void Start()
        {
            InitializeBattle();
        }

        void InitializeBattle()
        {
            CurrentState = BattleState.Setup;

            // Subscribe to character death events
            foreach (var character in GetAllCharacters())
            {
                character.OnCharacterDeath += OnCharacterDeath;
            }

            StartBattle();
        }

        void StartBattle()
        {
            CurrentState = BattleState.TurnStart;
            CalculateTurnOrder();
            StartTurn();
        }

        void CalculateTurnOrder()
        {
            _calculatedTurns.Clear();

            var aliveCharacters = GetAllCharacters().Where(c => c.IsAlive).ToList();
            if (aliveCharacters.Count == 0) return;

            // Calculate when each character gets their next turns
            var characterNextTurnTime = new Dictionary<BattleCharacter, float>();
            var characterTurnCount = new Dictionary<BattleCharacter, int>();

            // Initialize each character's next turn time and turn count
            foreach (var character in aliveCharacters)
            {
                characterNextTurnTime[character] = GetTurnInterval(character);
                characterTurnCount[character] = 1;
            }

            // Calculate the next 'turnsToCalculate' turns
            for (int i = 0; i < _turnsToCalculate; i++)
            {
                // Find the character with the earliest next turn
                var nextCharacter = characterNextTurnTime
                    .Where(kvp => kvp.Key.IsAlive)
                    .OrderBy(kvp => kvp.Value)
                    .ThenBy(kvp => UnityEngine.Random.Range(0f, 1f)) // Random tiebreaker
                    .FirstOrDefault();

                if (nextCharacter.Key == null) break;

                // Add this turn to the calculated turns
                _calculatedTurns.Add(new TurnEntry(
                    nextCharacter.Key,
                    nextCharacter.Value,
                    characterTurnCount[nextCharacter.Key]
                ));

                // Update the character's next turn time
                characterTurnCount[nextCharacter.Key]++;
                characterNextTurnTime[nextCharacter.Key] += GetTurnInterval(nextCharacter.Key);
            }

            OnTurnOrderUpdated?.Invoke(_calculatedTurns);
        }

        float GetTurnInterval(BattleCharacter character)
        {
            // Characters with higher speed get turns more frequently
            // A character with double speed gets turns at half the interval
            return _baseSpeedValue / Mathf.Max(1f, character.CurrentSpeed);
        }

        void StartTurn()
        {
            if (_currentTurnIndex >= _calculatedTurns.Count)
            {
                // Recalculate turn order if we've run out of calculated turns
                CalculateTurnOrder();
                _currentTurnIndex = 0;
            }

            if (_calculatedTurns.Count == 0)
            {
                EndBattle();
                return;
            }

            var currentTurnEntry = _calculatedTurns[_currentTurnIndex];
            _currentCharacter = currentTurnEntry.Character;

            // Check if character is still alive
            if (!_currentCharacter.IsAlive)
            {
                _currentTurnIndex++;
                StartTurn();
                return;
            }

            _currentCharacter.StartTurn();
            OnCharacterTurnStart?.Invoke(_currentCharacter);

            if (_currentCharacter.Team == Team.Player)
            {
                CurrentState = BattleState.SelectingActions;
                // Wait for player input
            }
            else
            {
                CurrentState = BattleState.SelectingActions;
                // AI makes decision
                StartCoroutine(AIDecision());
            }
        }

        System.Collections.IEnumerator AIDecision()
        {
            yield return new WaitForSeconds(1f); // AI thinking time

            // Simple AI: Use first available move on random valid target
            var availableMoves = _currentCharacter.CharacterData.AvailableMoves
                .Where(m => _currentCharacter.CanUseMove(m))
                .ToList();

            if (availableMoves.Count > 0)
            {
                var selectedMove = availableMoves[UnityEngine.Random.Range(0, availableMoves.Count)];
                var targets = GetValidTargets(_currentCharacter, selectedMove.TargetType);

                if (targets.Count > 0)
                {
                    var selectedTargets = new List<BattleCharacter> { targets[UnityEngine.Random.Range(0, targets.Count)] };
                    var action = new BattleAction(_currentCharacter, selectedMove, selectedTargets);
                    StartCoroutine(ExecuteActionRoutine(action));
                }
                else
                {
                    // No valid targets, end turn
                    EndCharacterTurn();
                }
            }
            else
            {
                // No available moves, end turn
                EndCharacterTurn();
            }
        }

        public void ExecutePlayerAction(BattleAction action)
        {
            if (_currentCharacter.Team == Team.Player && CurrentState == BattleState.SelectingActions)
            {
                StartCoroutine(ExecuteActionRoutine(action));
            }
        }

        IEnumerator ExecuteActionRoutine(BattleAction action)
        {
            CurrentState = BattleState.ExecutingActions;

            if (action.CanExecute())
            {
                action.Execute(this);
                OnActionExecuted?.Invoke(action);
                yield return new WaitForSeconds(1f); // Animation time
            }

            if (CheckBattleEnd())
            {
                EndBattle();
            }
            else
            {
                EndCharacterTurn();
            }
        }

        public void EndCharacterTurn()
        {
            _currentCharacter.EndTurn();
            _currentTurnIndex++;

            // Recalculate turn order after each turn to account for speed changes
            CalculateTurnOrder();
            _currentTurnIndex = 0; // Reset to start of new calculation

            if (CheckBattleEnd())
            {
                EndBattle();
            }
            else
            {
                CurrentState = BattleState.TurnEnd;
                StartCoroutine(StartNextTurn());
            }
        }

        System.Collections.IEnumerator StartNextTurn()
        {
            yield return new WaitForSeconds(0.5f);
            StartTurn();
        }

        bool CheckBattleEnd()
        {
            bool playerTeamAlive = PlayerTeam.Any(c => c.IsAlive);
            bool enemyTeamAlive = EnemyTeam.Any(c => c.IsAlive);

            return !playerTeamAlive || !enemyTeamAlive;
        }

        void EndBattle()
        {
            CurrentState = BattleState.BattleEnd;

            bool playerWon = PlayerTeam.Any(c => c.IsAlive);
            Team winner = playerWon ? Team.Player : Team.Enemy;

            OnBattleEnd?.Invoke(winner);
        }

        void OnCharacterDeath(BattleCharacter character)
        {
            // Remove all future turns for this character
            _calculatedTurns.RemoveAll(turn => turn.Character == character);

            // If it was the current character's turn, move to next
            if (_currentCharacter == character)
            {
                EndCharacterTurn();
            }
        }

        public List<BattleCharacter> GetValidTargets(BattleCharacter caster, TargetType targetType)
        {
            switch (targetType)
            {
                case TargetType.Self:
                    return new List<BattleCharacter> { caster };

                case TargetType.SingleAlly:
                    return GetTeamMembers(caster.Team).Where(c => c.IsAlive && c != caster).ToList();

                case TargetType.AllAllies:
                    return GetTeamMembers(caster.Team).Where(c => c.IsAlive && c != caster).ToList();

                case TargetType.SingleEnemy:
                    return GetEnemyTeamMembers(caster.Team).Where(c => c.IsAlive).ToList();

                case TargetType.AllEnemies:
                    return GetEnemyTeamMembers(caster.Team).Where(c => c.IsAlive).ToList();

                case TargetType.SingleAny:
                    return GetAllCharacters().Where(c => c.IsAlive).ToList();

                case TargetType.AllCharacters:
                    return GetAllCharacters().Where(c => c.IsAlive).ToList();

                default:
                    return new List<BattleCharacter>();
            }
        }

        List<BattleCharacter> GetAllCharacters()
        {
            var allCharacters = new List<BattleCharacter>();
            allCharacters.AddRange(PlayerTeam);
            allCharacters.AddRange(EnemyTeam);
            return allCharacters;
        }

        List<BattleCharacter> GetTeamMembers(Team team)
        {
            return team == Team.Player ? PlayerTeam : EnemyTeam;
        }

        List<BattleCharacter> GetEnemyTeamMembers(Team team)
        {
            return team == Team.Player ? EnemyTeam : PlayerTeam;
        }
    }
}
