using System.ComponentModel;
using System.Threading;

namespace Bindery.Interfaces.Observables
{
    public interface IOnCompleteDefined<TSource> : ISubscriptionComplete<TSource> 
    {
        ISubscriptionComplete<TSource> CancellationToken(CancellationToken token);
    }
}