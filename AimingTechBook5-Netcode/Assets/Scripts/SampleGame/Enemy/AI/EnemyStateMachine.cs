using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace SampleGame.Enemy.AI
{
    public class EnemyStateMachine : MonoBehaviour
    {
        [SerializeField] private EnemyStateFactory _enemyStateFactory;
        private readonly Queue<IEnemyState> _enemyStateQueue = new();

        private IEnemyState _currentState;

        private readonly Subject<IReadOnlyCollection<EnemyStateInformation>> _addStatesSubject = new();
        private readonly Subject<int> _stateDequeueSubject = new();
        private readonly Subject<EnemyStateInformation> _addDownSubject = new();

        public Observable<int> OnStateDequeue => _stateDequeueSubject;
        public Observable<IReadOnlyCollection<EnemyStateInformation>> OnAddStates => _addStatesSubject;
        public Observable<EnemyStateInformation> OnAddDown => _addDownSubject;

        private CancellationTokenSource _cancellationTokenSource;

        public async UniTask Execute()
        {
            if (_cancellationTokenSource != null)
            {
                Debug.LogWarning("StateMachineが既に実行中です。");
                return;
            }

            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
            var ct = _cancellationTokenSource.Token;

            while (true)
            {
                if (ct.IsCancellationRequested)
                {
                    Debug.Log("StateMachineがキャンセルされました。");
                    return;
                }

                if (!_enemyStateQueue.Any())
                {
                    Debug.LogWarning("敵の行動が空です！");

                    var idleState = new IdleState(stateDuration: 0.3f);
                    _enemyStateQueue.Enqueue(idleState);
                }

                _currentState = _enemyStateQueue.Dequeue();
                _stateDequeueSubject.OnNext(_enemyStateQueue.Count);

                await _currentState.Execute(ct);
            }

            // ReSharper disable once FunctionNeverReturns
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;

            _currentState = null;
        }

        public void ClearState()
        {
            _enemyStateQueue.Clear();
        }

        public void AddDownState(EnemyStateInformation stateInformation)
        {
            var state = _enemyStateFactory.CreateDown(stateInformation.Duration);
            _enemyStateQueue.Enqueue(state);
            _addDownSubject.OnNext(stateInformation);
        }

        public void AddStateNoNotify(EnemyStateInformation stateInformation)
        {
            AddState(stateInformation);
        }

        public void AddStateArrayWithNotify(IReadOnlyCollection<EnemyStateInformation> stateInformationArray)
        {
            foreach (var stateInformation in stateInformationArray)
            {
                AddState(stateInformation);
            }

            _addStatesSubject.OnNext(stateInformationArray);
        }

        public void AddStateArrayNoNotify(IReadOnlyCollection<EnemyStateInformation> stateInformationArray)
        {
            foreach (var stateInformation in stateInformationArray)
            {
                AddState(stateInformation);
            }
        }

        private void AddState(EnemyStateInformation stateInformation)
        {
            var state = stateInformation.State switch
            {
                EnemyState.Idle => _enemyStateFactory.CreateIdle(stateInformation.Duration),
                EnemyState.Move => _enemyStateFactory.CreateMove(stateInformation.Duration,
                    stateInformation.TargetPosition),
                EnemyState.Attack => _enemyStateFactory.CreateAttack(
                    stateInformation.Duration,
                    stateInformation.TargetPlayerClientId,
                    shootInterval: 0.5f),
                EnemyState.Down => _enemyStateFactory.CreateDown(stateInformation.Duration),
                _ => throw new System.ArgumentOutOfRangeException()
            };

            _enemyStateQueue.Enqueue(state);
        }
    }
}