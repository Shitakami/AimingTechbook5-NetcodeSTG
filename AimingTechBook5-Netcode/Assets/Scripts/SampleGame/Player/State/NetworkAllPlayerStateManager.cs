using System.Linq;
using Unity.Netcode;
using UnityEngine;
using R3;

namespace SampleGame.Player.State
{
    public class NetworkAllPlayerStateManager : NetworkBehaviour
    {
        [SerializeField] private PlayerStateManager _playerStateManager;
        [SerializeField] private AllPlayerStates _allPlayerStates;

        public override void OnNetworkSpawn()
        {
            if (IsHost)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += SendAllPlayerStatesToClient;
            }
            
            var ownClientId = NetworkManager.Singleton.LocalClientId;
            
            _playerStateManager.OnValueChanged
                .Subscribe(playerState => UpdatePlayerStateClientsAndHostRpc(ownClientId, playerState))
                .AddTo(this);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void UpdatePlayerStateClientsAndHostRpc(ulong clientId, PlayerState state)
        {
            Debug.Log($"Client {clientId} PlayerState: {state}");
            _allPlayerStates.UpdatePlayerState(clientId, state);
        }

        private void SendAllPlayerStatesToClient(ulong clientId)
        {
            var currentPlayerStates = _allPlayerStates.KeyValuePairs
                .Select(kvp => new PlayerStateData { ClientId = kvp.Key, State = kvp.Value })
                .ToArray();

            SendAllPlayerStatesToClientRpc(clientId, currentPlayerStates);
        }

        [Rpc(SendTo.NotServer)]
        private void SendAllPlayerStatesToClientRpc(ulong targetClientId, PlayerStateData[] playerStates,
            RpcParams rpcParams = default)
        {
            // 自分宛のデータのみ処理する
            if (NetworkManager.LocalClientId == targetClientId)
            {
                foreach (var data in playerStates)
                {
                    _allPlayerStates.UpdatePlayerState(data.ClientId, data.State);
                    Debug.Log($"受信: クライアント {data.ClientId} の状態: {data.State}");
                }
            }
        }

        private struct PlayerStateData : INetworkSerializable
        {
            public ulong ClientId;
            public PlayerState State;

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref ClientId);
                serializer.SerializeValue(ref State);
            }
        }
    }
}