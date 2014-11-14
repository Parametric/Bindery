using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Input;

namespace PerfectBound.WinForms.Interfaces
{
    public interface IControlBinder<TSource, TControl>:
        IBindableBinder<TSource, TControl>
        where TSource : INotifyPropertyChanged
        where TControl : Control
    {
        IControlBinder<TSource, TControl> OnClick(Func<TSource, ICommand> commandMember);
    }
}