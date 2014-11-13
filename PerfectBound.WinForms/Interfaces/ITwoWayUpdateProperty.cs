using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace PerfectBound.WinForms.Interfaces
{
    public interface ITwoWayUpdateProperty<TSource, TControl, TProp>
        where TSource : INotifyPropertyChanged
        where TControl : Control
    {
        IObservableSourceControl<TSource, TControl> BindTo(
            Expression<Func<TSource, TProp>> sourceMember,
            ControlUpdateMode controlUpdateMode = ControlUpdateMode.OnPropertyChanged,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged);
    }
}