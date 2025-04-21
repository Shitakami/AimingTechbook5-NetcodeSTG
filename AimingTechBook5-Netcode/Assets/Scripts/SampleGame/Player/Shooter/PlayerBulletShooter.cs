using System;
using R3;
using UnityEngine;

namespace SampleGame.Player.Shooter
{
    public class PlayerBulletShooter : MonoBehaviour
    {
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private float _bulletSpeed = 20f;
        [SerializeField] private float _fireRate = 0.5f;
        [SerializeField] private float _bulletLifetime = 5f;

        private bool IsOwn { get; set; }
        private Subject<Unit> _onShot = new Subject<Unit>();
        
        public Observable<Unit> OnShot => _onShot;
        
        public void Initialize(bool isOwn)
        {
            IsOwn = isOwn;
            if (IsOwn)
            {
                Observable.EveryUpdate()
                    .Where(_ => Input.GetKey(KeyCode.Space))
                    .ThrottleFirst(TimeSpan.FromSeconds(_fireRate))
                    .Subscribe(_ =>
                    {
                        Shoot();
                        _onShot.OnNext(Unit.Default);
                    })
                    .AddTo(this);
            }
        }

        public void Shoot()
        {
            var bullet = Instantiate(_bulletPrefab, _firePoint.position, _firePoint.rotation);
            var bulletComponent = bullet.GetComponent<PlayerBullet>();
            bulletComponent.Initialize(_bulletSpeed, IsOwn, _bulletLifetime);
        }
    }
}