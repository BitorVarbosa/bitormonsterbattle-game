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
        [SerializeField] private List<BattleCharacter> PlayerTeam = new List<BattleCharacter>();
        [SerializeField] private List<BattleCharacter> EnemyTeam = new List<BattleCharacter>();

        // Turn management
        private BattleTurnCalculator _turnCalculator;
        public BattleCharacter CurrentCharacter { get; private set; }

        // Battle State
        public BattleState CurrentState { get; private set; }

        // Events
        public event Action<BattleCharacter> OnCharacterTurnStart;
        public event Action<BattleAction> OnActionExecuted;
        public event Action<Team> OnBattleEnd;

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
                character.InitializeStats();
                character.OnCharacterDeath += OnCharacterDeath;
            }

            _turnCalculator = new BattleTurnCalculator(GetAllCharacters());

            StartBattle();
        }

        void StartBattle()
        {
            CurrentState = BattleState.TurnStart;
            StartCoroutine(StartNextTurn());
        }

        System.Collections.IEnumerator StartNextTurn()
        {
            yield return new WaitForSeconds(0.5f);

            BattleCharacter nextCharacter;
            WaitForSeconds timerInterval = new WaitForSeconds(0.01f);
            while (!_turnCalculator.IsCharacterReady(out nextCharacter))
            {
                _turnCalculator.ProgressTimers(0.1f);
                yield return timerInterval;
            }

            CurrentCharacter = nextCharacter;
            StartTurn();
        }

        void StartTurn()
        {
            // Check if character is still alive
            if (!CurrentCharacter.IsAlive)
            {
                StartTurn();
                return;
            }

            CurrentState = BattleState.SelectingActions;

            CurrentCharacter.StartTurn();
            OnCharacterTurnStart?.Invoke(CurrentCharacter);

            // Wait for action to be executed by the player or enemy controller
        }

        public void ExecuteBattleAction(BattleAction action)
        {
            if (CurrentState == BattleState.SelectingActions)
            {
                if (action == null)
                {
                    EndCharacterTurn();
                    return;
                }
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
            CurrentCharacter.EndTurn();

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
            // If it was the current character's turn, move to next
            if (CurrentCharacter == character)
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

        public List<BattleCharacter> GetAllCharacters()
        {
            var allCharacters = new List<BattleCharacter>();
            allCharacters.AddRange(PlayerTeam);
            allCharacters.AddRange(EnemyTeam);
            return allCharacters;
        }

        public List<BattleCharacter> GetTeamMembers(Team team)
        {
            return team == Team.Player ? PlayerTeam : EnemyTeam;
        }

        public List<BattleCharacter> GetEnemyTeamMembers(Team team)
        {
            return team == Team.Player ? EnemyTeam : PlayerTeam;
        }
    }
}
