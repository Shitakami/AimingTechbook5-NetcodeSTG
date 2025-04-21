using R3;
using Unity.Netcode;
using UnityEngine;

namespace SampleGame.Player.Shooter
{
    public class NetworkBulletShooter : NetworkBehaviour
    {
        [SerializeField] private PlayerBulletShooter playerBulletShooter;

        public override void OnNetworkSpawn()
        {
            playerBulletShooter.Initialize(IsOwner);
            
            playerBulletShooter.OnShot
                .Subscribe(_ => ShootNotMeRpc())
                .AddTo(this);
        }

        [Rpc(SendTo.NotMe)]
        private void ShootNotMeRpc()
        {
            playerBulletShooter.Shoot();
        }
    }
}