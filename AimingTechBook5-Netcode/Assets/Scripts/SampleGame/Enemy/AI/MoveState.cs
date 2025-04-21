using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SampleGame.Enemy.AI
{
    public class MoveState : IEnemyState
    {
        private Transform _transform;
        private Vector3 _targetPosition;
        private readonly float _stateDuration;
        
        public EnemyState State => EnemyState.Move;
        public EnemyStateInformation StateInformation =>
            EnemyStateInformation.CreateMove(_stateDuration, _targetPosition);
        
        public MoveState(
            EnemyStateInformation enemyStateInformation,
            Transform transform)
        {
            _stateDuration = enemyStateInformation.Duration;
            _transform = transform;
            _targetPosition = enemyStateInformation.TargetPosition;
        }
        
        public MoveState(
            float stateDuration,
            Vector3 targetPosition,
            Transform transform)
        {
            _stateDuration = stateDuration;
            _targetPosition = targetPosition;
            _transform = transform;
        }
        
        public async UniTask Execute(CancellationToken ct)
        {
            Debug.Log($"<color=yellow>Enter Move State, Target:{_targetPosition}</color>");
            
            var startPosition = _transform.position;
            var elapsedTime = 0f;

            while (elapsedTime < _stateDuration)
            {
                if (ct.IsCancellationRequested)
                {
                    return;
                }

                elapsedTime += Time.deltaTime;
                var t = elapsedTime / _stateDuration;
                var newPosition = Vector3.Lerp(startPosition, _targetPosition, t);
                _transform.position = newPosition;

                await UniTask.Yield();
            }
            
            Debug.Log($"<color=yellow>Exit Move State</color>");
        }
    }
}