using System;
using R3;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace SampleGame.Player.State
{
    public class PlayerStateView : MonoBehaviour
    {
        [SerializeField] private Button _readyButton;
        [SerializeField] private TextMeshProUGUI _playerStateText;
        [SerializeField] private PlayerStateManager _playerStateManager;
        [SerializeField] private AutoConnectionAndReadySetting _autoConnectionAndReadySetting;

        private void Start()
        {
            _playerStateManager.OnValueChanged
                .Subscribe(state =>
                {
                    UpdatePlayerStateUI(state);
                    SetReadyButtonInteractable(state == PlayerState.Prepare);
                })
                .AddTo(this);

            _readyButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _playerStateManager.SetPlayerState(PlayerState.Ready);
                })
                .AddTo(this);

            var autoReady = _autoConnectionAndReadySetting?.AutoReady ?? false;
            if (autoReady)
            {
                Observable.FromEvent<ulong>(
                    h => NetworkManager.Singleton.OnClientConnectedCallback += h,
                    h => { if (NetworkManager.Singleton != null) NetworkManager.Singleton.OnClientConnectedCallback -= h; })
                    .Delay(TimeSpan.FromSeconds(1f))
                    .Subscribe(clientId =>
                    {
                        // ローカルプレイヤーが接続された場合に自動的にReadyに設定
                        if (clientId == NetworkManager.Singleton.LocalClientId)
                        {
                            _playerStateManager.SetPlayerState(PlayerState.Ready);
                        }
                    })
                    .AddTo(this);
            }
        }
        
        private void SetReadyButtonInteractable(bool interactable)
        {
            _readyButton.interactable = interactable;
        }
        
        private void UpdatePlayerStateUI(PlayerState state)
        {
            _playerStateText.text = $"{state}";
        }
    }
}
