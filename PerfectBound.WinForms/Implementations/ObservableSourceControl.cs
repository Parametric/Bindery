using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Windows.Input;
using PerfectBound.WinForms.Interfaces;

namespace PerfectBound.WinForms.Implementations
{
    internal class ObservableSourceControl<TSource, TControl> : IObservableSourceControl<TSource, TControl> where TSource : INotifyPropertyChanged
        where TControl : Control
    {
        public ObservableSource<TSource> Source { get; private set; }
        public TControl Control { get; private set; }

        public ObservableSourceControl(ObservableSource<TSource> source, TControl control)
        {
            Source = source;
            Control = control;
        }

        public IObservableSourceControl<TSource, TControl> OnClick(Func<TSource, ICommand> commandMember)
        {
            var command = commandMember(Source.Object);
            Control.Click += (sender, e) => command.Execute(null);
            command.CanExecuteChanged += (sender, e) => Control.Enabled = command.CanExecute(null);
            return this;
        }

        public IObservableSourceControlProperty<TSource, TControl, TProp> Property<TProp>(Expression<Func<TControl, TProp>> member)
        {
            return new ObservableSourceControlProperty<TSource, TControl, TProp>(this, member);
        }

        public void AddDataBinding(Binding binding)
        {
            Control.DataBindings.Add(binding);
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