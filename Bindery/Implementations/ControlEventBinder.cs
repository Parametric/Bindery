using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Input;
using Bindery.Extensions;
using Bindery.Interfaces;

namespace Bindery.Implementations
{
    internal class ControlEventBinder<TSource, TControl, TEventArgs> : IControlEventBinder<TSource, TControl, TEventArgs>
        where TSource : INotifyPropertyChanged
        where TControl : IBindableComponent 
    {
        private readonly ControlBinder<TSource, TControl> _parent;
        private readonly IObservable<TEventArgs> _observable;

        public ControlEventBinder(ControlBinder<TSource, TControl> parent, string eventName)
        {
            _parent = parent;
            var memberInfos = typeof (TControl).GetMember(eventName);
            if (memberInfos.Length == 0)
                throw new ArgumentException(string.Format("'{1}' is not a member of '{0}'.", 
                    parent.Control.GetType().FullName, eventName));
            var eventInfo = memberInfos[0] as EventInfo;
            if (eventInfo==null)
                throw new ArgumentException(string.Format("'{0}.{1}' is not an event.",
                    parent.Control.GetType().FullName, eventName));
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

        public IControlBinder<TSource, TControl> Executes(Func<TSource, ICommand> commandMember)
        {
            var command = commandMember(_parent.Source);
            var subscription = _observable.Subscribe(x => command.ExecuteIfValid(default(TEventArgs)));
            _parent.AddSubscription(subscription);
            return _parent;
        }

        public IControlObservableBinder<TSource, TControl, TConverted> ConvertArgsTo<TConverted>(Func<TEventArgs, TConverted> conversion)
        {
            return new ControlObservableConversionBinder<TSource, TControl, TEventArgs, TConverted>(_parent, _observable, conversion);
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
            var target = Expression.Constant(_parent.Control);
            var callAdd = Expression.Call(target, eventMethod, new Expression[] { handler });
            var actionType = Expression.GetActionType(eventInfo.EventHandlerType);
            return Expression.Lambda(actionType, callAdd, handler);
        }
    }
}