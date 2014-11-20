using System;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Windows.Forms;
using System.Windows.Input;
using Bindery.Interfaces;

namespace Bindery.Implementations
{
    internal class ControlBinder<TSource, TControl> : IControlBinder<TSource, TControl>
        where TControl : IBindableComponent
    {
        public SourceBinder<TSource> SourceBinder { get; private set; }
        public TControl Control { get; private set; }

        public ControlBinder(SourceBinder<TSource> sourceBinder, TControl control)
        {
            SourceBinder = sourceBinder;
            Control = control;
        }

        IControlPropertyBinder<TSource, TControl, TProp> IControlBinder<TSource, TControl>.Property<TProp>(Expression<Func<TControl, TProp>> member)
        {
            return new ControlPropertyBinder<TSource, TControl, TProp>(this, member);
        }

        IObservableBinder<TSource, EventArgs> IControlBinder<TSource, TControl>.OnEvent(string eventName)
        {
            var observable = Create.ObservableFor(Control).Event(eventName);
            return new ObservableBinder<TSource, EventArgs>(SourceBinder, observable);
        }

        IObservableBinder<TSource, TEventArgs> IControlBinder<TSource, TControl>.OnEvent<TEventArgs>(string eventName)
        {
            var observable = Create.ObservableFor(Control).Event<TEventArgs>(eventName);
            return new ObservableBinder<TSource, TEventArgs>(SourceBinder, observable);
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

        public IObservableBinder<TSource, TArg> Observe<TArg>(Func<TControl, IObservable<TArg>> observableMember)
        {
            var observable = observableMember(Control);
            return new ObservableBinder<TSource, TArg>(SourceBinder, observable);
        }

        public TSource Source
        {
            get { return SourceBinder.Source; }
        }

        public void AddDataBinding(Binding binding, ConvertEventHandler formatHandler = null, ConvertEventHandler parseHandler = null)
        {
            if (formatHandler != null)
                binding.Format += formatHandler;
            if (parseHandler != null)
                binding.Parse += parseHandler;
            Control.DataBindings.Add(binding);

            var subscription = Disposable.Create(() => RemoveDataBinding(binding, formatHandler, parseHandler));
            AddSubscription(subscription);
        }

        private void RemoveDataBinding(Binding binding, ConvertEventHandler formatHandler, ConvertEventHandler parseHandler)
        {
            Control.DataBindings.Remove(binding);
            if (formatHandler != null)
                binding.Format -= formatHandler;
            if (parseHandler != null)
                binding.Parse -= parseHandler;
        }

        internal Binding CreateBinding(string controlPropertyName, string sourcePropertyName, ControlUpdateMode controlUpdateMode, DataSourceUpdateMode dataSourceUpdateMode)
        {
            return new Binding(controlPropertyName, Source, sourcePropertyName)
            {
                ControlUpdateMode = controlUpdateMode,
                DataSourceUpdateMode = dataSourceUpdateMode
            };
        }

        public void AddSubscription(IDisposable subscription)
        {
            SourceBinder.AddSubscription(subscription);
        }
    }
}