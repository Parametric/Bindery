using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;
using PerfectBound.WinForms.Interfaces;

namespace PerfectBound.WinForms.Implementations
{
    internal class ObservableSourceBindable<TSource, TBindable> : IObservableSourceBindable<TSource, TBindable> 
        where TSource : INotifyPropertyChanged
        where TBindable : IBindableComponent
    {
        public ObservableSource<TSource> Source { get; private set; }
        public TBindable Bindable { get; private set; }

        public ObservableSourceBindable(ObservableSource<TSource> source, TBindable bindable)
        {
            Source = source;
            Bindable = bindable;
        }

        public IObservableSourceBindableProperty<TSource, TBindable, TProp> Property<TProp>(Expression<Func<TBindable, TProp>> member)
        {
            return new ObservableSourceBindableProperty<TSource, TBindable, TProp>(this, member);
        }

        public void AddDataBinding(Binding binding)
        {
            Bindable.DataBindings.Add(binding);
        }

        internal Binding CreateBinding(string controlPropertyName, string sourcePropertyName, ControlUpdateMode controlUpdateMode, DataSourceUpdateMode dataSourceUpdateMode)
        {
            return new Binding(controlPropertyName, Source.Object, sourcePropertyName)
            {
                ControlUpdateMode = controlUpdateMode,
                DataSourceUpdateMode = dataSourceUpdateMode
            };
        }
    }
}