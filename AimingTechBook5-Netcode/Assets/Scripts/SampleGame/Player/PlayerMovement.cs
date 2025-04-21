using Unity.Netcode;
using UnityEngine;

namespace SampleGame.Player
{
    public class PlayerMovement : NetworkBehaviour
    {
        [SerializeField] private float _speed = 5f;
        [SerializeField] private Transform _cameraTransform;

        // マウス感度設定
        [SerializeField] private float _mouseSensitivity = 2f;

        private void Start()
        {
            if (!IsOwner) return;

            var mainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
            mainCamera.SetPositionAndRotation(_cameraTransform.position, _cameraTransform.rotation);
            mainCamera.parent = _cameraTransform;
        }

        private void Update()
        {
            if (!IsOwner) return;

            // プレイヤーの回転（マウスの水平方向入力による）
            RotatePlayer();

            // プレイヤーの移動（キーボード入力による）
            MovePlayer();
        }

        private void RotatePlayer()
        {
            // マウスの水平方向の入力を取得
            float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;

            // プレイヤー全体を水平方向に回転
            transform.Rotate(0, mouseX, 0);
        }

        private void MovePlayer()
        {
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");

            // プレイヤーのローカル座標系で移動方向を計算
            Vector3 movement = transform.forward * vertical + transform.right * horizontal;

            // 移動ベクトルが長さ1を超えないようにする（斜め移動が速くなるのを防止）
            if (movement.magnitude > 1f)
            {
                movement.Normalize();
            }

            // 移動を適用
            transform.position += movement * (_speed * Time.deltaTime);
        }
    }
}