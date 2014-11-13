using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Windows.Input;

namespace PerfectBound.WinForms.Binding
{
    public interface IObservableSourceControl<TSource, TControl>
        where TSource : INotifyPropertyChanged
        where TControl : Control
    {
        IObservableSourceControl<TSource, TControl> OnClick(Func<TSource, ICommand> commandMember);
        IObservableSourceControlProperty<TSource, TControl, TProp> Property<TProp>(Expression<Func<TControl, TProp>> member);
    }
}