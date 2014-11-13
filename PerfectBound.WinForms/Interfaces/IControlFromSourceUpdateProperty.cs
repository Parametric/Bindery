using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace PerfectBound.WinForms.Interfaces
{
    public interface IControlFromSourceUpdateProperty<TSource, TControl, TProp>
        where TSource : INotifyPropertyChanged
        where TControl : Control
    {
        IObservableSourceControl<TSource, TControl> UpdateControlFrom(Expression<Func<TSource, TProp>> sourceMember);
    }
}