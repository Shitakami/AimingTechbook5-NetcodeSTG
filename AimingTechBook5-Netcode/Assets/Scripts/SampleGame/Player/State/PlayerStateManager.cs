using R3;
using UnityEngine;

namespace SampleGame.Player.State
{
    public class PlayerStateManager : MonoBehaviour
    {
        private readonly ReactiveProperty<PlayerState> _value = new(PlayerState.None);
        
        public Observable<PlayerState> OnValueChanged => _value;

        private void Start()
        {
            _value.Value = PlayerState.Prepare;
        }
        
        public void SetPlayerState(PlayerState state)
        {
            _value.Value = state;
        }
    }
}
