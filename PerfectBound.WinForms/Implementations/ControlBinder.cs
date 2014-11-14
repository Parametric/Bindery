using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Input;
using PerfectBound.WinForms.Interfaces;

namespace PerfectBound.WinForms.Implementations
{
    internal class ControlBinder<TSource, TControl> : 
        BindableBinder<TSource,TControl>, 
        IControlBinder<TSource, TControl> 
        where TSource : INotifyPropertyChanged
        where TControl : Control
    {
        public ControlBinder(SourceBinder<TSource> sourceBinder, TControl bindable) :base(sourceBinder,bindable)
        {
        }

        public IControlBinder<TSource, TControl> OnClick(Func<TSource, ICommand> commandMember)
        {
            var command = commandMember(SourceBinder.Object);
            Bindable.Click += (sender, e) => command.Execute(null);
            command.CanExecuteChanged += (sender, e) => Bindable.Enabled = command.CanExecute(null);
            return this;
        }

    }
}