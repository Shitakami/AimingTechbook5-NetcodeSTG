using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using R3;
using Unity.Netcode;
using UnityEngine;

namespace SampleGame.Enemy.AI
{
    public class EnemyStateSynchronizer : NetworkBehaviour
    {
        [SerializeField] private EnemyStateMachine _enemyStateMachine;
        [SerializeField] private EnemyBrain _enemyBrain;

        // Brain機能を作って、そこで状態を作り共有
        // 自身含め、StateMachineに状態をQueuingする
        public override void OnNetworkSpawn()
        {
            if (IsHost)
            {
                _enemyStateMachine.OnAddDown
                    .Subscribe(enemyStateInformation =>
                    {
                        var networkedEnemyStateInformation = new NetworkedEnemyStateInformation(enemyStateInformation);
                        BroadcastEnemyDownStateClientRpc(networkedEnemyStateInformation);
                    })
                    .AddTo(this);

                _enemyStateMachine.OnAddStates
                    .Subscribe(enemyStates =>
                    {
                        var networkedEnemyStateInformationArray = new NetworkedEnemyStateInformationArray(enemyStates);
                        BroadcastEnemyStateArrayClientRpc(networkedEnemyStateInformationArray);
                    })
                    .AddTo(this);
            }
        }

        [Rpc(SendTo.NotServer)]
        private void BroadcastEnemyStateArrayClientRpc(
            NetworkedEnemyStateInformationArray networkedEnemyStateInformationArray)
        {
            if (IsHost)
            {
                return;
            }

            var enemyStateInformationArray = networkedEnemyStateInformationArray.ToEnemyStateInformations();
            _enemyStateMachine.AddStateArrayNoNotify(enemyStateInformationArray);
        }

        [Rpc(SendTo.NotServer)]
        private void BroadcastEnemyDownStateClientRpc(
            NetworkedEnemyStateInformation networkedEnemyStateInformation)
        {
            if (IsHost)
            {
                return;
            }

            if (_enemyBrain.CompleteDown)
            {
                return;
            }

            _enemyBrain.CompleteDown = true;

            _enemyStateMachine.Stop();
            _enemyStateMachine.ClearState();
            _enemyStateMachine.AddStateNoNotify(networkedEnemyStateInformation.ToEnemyStateInformation());
            _enemyStateMachine.Execute().Forget();
        }

        private struct NetworkedEnemyStateInformation : INetworkSerializable
        {
            public EnemyState State;
            public int TargetClientId;
            public Vector3 TargetPosition;
            public float Duration;

            public NetworkedEnemyStateInformation(EnemyStateInformation enemyStateInformation)
            {
                State = enemyStateInformation.State;
                TargetClientId = enemyStateInformation.TargetPlayerClientId;
                TargetPosition = enemyStateInformation.TargetPosition;
                Duration = enemyStateInformation.Duration;
            }

            public EnemyStateInformation ToEnemyStateInformation()
            {
                return new EnemyStateInformation(
                    State,
                    TargetClientId,
                    TargetPosition,
                    Duration);
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref State);
                serializer.SerializeValue(ref TargetClientId);
                serializer.SerializeValue(ref TargetPosition);
                serializer.SerializeValue(ref Duration);
            }
        }

        private struct NetworkedEnemyStateInformationArray : INetworkSerializable
        {
            public NetworkedEnemyStateInformation[] EnemyStateInformationArray;

            public NetworkedEnemyStateInformationArray(
                IReadOnlyCollection<EnemyStateInformation> enemyStateInformations)
            {
                EnemyStateInformationArray = new NetworkedEnemyStateInformation[enemyStateInformations.Count];
                for (var i = 0; i < enemyStateInformations.Count; i++)
                {
                    EnemyStateInformationArray[i] =
                        new NetworkedEnemyStateInformation(enemyStateInformations.ElementAt(i));
                }
            }

            public EnemyStateInformation[] ToEnemyStateInformations()
            {
                var result = new EnemyStateInformation[EnemyStateInformationArray.Length];
                for (var i = 0; i < EnemyStateInformationArray.Length; i++)
                {
                    result[i] = EnemyStateInformationArray[i].ToEnemyStateInformation();
                }

                return result;
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                var length = 0;
                if (!serializer.IsReader)
                {
                    length = EnemyStateInformationArray?.Length ?? 0;
                }

                serializer.SerializeValue(ref length);

                if (serializer.IsReader)
                {
                    EnemyStateInformationArray = new NetworkedEnemyStateInformation[length];
                }

                for (var i = 0; i < length; i++)
                {
                    serializer.SerializeValue(ref EnemyStateInformationArray[i]);
                }
            }
        }
    }
}