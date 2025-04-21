using System.Collections.Generic;
using ObservableCollections;
using UnityEngine;

namespace SampleGame.Player.State
{
    public class AllPlayerStates : MonoBehaviour
    {
        private readonly ObservableDictionary<ulong, PlayerState> _playerStates = new();
        
        public IReadOnlyObservableDictionary<ulong, PlayerState> PlayerStates => _playerStates;
        public IReadOnlyCollection<KeyValuePair<ulong, PlayerState>> KeyValuePairs => _playerStates;
        
        public bool Contains(ulong clientId)
        {
            return _playerStates.ContainsKey(clientId);
        }
        
        public void Remove(ulong clientId)
        {
            _playerStates.Remove(clientId);
        }
        
        public void UpdatePlayerState(ulong clientId, PlayerState state)
        {
            _playerStates[clientId] = state;
        }
        
        public bool IsAllPlayerReady()
        {
            foreach (var state in _playerStates)
            {
                var (_, value) = state;
                if (value != PlayerState.Ready)
                {
                    return false;
                }
            }

            return true;
        }
    }
}