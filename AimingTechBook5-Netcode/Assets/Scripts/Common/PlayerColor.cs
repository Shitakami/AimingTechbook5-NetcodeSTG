using Unity.Netcode;
using UnityEngine;

public class PlayerColor : NetworkBehaviour
{
    [SerializeField] private Color[] _colors;

    private NetworkVariable<Color> _playerColor = new NetworkVariable<Color>(
        Color.white,
        NetworkVariableReadPermission.Everyone, // 全てのクライアントが読み取り可能
        NetworkVariableWritePermission.Server // サーバのみ書き込み可能
    );

    public override void OnNetworkSpawn()
    { 
        if (IsServer)
        {
            var clientId = OwnerClientId;
            _playerColor.Value = _colors[(int)clientId];
        }
        
        var renderer = GetComponent<Renderer>();
        renderer.material.color = _playerColor.Value;
    }
}
