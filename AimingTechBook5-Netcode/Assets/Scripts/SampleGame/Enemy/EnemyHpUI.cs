using TMPro;
using UnityEngine;

namespace SampleGame.Enemy
{
    public class EnemyHpUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        public void UpdateHpUI(int currentHp, int maxHp)
        {
            // テキストを更新
            _text.text = $"{currentHp}/{maxHp}";
        }
    }
}