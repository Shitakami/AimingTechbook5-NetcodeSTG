using R3;
using Unity.Netcode;
using UnityEngine;

namespace SampleGame.Enemy
{
    public class SubscribeEnemyUI : NetworkBehaviour
    {
        [SerializeField] private EnemyHp enemyHp;
        
        public override void OnNetworkSpawn()
        {
            var enemyHpUI = FindAnyObjectByType<EnemyHpUI>();
            
            if (enemyHpUI == null)
            {
                Debug.LogError("EnemyHpUI not found in the scene.");
                return;
            }
            
            enemyHp.OnHealthChangedObservable
                .Subscribe(health => enemyHpUI.UpdateHpUI(health, enemyHp.MaxHealth))
                .AddTo(this);
        }
    }
}