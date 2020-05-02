using System;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Windows.Forms;
using System.Windows.Input;
using Bindery.Extensions;
using Bindery.Interfaces;
using Bindery.Interfaces.Binders;

namespace Bindery.Implementations
{
    internal class ControlBinder<TSource, TControl> : TargetBinder<TSource, TControl>, IControlBinder<TSource, TControl>
        where TControl : IBindableComponent
    {
        private readonly TControl _control;

        public ControlBinder(SourceBinder<TSource> sourceBinder, TControl control) : base(sourceBinder, control)
        {
            _control = control;
        }

        IControlPropertyBinder<TSource, TControl, TProp> IControlBinder<TSource, TControl>.Property<TProp>(
            Expression<Func<TControl, TProp>> member)
        {
            return new ControlPropertyBinder<TSource, TControl, TProp>(this, member);
        }

        public IControlStringPropertyBinder<TSource, TControl> Property(Expression<Func<TControl, string>> member)
        {
            return new ControlStringPropertyBinder<TSource, TControl>(this, member);
        }

        public IControlBinder<TSource, TControl> OnClick(ICommand command, object parameter = null)
        {
            return OnClick(command, () => parameter);
        }

        public IControlBinder<TSource, TControl> OnClick(ICommand command, Func<object> getParameter)
        {
            throw new NotImplementedException();
        }

        public IObservableBinder<TSource, EventArgs> OnClick()
        {
            var control = _control as Control;
            if (control == null)
                throw new NotSupportedException(
                    "The control must inherit from System.Windows.Form.Control in order use OnClick()");
            var observable = control.CreateClickObservable();
            var observableBinder = CreateObservableBinder(observable);
            return observableBinder;
        }

        public void AddDataBinding(Binding binding, ConvertEventHandler formatHandler = null,
            ConvertEventHandler parseHandler = null)
        {
            if (formatHandler != null)
                binding.Format += formatHandler;
            if (parseHandler != null)
                binding.Parse += parseHandler;
            _control.DataBindings.Add(binding);

            var subscription = Disposable.Create(() => RemoveDataBinding(binding, formatHandler, parseHandler));
            AddSubscription(subscription);
        }

        private void RemoveDataBinding(Binding binding, ConvertEventHandler formatHandler,
            ConvertEventHandler parseHandler)
        {
            _control.DataBindings.Remove(binding);
            if (formatHandler != null)
                binding.Format -= formatHandler;
            if (parseHandler != null)
                binding.Parse -= parseHandler;
        }

        internal Binding CreateBinding(string controlPropertyName, string sourcePropertyName, bool formattingEnabled, 
            ControlUpdateMode controlUpdateMode, DataSourceUpdateMode dataSourceUpdateMode)
        {
            return new Binding(controlPropertyName, Source, sourcePropertyName, formattingEnabled)
            {
                ControlUpdateMode = controlUpdateMode,
                DataSourceUpdateMode = dataSourceUpdateMode
            };
        }
    }
}