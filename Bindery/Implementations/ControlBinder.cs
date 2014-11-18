using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Windows.Input;
using Bindery.Implementations.Basic;
using Bindery.Interfaces;

namespace Bindery.Implementations
{
    internal class ControlBinder<TSource, TControl> : BasicControlBinder<TSource, TControl>, IControlBinder<TSource, TControl>
        where TSource : INotifyPropertyChanged
        where TControl : IBindableComponent
    {
        public ControlBinder(SourceBinder<TSource> sourceBinder, TControl control) : base(sourceBinder, control)
        {
        }

        IControlPropertyBinder<TSource, TControl, TProp> IControlBinder<TSource, TControl>.Property<TProp>(Expression<Func<TControl, TProp>> member)
        {
            return new ControlPropertyBinder<TSource, TControl, TProp>(this, member);
        }

        IControlEventBinder<TSource, TControl, EventArgs> IControlBinder<TSource, TControl>.OnEvent(string eventName)
        {
            return new ControlEventBinder<TSource, TControl, EventArgs>(this, eventName);
        }

        IControlEventBinder<TSource, TControl, TEventArgs> IControlBinder<TSource, TControl>.OnEvent<TEventArgs>(string eventName)
        {
            return new ControlEventBinder<TSource, TControl, TEventArgs>(this, eventName);
        }

        public IControlBinder<TSource, TControl> OnClick(ICommand command)
        {
            return OnClick(command, null);
        }

        public IControlBinder<TSource, TControl> OnClick(ICommand command, Func<object> getParameter)
        {
            var control = Control as Control;
            if (control == null)
                throw new NotSupportedException("The control must inherit from System.Windows.Form.Control in order use OnClick()");
            var parameter = getParameter == null ? null : getParameter();
            control.Click += (sender, e) => command.Execute(parameter);
            command.CanExecuteChanged += (sender, e) => control.Enabled = command.CanExecute(parameter);
            control.Enabled = command.CanExecute(parameter);
            return this;
        }

        IControlObservableBinder<TSource, TControl, TArg> IControlBinder<TSource, TControl>.On<TArg>(Func<TControl, IObservable<TArg>> observableMember)
        {
            return new ControlObservableBinder<TSource,TControl,TArg>(this,observableMember(Control));
        }
    }
}