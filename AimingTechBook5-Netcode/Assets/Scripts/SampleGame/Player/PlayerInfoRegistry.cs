using System.Collections.Generic;
using UnityEngine;

namespace Sample.Player
{
    public class PlayerInfoRegistry : MonoBehaviour
    {
        private readonly Dictionary<ulong, Transform> _playerInfoDict = new();
        
        public void RegisterPlayer(ulong clientId, Transform playerTransform)
        {
            _playerInfoDict.TryAdd(clientId, playerTransform);
            
            Debug.Log($"Player registered: {clientId}");
        }

        public int GetConnectedPlayerCount()
        {
            return _playerInfoDict.Count;
        }

        public bool TryGetPlayerTransform(int targetPlayerId, out Transform playerTransform)
        {
            return _playerInfoDict.TryGetValue((ulong)targetPlayerId, out playerTransform);
        }
    }
}