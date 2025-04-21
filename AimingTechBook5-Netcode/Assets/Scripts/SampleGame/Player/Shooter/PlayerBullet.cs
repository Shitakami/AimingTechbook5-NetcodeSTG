using SampleGame.Enemy;
using UnityEngine;

namespace SampleGame.Player.Shooter
{
    public class PlayerBullet : MonoBehaviour
    {
        [SerializeField] private int _damage = 10;

        private float _speed;

        private bool IsOwn { get; set; }

        public void Initialize(float speed, bool isOwn, float lifetime)
        {
            _speed = speed;
            IsOwn = isOwn;

            // 一定時間後に破棄
            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            // 全クライアントで同じ動きをする
            transform.Translate(Vector3.forward * _speed * Time.deltaTime);
        }

        // TODO: 敵側にHit処理を定義した方が分かりやすそう💭
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<EnemyHp>(out var enemy))
            {
                // 自分が所有する弾の場合のみサーバーに通知
                if (IsOwn)
                {
                    enemy.NotifyDamageServerRpc(_damage);

                    // クラ側で即時ダメージ処理
                    enemy.DamageImmediate(_damage);
                }

                Destroy(gameObject);
            }
        }
    }
}