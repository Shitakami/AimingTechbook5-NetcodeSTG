using Cysharp.Threading.Tasks;
using R3;
using Unity.Netcode;
using UnityEngine;

namespace SampleGame.Enemy
{
    public class EnemyHp : NetworkBehaviour
    {
        [SerializeField] private int _maxHealth = 100;

        private AnticipatedNetworkVariable<int> _currentHealth = new();

        public Observable<int> OnHealthChangedObservable => _currentHealth
            .ObserveChanged()
            .Select(tuple => tuple.newValue)
            .Merge(_healthChangedSubject)
            .Prepend(_currentHealth.Value);

        public Observable<int> OnAnticipatedHealthChangedObservable => _healthChangedSubject;

        public int MaxHealth => _maxHealth;
        private readonly Subject<int> _healthChangedSubject = new Subject<int>();

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                _currentHealth.AuthoritativeValue = _maxHealth;
            }

            OnHealthChangedObservable
                .Subscribe(updateHealth => OnHealthChanged(updateHealth))
                .AddTo(this);
        }

        private void Update()
        {
            // AnticipatedNetworkVariableの権威値を更新
            _currentHealth.Update();
        }

        // クライアントからサーバーへのダメージ通知用RPC
        [Rpc(SendTo.Server)]
        public void NotifyDamageServerRpc(int damage)
        {
            int newHealth = Mathf.Max(0, _currentHealth.AuthoritativeValue - damage);
            _currentHealth.AuthoritativeValue = newHealth;
        }

        public void DamageImmediate(int damage)
        {
            if (IsServer)
            {
                return;
            }

            // クライアントで予測値を設定
            var newValue = Mathf.Max(0, _currentHealth.Value - damage);
            _currentHealth.Anticipate(newValue);
            _healthChangedSubject.OnNext(newValue);
        }

        private void OnHealthChanged(int health)
        {
            if (health == 0)
            {
                // MEMO: すぐにオブジェクトを破棄した場合に、HPが0になったことを通知できない
                // そこでHPが0になった場合は、オブジェクトを非表示にして疑似的に破棄を再現
                gameObject.SetActive(false);

                // オブジェクトを破棄する
                if (IsHost)
                {
                    // ホストの場合、オブジェクトを破棄
                    DelayDespawn().Forget();
                }
            }
        }

        private async UniTask DelayDespawn()
        {
            if (IsHost)
            {
                await UniTask.Delay(3000, cancellationToken: destroyCancellationToken);

                // ホストの場合、オブジェクトを破棄
                NetworkObject.Despawn();
            }
        }
    }
}