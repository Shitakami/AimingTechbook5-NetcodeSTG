using Sample.Player;
using UnityEngine;

namespace SampleGame.Enemy.AI
{
    public class EnemyStateFactory : MonoBehaviour
    {
        [SerializeField] private Transform _enemyTransform;
        [SerializeField] private GameObject _enemyBulletPrefab;

        private PlayerInfoRegistry _playerInfoRegistry;
        
        private void Awake()
        {
            _playerInfoRegistry = FindAnyObjectByType<PlayerInfoRegistry>();
            if (_playerInfoRegistry == null)
            {
                Debug.LogError("PlayerInfoRegistry not found in the scene.");
            }
        }

        public IEnemyState CreateIdle(float stateDuration)
        {
            return new IdleState(stateDuration);
        }
        
        public IEnemyState CreateMove(float stateDuration, Vector3 targetPosition)
        {
            return new MoveState(stateDuration, targetPosition, _enemyTransform);
        }
        
        public IEnemyState CreateAttack(float stateDuration, int targetPlayClientId, float shootInterval)
        {
            if (_playerInfoRegistry.TryGetPlayerTransform(targetPlayClientId, out var targetPlayerTransform))
            {
                return new AttackState(
                    _enemyBulletPrefab,
                    shootInterval,
                    _enemyTransform,
                    targetPlayClientId,
                    targetPlayerTransform,
                    stateDuration
                );
            }
            
            Debug.LogError($"Player with ID {targetPlayClientId} not found.");
            return CreateIdle(stateDuration);
        }
        
        public IEnemyState CreateDown(float stateDuration)
        {
            return new DownState(stateDuration, _enemyTransform);
        }
    }
}