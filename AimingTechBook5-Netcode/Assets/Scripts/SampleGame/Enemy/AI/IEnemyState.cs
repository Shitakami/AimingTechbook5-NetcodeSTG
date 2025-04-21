using System.Threading;
using Cysharp.Threading.Tasks;

namespace SampleGame.Enemy.AI
{
    public interface IEnemyState
    {
        public UniTask Execute(CancellationToken ct);
    }
}