using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace PerfectBound.WinForms.Interfaces
{
    public interface IControlToSourceUpdateProperty<TSource, TBindable, TProp>
        where TSource : INotifyPropertyChanged
        where TBindable: IBindableComponent
    {
        IObservableSourceBindable<TSource, TBindable> UpdateSource(Expression<Func<TSource, TProp>> sourceMember, DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged);
    }
}