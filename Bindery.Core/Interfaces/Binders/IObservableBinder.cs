using System;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Windows.Input;
using Bindery.Interfaces.Subscriptions;

namespace Bindery.Interfaces.Binders
{
    /// <summary>
    ///     Binder for subscribing to an observable
    /// </summary>
    /// <typeparam name="TSource">The type of the source for the parent binder</typeparam>
    /// <typeparam name="TArg">The type of the observable</typeparam>
    public interface IObservableBinder<TSource, TArg>
    {
        /// <summary>
        ///     Subscribe to the observable
        /// </summary>
        /// <param name="action">An action to take on IObservable.OnNext</param>
        /// <returns>The parent source binder</returns>
        ISourceBinder<TSource> Subscribe(Action<TArg> action);

        /// <summary>
        ///     Subscriber to the observable
        /// </summary>
        /// <param name="subscription">A function that receives a subscription context</param>
        /// <returns>The parent source binder</returns>
        /// <remarks>
        ///     This method overload allows full definition of OnNext, OnError, OnComplete, and CancellationToken for the
        ///     subscription
        /// </remarks>
        ISourceBinder<TSource> Subscribe(Func<ISubscriptionContext<TArg>, ISubscriptionComplete> subscription);

        /// <summary>
        ///     Create a subscription that executes a command
        /// </summary>
        /// <param name="command">The command</param>
        /// <returns>The parent source binder</returns>
        /// <remarks>The value of the observable is passed as the parameter to ICommand.Execute</remarks>
        ISourceBinder<TSource> Execute(ICommand command);

        /// <summary>
        ///     Create a subscription that executes a command
        /// </summary>
        /// <param name="command">The command</param>
        /// <param name="commandParameter">The parameter passed to ICommand.Execute</param>
        /// <returns>The parent source binder</returns>
        ISourceBinder<TSource> Execute(ICommand command, object commandParameter);

        /// <summary>
        ///     Create a subscription that executes a command
        /// </summary>
        /// <param name="command">The command</param>
        /// <param name="getCommandParameter">Function to get the command parameter, which is evaluated at command execution time</param>
        /// <returns>The parent source binder</returns>
        ISourceBinder<TSource> Execute(ICommand command, Func<object> getCommandParameter);

        /// <summary>
        ///     Create a subscription that sets a source member with the observable's value
        /// </summary>
        /// <param name="member">An expression that defines the source member</param>
        /// <returns></returns>
        ISourceBinder<TSource> Set(Expression<Func<TSource, TArg>> member);

        /// <summary>
        ///     Transform the observable
        /// </summary>
        /// <typeparam name="TOut">The type of the resulting observable</typeparam>
        /// <param name="transform">A function that transforms (filters, maps, reduces) the observable</param>
        /// <returns>An observable binder for the transformed observable</returns>
        IObservableBinder<TSource, TOut> Transform<TOut>(Func<IObservable<TArg>, IObservable<TOut>> transform);

        /// <summary>
        ///     Override the default action scheduler used by the observable
        /// </summary>
        /// <param name="scheduler">The scheduler</param>
        /// <returns>This binder</returns>
        IObservableBinder<TSource, TArg> ObserveOn(IScheduler scheduler);
    }
}