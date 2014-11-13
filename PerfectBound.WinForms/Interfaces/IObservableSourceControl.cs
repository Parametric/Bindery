using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Input;

namespace PerfectBound.WinForms.Interfaces
{
    public interface IObservableSourceControl<TSource, TControl>:
        IObservableSourceBindable<TSource, TControl>
        where TSource : INotifyPropertyChanged
        where TControl : Control
    {
        IObservableSourceControl<TSource, TControl> OnClick(Func<TSource, ICommand> commandMember);
    }
}