using System;

namespace Bindery.Interfaces.Subscriptions
{
    public interface ISubscriptionContext<out TArg>
    {
        IOnNextDefined OnNext(Action<TArg> action);
    }
}