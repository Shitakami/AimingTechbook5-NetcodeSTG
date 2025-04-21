using Cysharp.Threading.Tasks;
using R3;
using Sample.Player;
using Unity.Netcode;
using UnityEngine;

namespace SampleGame.Enemy.AI
{
    public class EnemyBrain : NetworkBehaviour
    {
        [SerializeField] private EnemyHp _enemyHp;
        [SerializeField] private EnemyStateMachine _enemyStateMachine;
        [SerializeField] private Vector3 _minMovablePosition;
        [SerializeField] private Vector3 _maxMovablePosition;
        
        private PlayerInfoRegistry _playerInfoRegistry;

        public bool CompleteDown { get; set; }

        public override void OnNetworkSpawn()
        {
            _playerInfoRegistry = FindAnyObjectByType<PlayerInfoRegistry>();
            var maxHealth = _enemyHp.MaxHealth;
            var hpDownPhaseThreshold = maxHealth / 2f;
            
            if (IsServer)
            {
                _enemyStateMachine.OnStateDequeue
                    .Where(restStateCount => restStateCount <= 1)
                    .Subscribe(_ => AddEnemyStateArray())
                    .AddTo(this);
                
                _enemyHp.OnHealthChangedObservable
                    .Where(hp => hp <= hpDownPhaseThreshold)
                    .Take(1)
                    .DefaultIfEmpty()
                    .Subscribe(_ =>
                    {
                        CompleteDown = true;
                        _enemyStateMachine.Stop();
                        _enemyStateMachine.ClearState();
                        
                        AddEnemyDownState();
                        AddEnemyStateArray();
                        _enemyStateMachine.Execute().Forget();
                    })
                    .AddTo(this);
                
                AddEnemyStateArray();
            }
            
            if (IsClient && !IsHost)
            {
                _enemyHp.OnAnticipatedHealthChangedObservable
                    .Where(hp => hp <= hpDownPhaseThreshold && !CompleteDown)
                    .Take(1)
                    .DefaultIfEmpty()
                    .Subscribe(_ =>
                    {
                        CompleteDown = true;
                        _enemyStateMachine.Stop();
                        _enemyStateMachine.ClearState();

                        AddEnemyDownState();
                        _enemyStateMachine.Execute().Forget();
                    })
                    .AddTo(this);
            }
            
            _enemyStateMachine.Execute().Forget();
        }
        
        private void AddEnemyDownState()
        {
            var downState = EnemyStateInformation.CreateDown(3f);
            _enemyStateMachine.AddDownState(downState);
        }

        private void AddEnemyStateArray()
        {
            EnemyStateInformation[] enemyStateInformationArray;
            
            if (CompleteDown)
            {
                enemyStateInformationArray = new[]
                {
                    CreateIdleStateInformation(0.5f),
                    CreateMoveStateInformation(1.5f),
                    CreateMoveStateInformation(1.5f),
                    CreateAttackStateInformation(3f),
                };
            }
            else
            {
                enemyStateInformationArray = new[]
                {
                    CreateIdleStateInformation(1f),
                    CreateMoveStateInformation(3f),
                    CreateAttackStateInformation(3f),
                };
            }
            
            _enemyStateMachine.AddStateArrayWithNotify(enemyStateInformationArray);
        }
        
        private static EnemyStateInformation CreateIdleStateInformation(float duration)
        {
            return EnemyStateInformation.CreateIdle(duration);
        }

        private EnemyStateInformation CreateMoveStateInformation(float duration)
        {
            var targetPosition = GetRandomTargetPosition();
            return EnemyStateInformation.CreateMove(duration, targetPosition);
        }

        private EnemyStateInformation CreateAttackStateInformation(float duration)
        {
            var connectedPlayerCount = _playerInfoRegistry.GetConnectedPlayerCount();
            if (connectedPlayerCount == 0)
            {
                return CreateIdleStateInformation(duration);
            }

            // プレイヤーの中からランダムにターゲットを選ぶ
            var targetPlayerId = Random.Range(0, connectedPlayerCount);
            return new EnemyStateInformation(
                EnemyState.Attack,
                targetPlayerId,
                Vector3.zero,
                duration);
        }

        private Vector3 GetRandomTargetPosition()
        {
            var positionY = transform.position.y;
            
            return new Vector3(
                Random.Range(_minMovablePosition.x, _maxMovablePosition.x),
                positionY,
                Random.Range(_minMovablePosition.z, _maxMovablePosition.z));
        }
    }
}