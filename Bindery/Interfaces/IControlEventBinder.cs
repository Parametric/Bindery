using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Input;

namespace Bindery.Interfaces
{
    public interface IControlEventBinder<TSource, TControl>
        where TSource : INotifyPropertyChanged
        where TControl : IBindableComponent
    {
        IControlBinder<TSource, TControl> Executes(Func<TSource, ICommand> commandMember);
    }
}