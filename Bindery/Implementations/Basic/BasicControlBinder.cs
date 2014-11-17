using System;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Windows.Forms;
using Bindery.Interfaces;
using Bindery.Interfaces.Basic;

namespace Bindery.Implementations.Basic
{
    internal class BasicControlBinder<TSource, TControl> : IBasicControlBinder<TSource,TControl> 
        where TControl : IBindableComponent
    {
        protected BasicSourceBinder<TSource> SourceBinder { get; private set; }
        public TControl Control { get; private set; }

        public BasicControlBinder(BasicSourceBinder<TSource> sourceBinder, TControl control)
        {
            SourceBinder = sourceBinder;
            Control = control;
        }

        public IBasicControlPropertyBinder<TSource, TControl, TProp> Property<TProp>(Expression<Func<TControl, TProp>> member)
        {
            return new BasicControlPropertyBinder<TSource, TControl, TProp>(this, member);
        }

        public IBasicControlEventBinder<TSource, TControl, TEventArgs> OnEvent<TEventArgs>(string eventName)
        {
            return new BasicControlEventBinder<TSource, TControl, TEventArgs>(this, eventName);
        }

        public IBasicControlEventBinder<TSource, TControl, EventArgs> OnEvent(string eventName)
        {
            return new BasicControlEventBinder<TSource, TControl, EventArgs>(this, eventName);
        }

        public IBasicControlObservableBinder<TSource, TControl, TArg> On<TArg>(Func<TControl, IObservable<TArg>> observableMember)
        {
            return new BasicControlObservableBinder<TSource, TControl, TArg>(this, observableMember(Control));
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