using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace PerfectBound.WinForms.Interfaces
{
    public interface IControlToSourceUpdatePropertyBinder<TSource, TBindable, TProp>
        where TSource : INotifyPropertyChanged
        where TBindable: IBindableComponent
    {
        IBindableBinder<TSource, TBindable> UpdateSource(Expression<Func<TSource, TProp>> sourceMember, DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged);
    }
}