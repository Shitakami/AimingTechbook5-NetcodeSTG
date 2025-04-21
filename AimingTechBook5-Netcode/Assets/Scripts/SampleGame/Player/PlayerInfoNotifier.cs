using Unity.Netcode;
using UnityEngine;

namespace Sample.Player
{
    public class PlayerInfoNotifier : NetworkBehaviour
    {
        public override void OnNetworkSpawn() 
        {
            // Assuming you have a PlayerInfoRegistry component in the scene
            var playerInfoRegistry = FindAnyObjectByType<PlayerInfoRegistry>();
            if (playerInfoRegistry != null)
            {
                var clientId = OwnerClientId;
                playerInfoRegistry.RegisterPlayer(clientId, transform); // Example clientId and transform
            }
            else
            {
                Debug.LogError("PlayerInfoRegistry not found in the scene.");
            }
        }
    }
}
