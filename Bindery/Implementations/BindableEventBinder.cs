using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Windows.Input;
using Bindery.Interfaces;

namespace Bindery.Implementations
{
    internal class BindableEventBinder<TSource, TBindable, TEventArgs, THandler> : IBindableEventBinder<TSource, TBindable>
        where TSource : INotifyPropertyChanged
        where TBindable : IBindableComponent 
    {
        private readonly BindableBinder<TSource, TBindable> _parent;
        private readonly Func<TBindable, Action<THandler>> _getAddHandler;

        public BindableEventBinder(BindableBinder<TSource, TBindable> parent, Func<TBindable, Action<THandler>> getAddHandler)
        {
            _parent = parent;
            _getAddHandler = getAddHandler;
        }

        public IBindableBinder<TSource, TBindable> Triggers(Func<TSource, ICommand> commandMember)
        {
            var command = commandMember(_parent.SourceBinder.Object);
            var addHandler = _getAddHandler(_parent.Bindable);
            var @delegate = CreateExpression(command).Compile();
            addHandler(@delegate);
            return _parent;
        }

        private static Expression<THandler> CreateExpression(ICommand command)
        {
            // Builds:
            // (sender,e)=>
            //   if (command.CanExecute(e))
            //      command.Execute(e);
            Expression<Func<object, bool>> canExecute = arg => command.CanExecute(arg);
            Expression<Action<object>> execute = arg => command.Execute(arg);
            var sender = Expression.Parameter(typeof (object), "sender");
            var e = Expression.Parameter(typeof (TEventArgs), "e");
            var canExecuteIsTrue = Expression.Equal(Expression.Invoke(canExecute,e),Expression.Constant(true));

            var body = Expression.IfThen(canExecuteIsTrue, Expression.Invoke(execute, e));
            return  Expression.Lambda<THandler>(body, sender, e);
        }
    }
}