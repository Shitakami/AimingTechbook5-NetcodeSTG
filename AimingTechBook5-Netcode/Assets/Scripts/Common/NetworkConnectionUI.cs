using System.Linq;
using SampleGame;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Common
{
    public class NetworkConnectionUI : MonoBehaviour
    {
        [SerializeField] private GameObject uiRoot;
        [SerializeField] private Button hostButton;
        [SerializeField] private Button clientButton;

        [SerializeField] private AutoConnectionAndReadySetting autoConnectionAndReadySetting;
        
        private void Start()
        {
            var autoConnect = autoConnectionAndReadySetting?.AutoConnect ?? false;
            
            if (autoConnect)
            {
                var multiPlayerTag = Unity.Multiplayer.Playmode.CurrentPlayer.ReadOnlyTags();
                var isHost = multiPlayerTag.FirstOrDefault(playerTag => playerTag == "HostPlayer");
                var isClient = multiPlayerTag.FirstOrDefault(playerTag => playerTag == "ClientPlayer");
                
                if (isHost != null)
                {
                    NetworkManager.Singleton.StartHost();
                    uiRoot.SetActive(false);
                }
                else if (isClient != null)
                {
                    NetworkManager.Singleton.StartClient();
                    uiRoot.SetActive(false);
                }
            }
            else
            {
                uiRoot.SetActive(true);
            }

            hostButton.onClick.AddListener(StartHost);
            clientButton.onClick.AddListener(StartClient);
        }

        private void StartHost()
        {
            NetworkManager.Singleton.StartHost();
            uiRoot.SetActive(false);
        }

        private void StartClient()
        {
            NetworkManager.Singleton.StartClient();
            uiRoot.SetActive(false);
        }
    }
}