using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace SampleGame.Enemy.AI
{
    public class IdleState : IEnemyState
    {
        private readonly float _stateDuration;
        
        public EnemyState State => EnemyState.Idle;
        public EnemyStateInformation StateInformation =>
            EnemyStateInformation.CreateIdle(_stateDuration);

        public IdleState(EnemyStateInformation enemyStateInformation)
        {
            _stateDuration = enemyStateInformation.Duration;
        }

        public IdleState(float stateDuration)
        {
            _stateDuration = stateDuration;
        }

        public async UniTask Execute(CancellationToken ct)
        {
            Debug.Log("<color=yellow>Enter Idle State</color>");
            
            await UniTask.Delay((int)(_stateDuration * 1000), cancellationToken: ct);
            
            Debug.Log("<color=yellow>Exit Idle State</color>");
        }
    }
}