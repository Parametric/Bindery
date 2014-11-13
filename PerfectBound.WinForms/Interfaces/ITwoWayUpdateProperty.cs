using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace PerfectBound.WinForms.Interfaces
{
    public interface ITwoWayUpdateProperty<TSource, TBindable, TProp>
        where TSource : INotifyPropertyChanged
        where TBindable : IBindableComponent
    {
        IObservableSourceBindable<TSource, TBindable> BindTo(
            Expression<Func<TSource, TProp>> sourceMember,
            ControlUpdateMode controlUpdateMode = ControlUpdateMode.OnPropertyChanged,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged);
    }
}