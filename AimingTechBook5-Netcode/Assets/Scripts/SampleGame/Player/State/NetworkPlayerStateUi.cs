using System.Text;
using ObservableCollections;
using TMPro;
using UnityEngine;
using R3;

namespace SampleGame.Player.State
{
    public class AllPlayerStatesUI : MonoBehaviour
    {
        [SerializeField] private AllPlayerStates _allPlayerStates;
        [SerializeField] private TextMeshProUGUI _playerStatesText;

        private readonly StringBuilder _stringBuilder = new StringBuilder();
        private CompositeDisposable _disposables = new CompositeDisposable();

        private void Start()
        {
            var playerStates = _allPlayerStates.PlayerStates;

            // 各種イベントを直接購読
            playerStates.ObserveAdd()
                .Subscribe(_ => UpdateUI())
                .AddTo(_disposables);

            playerStates.ObserveReplace()
                .Subscribe(_ => UpdateUI())
                .AddTo(_disposables);

            playerStates.ObserveRemove()
                .Subscribe(_ => UpdateUI())
                .AddTo(_disposables);

            playerStates.ObserveReset()
                .Subscribe(_ => UpdateUI())
                .AddTo(_disposables);

            // 初期表示
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (_playerStatesText == null) return;

            _stringBuilder.Clear();

            var states = _allPlayerStates.KeyValuePairs;
            foreach (var kvp in states)
            {
                _stringBuilder.AppendLine($"ClientID: {kvp.Key} - {kvp.Value}");
            }

            _playerStatesText.text = _stringBuilder.ToString();
        }

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}