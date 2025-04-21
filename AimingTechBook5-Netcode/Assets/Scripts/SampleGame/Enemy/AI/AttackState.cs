using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SampleGame.Enemy.AI
{
    public class AttackState : IEnemyState
    {
        private GameObject _enemyBulletPrefab;
        private readonly float _shootInterval;
        private Transform _enemyTransform;
        private readonly int _targetPlayerClientId;
        private Transform _targetPlayerTransform;
        private readonly float _stateDuration;
        
        public EnemyState State => EnemyState.Attack;
        public EnemyStateInformation StateInformation =>
            EnemyStateInformation.CreateAttack(_stateDuration, _targetPlayerClientId);
        
        public AttackState(
            EnemyStateInformation enemyStateInformation,
            Transform enemyTransform,
            int targetPlayerClientId,
            Transform targetPlayerTransform,
            GameObject enemyBulletPrefab,
            float shootInterval)
        {
            
            _enemyBulletPrefab = enemyBulletPrefab;
            _shootInterval = shootInterval;
            _enemyTransform = enemyTransform;
            _targetPlayerClientId = targetPlayerClientId;
            _targetPlayerTransform = targetPlayerTransform;
            _stateDuration = enemyStateInformation.Duration;
        }

        public AttackState(
            GameObject enemyBulletPrefab,
            float shootInterval,
            Transform enemyTransform,
            int targetPlayerClientId,
            Transform targetPlayerTransform,
            float stateDuration)
        {
            _enemyBulletPrefab = enemyBulletPrefab;
            _shootInterval = shootInterval;
            _enemyTransform = enemyTransform;
            _targetPlayerClientId = targetPlayerClientId;
            _targetPlayerTransform = targetPlayerTransform;
            _stateDuration = stateDuration;
        }

        public async UniTask Execute(CancellationToken ct)
        {
            Debug.Log($"<color=yellow>Enter Attack State, Target:{_targetPlayerClientId}</color>");
            
            var elapsedTime = 0f;

            while (elapsedTime < _stateDuration)
            {
                if (ct.IsCancellationRequested)
                {
                    return;
                }

                // _enemyTransformを_targetPlayerTransformに向ける
                var direction = (_targetPlayerTransform.position - _enemyTransform.position).normalized;
                _enemyTransform.rotation = Quaternion.LookRotation(direction);

                // 弾を発射
                Shoot();

                // 次の発射まで待機
                elapsedTime += _shootInterval;
                await UniTask.Delay((int)(_shootInterval * 1000), cancellationToken: ct);
            }
            
            Debug.Log($"<color=yellow>Exit Attack State, Target:{_targetPlayerClientId}</color>");
        }
        
        private void Shoot()
        {
            if (_enemyBulletPrefab == null) return;

            // 弾を生成して発射
            Object.Instantiate(_enemyBulletPrefab, _enemyTransform.position, _enemyTransform.rotation);
        }
    }
}