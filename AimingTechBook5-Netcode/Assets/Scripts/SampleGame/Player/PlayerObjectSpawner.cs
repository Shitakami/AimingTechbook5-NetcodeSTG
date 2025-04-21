using Unity.Netcode;
using UnityEngine;

namespace SampleGame.Player
{
    public class PlayerObjectSpawner : NetworkBehaviour
    {
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private Transform[] _spawnPoints;

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                // サーバーのみ実行する処理
                NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayerForClient;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= SpawnPlayerForClient;
            }
        }

        private void SpawnPlayerForClient(ulong clientId)
        {
            // プレイヤープレハブをインスタンス化
            var playerInstance = Instantiate(_playerPrefab);

            // スポーン位置を決定
            var spawnPoint = GetSpawnPoint(clientId);
            playerInstance.transform.position = spawnPoint.position;
            playerInstance.transform.rotation = spawnPoint.rotation;

            // ネットワークオブジェクトとしてスポーン
            var networkObject = playerInstance.GetComponent<NetworkObject>();
            networkObject.SpawnAsPlayerObject(clientId);
        }

        private Transform GetSpawnPoint(ulong clientId)
        {
            // スポーンポイントが設定されていない場合
            if (_spawnPoints == null || _spawnPoints.Length == 0)
            {
                return transform;
            }

            // クライアントIDに基づいてスポーンポイントを選択
            int index = (int)(clientId % (ulong)_spawnPoints.Length);
            return _spawnPoints[index];
        }
    }
}