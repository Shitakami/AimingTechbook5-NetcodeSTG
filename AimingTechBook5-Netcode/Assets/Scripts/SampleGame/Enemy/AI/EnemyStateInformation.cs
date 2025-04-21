using UnityEngine;

namespace SampleGame.Enemy.AI
{
    public class EnemyStateInformation
    {
        public readonly EnemyState State;
        public readonly int TargetPlayerClientId;
        public readonly Vector3 TargetPosition;
        public readonly float Duration;
        
        private const int InvalidPlayClientId = -1;

        public EnemyStateInformation(
            EnemyState state,
            int targetPlayerClientId, 
            Vector3 targetPosition,
            float duration)
        {
            State = state;
            TargetPlayerClientId = targetPlayerClientId;
            TargetPosition = targetPosition;
            Duration = duration;
        }
        
        public static EnemyStateInformation CreateIdle(float duration)
        {
            return new EnemyStateInformation(
                EnemyState.Idle,
                InvalidPlayClientId,
                Vector3.zero,
                duration);
        }
        
        public static EnemyStateInformation CreateMove(float duration, Vector3 targetPosition)
        {
            return new EnemyStateInformation(
                EnemyState.Move,
                InvalidPlayClientId,
                targetPosition,
                duration);
        }

        public static EnemyStateInformation CreateAttack(float duration, int targetPlayClientId)
        {
            return new EnemyStateInformation(
                EnemyState.Attack,
                targetPlayClientId,
                Vector3.zero,
                duration
            );
        }
        
        public static EnemyStateInformation CreateDown(float duration)
        {
            return new EnemyStateInformation(
                EnemyState.Down,
                InvalidPlayClientId,
                Vector3.zero,
                duration);
        }
    }
}