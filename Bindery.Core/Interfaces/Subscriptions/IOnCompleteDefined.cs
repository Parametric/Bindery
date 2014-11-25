using System.Threading;

namespace Bindery.Interfaces.Subscriptions
{
    public interface IOnCompleteDefined : ISubscriptionComplete
    {
        ISubscriptionComplete CancellationToken(CancellationToken token);
    }
}