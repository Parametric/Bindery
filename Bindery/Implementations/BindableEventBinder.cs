using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Input;
using Bindery.Interfaces;

namespace Bindery.Implementations
{
    internal class BindableEventBinder<TSource, TBindable, TEventArgs> : IBindableEventBinder<TSource, TBindable>
        where TSource : INotifyPropertyChanged
        where TBindable : IBindableComponent 
    {
        private readonly BindableBinder<TSource, TBindable> _parent;
        private readonly IObservable<TEventArgs> _observable;

        public BindableEventBinder(BindableBinder<TSource, TBindable> parent, string eventName)
        {
            _parent = parent;
            var memberInfos = typeof (TBindable).GetMember(eventName);
            if (memberInfos.Length == 0)
                throw new ArgumentException(string.Format("{1} is not a member of {0}.", 
                    parent.Bindable.GetType().Name, eventName));
            var eventInfo = memberInfos[0] as EventInfo;
            if (eventInfo==null)
                throw new ArgumentException(string.Format("{0}.{1} is not an event.",
                    parent.Bindable.GetType().Name, eventName));
            _observable = CreateObservable(eventInfo);
        }

        private IObservable<TEventArgs> CreateObservable(EventInfo eventInfo)
        {
            var conversion = CreateConversion(eventInfo);
            var addHandler = CreateEventHandlerAction(eventInfo, eventInfo.GetAddMethod());
            var removeHandler = CreateEventHandlerAction(eventInfo, eventInfo.GetRemoveMethod());

            //Observable.FromEvent<THandler,TEventArgs>(Action<TEventArgs>=>THandler,THandler=>void,THandler=>void)
            var call = Expression.Call(typeof(Observable), "FromEvent",
                new[] {eventInfo.EventHandlerType, typeof (TEventArgs)},
                new[] {conversion, addHandler, removeHandler});
            var lambda = Expression.Lambda<Func<IObservable<TEventArgs>>>(call);
            var func = lambda.Compile();
            return func();
        }

        public IBindableBinder<TSource, TBindable> Triggers(Func<TSource, ICommand> commandMember)
        {
            var command = commandMember(_parent.SourceBinder.Object);
            var subscription = _observable.Subscribe(x =>
            {
                if (command.CanExecute(null))
                    command.Execute(null);
            });
            _parent.SourceBinder.AddSubscription(subscription);
            return _parent;
        }

        private static Expression CreateConversion(EventInfo eventInfo)
        {
            // Builds:
            // func(argAction=>(sender,e)=>argAction(e))
            // where argAction is Action<TEventArgs> and (sender,e)=>action(e) has event handler signature
            var argAction = Expression.Parameter(typeof (Action<TEventArgs>), "argAction");
            var sender = Expression.Parameter(typeof(object), "sender");
            var e = Expression.Parameter(typeof(TEventArgs), "e");
            var argActionInvoke = Expression.Invoke(argAction, e);
            var eventHandler =  Expression.Lambda(eventInfo.EventHandlerType, argActionInvoke, sender, e);
            var funcType = Expression.GetFuncType(typeof (Action<TEventArgs>), eventInfo.EventHandlerType);
            return Expression.Lambda(funcType, eventHandler, argAction);
        }

        private Expression CreateEventHandlerAction(EventInfo eventInfo, MethodInfo eventMethod)
        {
            // Builds:
            //  handler=>event.Add(handler)
            var handler = Expression.Parameter(eventInfo.EventHandlerType, "handler");
            var target = Expression.Constant(_parent.Bindable);
            var callAdd = Expression.Call(target, eventMethod, new Expression[] { handler });
            var actionType = Expression.GetActionType(eventInfo.EventHandlerType);
            return Expression.Lambda(actionType, callAdd, handler);
        }
    }
}