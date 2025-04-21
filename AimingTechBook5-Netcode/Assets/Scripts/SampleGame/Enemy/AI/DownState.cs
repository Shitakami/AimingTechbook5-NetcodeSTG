using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SampleGame.Enemy.AI
{
    public class DownState : IEnemyState
    {
        public EnemyState State => EnemyState.Down;
        private readonly Transform _enemyTransform;
        private readonly float _stateDuration;
        
        public EnemyStateInformation StateInformation => 
            EnemyStateInformation.CreateDown(_stateDuration);

        public DownState(
            EnemyStateInformation enemyStateInformation,
            Transform enemyTransform)
        {
            _enemyTransform = enemyTransform;
            _stateDuration = enemyStateInformation.Duration;
        }
        
        public DownState(
            float stateDuration,
            Transform enemyTransform)
        {
            _enemyTransform = enemyTransform;
            _stateDuration = stateDuration;
        }
        
        public async UniTask Execute(CancellationToken ct)
        {
            // ダウン開始をログに記録
            Debug.Log("敵がダウンしました！");

            // ダウン効果のパラメータ設定
            var startTime = UnityEngine.Time.time;
            
            // 元の回転とスケールを保存
            var originalRotation = _enemyTransform.rotation;
            var originalScale = _enemyTransform.localScale;
            
            // ダウン効果の実装
            while (Time.time - startTime < _stateDuration)
            {
                if (ct.IsCancellationRequested) return;

                // 経過時間の割合を計算（0～1）
                float elapsedRatio = (UnityEngine.Time.time - startTime) / _stateDuration;
                
                // 振動効果（徐々に小さくなる）
                float shake = Mathf.Sin(elapsedRatio * 30) * (1 - elapsedRatio) * 0.2f;
                
                // 回転効果（左右に揺れる）
                _enemyTransform.rotation = originalRotation * Quaternion.Euler(0, shake * 30, 0);
                
                // スケール効果（縮んでから戻る）
                float scaleEffect = 1f - (0.3f * (1 - elapsedRatio));
                _enemyTransform.localScale = originalScale * scaleEffect;
                
                await UniTask.Yield();
            }

            // ダウン終了後、元の回転とスケールに戻す
            _enemyTransform.rotation = originalRotation;
            _enemyTransform.localScale = originalScale;
        }
    }
}