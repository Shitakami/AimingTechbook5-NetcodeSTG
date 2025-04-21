using ObservableCollections;
using Unity.Netcode;
using UnityEngine;
using R3;
using SampleGame.Player.State;

namespace SampleGame.Enemy
{
    public class EnemySpawnManager : NetworkBehaviour
    {
        [SerializeField] private GameObject _enemyPrefab;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private AllPlayerStates _playerStates;
        
        public override void OnNetworkSpawn()
        {
            if (IsHost)
            {
                // プレイヤーの状態を監視し、全員が準備完了したら敵をスポーンする
                _playerStates.PlayerStates.ObserveChanged()
                    .Where(_ => _playerStates.IsAllPlayerReady())
                    .Subscribe(_ => SpawnEnemy())
                    .AddTo(this);
            }
        }

        private void SpawnEnemy()
        {
            // 敵をスポーンするロジック
            var enemy = Instantiate(_enemyPrefab);
            
            enemy.transform.position = _spawnPoint.position;
            enemy.transform.rotation = _spawnPoint.rotation;

            enemy.GetComponent<NetworkObject>().Spawn();
        }
    }
}