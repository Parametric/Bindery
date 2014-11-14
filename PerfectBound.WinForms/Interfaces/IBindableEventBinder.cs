using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Input;

namespace PerfectBound.WinForms.Interfaces
{
    public interface IBindableEventBinder<TSource, TBindable>
        where TSource : INotifyPropertyChanged
        where TBindable : IBindableComponent
    {
        IBindableBinder<TSource, TBindable> Triggers(Func<TSource, ICommand> commandMember);
    }
}