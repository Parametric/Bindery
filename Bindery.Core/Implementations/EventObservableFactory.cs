using System;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reflection;
using Bindery.Interfaces;

namespace Bindery.Implementations
{
    internal class EventObservableFactory<T> : IEventObservableFactory
    {
        private readonly T _obj;

        public EventObservableFactory(T obj)
        {
            _obj = obj;
        }

        public IObservable<EventArgs> Event(string eventName)
        {
            return EventAsObservable<EventArgs>(_obj, eventName);
        }

        public IObservable<TEventArgs> Event<TEventArgs>(string eventName)
        {
            return EventAsObservable<TEventArgs>(_obj, eventName);
        }

        private static IObservable<TEventArgs> EventAsObservable<TEventArgs>(T obj, string eventName)
        {
            var memberInfos = typeof (T).GetMember(eventName);
            if (memberInfos.Length == 0)
                throw new ArgumentException(String.Format("'{1}' is not a member of '{0}'.",
                    obj.GetType().FullName, eventName));
            var eventInfo = memberInfos[0] as EventInfo;
            if (eventInfo == null)
                throw new ArgumentException(String.Format("'{0}.{1}' is not an event.",
                    obj.GetType().FullName, eventName));
            return CreateObservable<TEventArgs>(obj, eventInfo);
        }

        private static IObservable<TEventArgs> CreateObservable<TEventArgs>(T control, EventInfo eventInfo)
        {
            var conversion = CreateConversion<TEventArgs>(eventInfo);
            var addHandler = CreateEventHandlerAction(control, eventInfo, eventInfo.GetAddMethod());
            var removeHandler = CreateEventHandlerAction(control, eventInfo, eventInfo.GetRemoveMethod());

            //Observable.FromEvent<THandler,TEventArgs>(Action<TEventArgs>=>THandler,THandler=>void,THandler=>void)
            var call = Expression.Call(typeof (Observable), "FromEvent",
                new[] {eventInfo.EventHandlerType, typeof (TEventArgs)},
                new[] {conversion, addHandler, removeHandler});
            var lambda = Expression.Lambda<Func<IObservable<TEventArgs>>>(call);
            var func = lambda.Compile();
            return func();
        }

        private static Expression CreateConversion<TEventArgs>(EventInfo eventInfo)
        {
            // Builds:
            // func(argAction=>(sender,e)=>argAction(e))
            // where argAction is Action<TEventArgs> and (sender,e)=>action(e) has event handler signature
            var argAction = Expression.Parameter(typeof (Action<TEventArgs>), "argAction");
            var sender = Expression.Parameter(typeof (object), "sender");
            var e = Expression.Parameter(typeof (TEventArgs), "e");
            var argActionInvoke = Expression.Invoke(argAction, e);
            var eventHandler = Expression.Lambda(eventInfo.EventHandlerType, argActionInvoke, sender, e);
            var funcType = Expression.GetFuncType(typeof (Action<TEventArgs>), eventInfo.EventHandlerType);
            return Expression.Lambda(funcType, eventHandler, argAction);
        }

        private static Expression CreateEventHandlerAction(T control, EventInfo eventInfo, MethodInfo eventMethod)
        {
            // Builds:
            //  handler=>event.Add(handler)
            var handler = Expression.Parameter(eventInfo.EventHandlerType, "handler");
            var target = Expression.Constant(control);
            var callAdd = Expression.Call(target, eventMethod, new Expression[] {handler});
            var actionType = Expression.GetActionType(eventInfo.EventHandlerType);
            return Expression.Lambda(actionType, callAdd, handler);
        }
    }
}