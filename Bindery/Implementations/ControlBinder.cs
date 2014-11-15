using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Windows.Input;
using Bindery.Interfaces;

namespace Bindery.Implementations
{
    internal class ControlBinder<TSource, TControl> : IControlBinder<TSource, TControl> 
        where TSource : INotifyPropertyChanged
        where TControl : IBindableComponent
    {
        public SourceBinder<TSource> SourceBinder { get; private set; }
        public TControl Control { get; private set; }

        public ControlBinder(SourceBinder<TSource> sourceBinder, TControl control)
        {
            SourceBinder = sourceBinder;
            Control = control;
        }

        public IControlPropertyBinder<TSource, TControl, TProp> Property<TProp>(Expression<Func<TControl, TProp>> member)
        {
            return new ControlPropertyBinder<TSource, TControl, TProp>(this, member);
        }

        public IControlEventBinder<TSource, TControl> Event<TEventArgs>(string eventName)
        {
            return new ControlEventBinder<TSource, TControl, TEventArgs>(this, eventName);
        }

        public IControlEventBinder<TSource, TControl> Event(string eventName)
        {
            return new ControlEventBinder<TSource, TControl, EventArgs>(this, eventName);
        }

        public IControlBinder<TSource, TControl> OnClick(Func<TSource, ICommand> commandMember)
        {
            var control = Control as Control;
            if (control==null)
                throw new NotSupportedException("The control must inherit from System.Windows.Form.Control in order use OnClick()");
            var command = commandMember(SourceBinder.Object);
            control.Click += (sender, e) => command.Execute(null);
            command.CanExecuteChanged += (sender, e) => control.Enabled = command.CanExecute(null);
            return this;
        }
        

        public void AddDataBinding(Binding binding)
        {
            Control.DataBindings.Add(binding);
        }

        internal Binding CreateBinding(string controlPropertyName, string sourcePropertyName, ControlUpdateMode controlUpdateMode, DataSourceUpdateMode dataSourceUpdateMode)
        {
            return new Binding(controlPropertyName, SourceBinder.Object, sourcePropertyName)
            {
                ControlUpdateMode = controlUpdateMode,
                DataSourceUpdateMode = dataSourceUpdateMode
            };
        }
    }
}