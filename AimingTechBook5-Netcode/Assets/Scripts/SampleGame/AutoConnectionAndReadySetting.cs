using UnityEngine;

namespace SampleGame
{
    [CreateAssetMenu(fileName = "AutoConnectionAndReadySetting", menuName = "SampleGame/AutoConnectionAndReadySetting", order = 0)]
    public class AutoConnectionAndReadySetting : ScriptableObject
    {
        [SerializeField] private bool _autoConnect;
        [SerializeField] private bool _autoReady;
        
        public bool AutoConnect => _autoConnect;
        public bool AutoReady => _autoReady && _autoConnect;
    }
}