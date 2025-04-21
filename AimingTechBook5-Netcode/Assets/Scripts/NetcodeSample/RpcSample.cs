using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class RpcSample : NetworkBehaviour
{
    [SerializeField] private Button serverRpcButton;
    [SerializeField] private Button broadcastRpcButton;

    public override void OnNetworkSpawn()
    {
        Debug.Log($"OwnerClientId: {OwnerClientId}");
        
        serverRpcButton.onClick.AddListener(OnServerRpcButtonClicked);
        broadcastRpcButton.onClick.AddListener(OnBroadcastRpcButtonClicked);
        
        serverRpcButton.interactable = IsClient && !IsHost;
    }

    private void OnServerRpcButtonClicked()
    {
        var msg = $"From Player {OwnerClientId} Message";
        SendMsgToServerRpc(msg);
        Debug.Log($"Send ServerRPC: {msg}");
    }

    private void OnBroadcastRpcButtonClicked()
    {
        var msg = $"From Player{OwnerClientId} Message";
        SendMsgClientsAndHostRpc(msg);
        Debug.Log($"Send BroadCastRPC: {msg}");
    }

    [Rpc(SendTo.Server)] // クライアントからサーバへメッセージを送信
    private void SendMsgToServerRpc(string msg, RpcParams rpcParams = default)
    {
        var senderId = rpcParams.Receive.SenderClientId;
        Debug.Log($"Receive Server RPC: \"{msg}\" (From Client: {senderId})");
    }

    [Rpc(SendTo.ClientsAndHost)] // ホスト含め全クライアントにメッセージを送信
    private void SendMsgClientsAndHostRpc(string msg, RpcParams rpcParams = default)
    {
        var senderId = rpcParams.Receive.SenderClientId;
        Debug.Log($"Receive Broadcast: \"{msg}\" (from Client: {senderId})");
    }
}