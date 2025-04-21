using R3;
using Unity.Netcode;

namespace SampleGame
{
    public static class AnticipatedNetworkVariableExtensions
    {
        public static Observable<(T previousValue, T newValue)> ObserveChanged<T>(this AnticipatedNetworkVariable<T> source)
        {
            return Observable.Create<(T previousValue, T newValue)>(observer =>
            {
                source.OnAuthoritativeValueChanged += Handler;
                
                return Disposable.Create(() =>
                {
                    source.OnAuthoritativeValueChanged -= Handler;
                });

                void Handler(AnticipatedNetworkVariable<T> sender, in T previousValue, in T newValue)
                {
                    observer.OnNext((previousValue, newValue));
                }
            });
        }
    }
}