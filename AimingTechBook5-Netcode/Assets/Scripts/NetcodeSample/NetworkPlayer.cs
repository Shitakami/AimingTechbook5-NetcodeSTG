using Unity.Netcode;
using UnityEngine;

namespace NetcodeSample
{
    public class NetworkPlayer : NetworkBehaviour
    {
        // 移動速度
        [SerializeField] private float moveSpeed = 5.0f;

        void Update()
        {
            if (IsOwner) // ローカルプレイヤーかつ所有者の場合のみ入力を取得
            {
                // 入力を取得
                float horizontal = Input.GetAxis("Horizontal");
                float vertical = Input.GetAxis("Vertical");
                var moveInput = new Vector2(horizontal, vertical);

                // 入力がある場合のみサーバーに送信
                if (moveInput != Vector2.zero)
                {
                    SendInputToServerRpc(moveInput);
                }
            }
        }

        [Rpc(SendTo.Server)]
        private void SendInputToServerRpc(Vector2 input)
        {
            // サーバー側で移動を計算して適用
            var movement = new Vector3(input.x, 0, input.y) * moveSpeed * Time.deltaTime;
            transform.position += movement;
        }
    }
}