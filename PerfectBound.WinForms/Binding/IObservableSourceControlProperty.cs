using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace PerfectBound.WinForms.Binding
{
    public interface IObservableSourceControlProperty<TSource, TControl, TProp>
        where TSource : INotifyPropertyChanged
        where TControl : Control
    {
        IObservableSourceControl<TSource, TControl> BindTo(
            Expression<Func<TSource, TProp>> sourceMember,
            ControlUpdateMode controlUpdateMode = ControlUpdateMode.OnPropertyChanged,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged);

        IObservableSourceControl<TSource, TControl> Updates(
            Expression<Func<TSource, TProp>> sourceMember,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged);

        IObservableSourceControl<TSource, TControl> UpdatedBy(Expression<Func<TSource, TProp>> sourceMember);
    }
}