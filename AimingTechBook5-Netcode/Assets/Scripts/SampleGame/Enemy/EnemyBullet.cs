using UnityEngine;

namespace SampleGame.Enemy
{
    public class EnemyBullet : MonoBehaviour
    {
        [SerializeField] private float _velocity;
        [SerializeField] private float _lifeTime;

        private void Start()
        {
            Destroy(gameObject, _lifeTime);
        }

        private void Update()
        {
            transform.position += transform.forward * _velocity * Time.deltaTime;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                // プレイヤーにダメージを与える処理をここに追加
                Debug.Log("Hit Player!");
                
                // 弾を破棄
                Destroy(gameObject);
            }
        }
    }
}