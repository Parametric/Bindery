using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace PerfectBound.WinForms.Interfaces
{
    public interface IControlToSourceUpdateProperty<TSource, TControl, TProp>
        where TSource : INotifyPropertyChanged
        where TControl: Control
    {
        IObservableSourceControl<TSource, TControl> UpdateSource(Expression<Func<TSource, TProp>> sourceMember, DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged);
    }
}