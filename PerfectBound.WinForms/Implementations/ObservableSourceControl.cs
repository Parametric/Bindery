using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Input;
using PerfectBound.WinForms.Interfaces;

namespace PerfectBound.WinForms.Implementations
{
    internal class ObservableSourceControl<TSource, TControl> : 
        ObservableSourceBindable<TSource,TControl>, 
        IObservableSourceControl<TSource, TControl> 
        where TSource : INotifyPropertyChanged
        where TControl : Control
    {
        public ObservableSourceControl(ObservableSource<TSource> source, TControl bindable) :base(source,bindable)
        {
        }

        public IObservableSourceControl<TSource, TControl> OnClick(Func<TSource, ICommand> commandMember)
        {
            var command = commandMember(Source.Object);
            Bindable.Click += (sender, e) => command.Execute(null);
            command.CanExecuteChanged += (sender, e) => Bindable.Enabled = command.CanExecute(null);
            return this;
        }

    }
}