using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace Bindery.Interfaces
{
    public interface IControlToSourceUpdatePropertyBinder<TSource, TControl, TProp>
        where TSource : INotifyPropertyChanged
        where TControl: IBindableComponent
    {
        IControlBinder<TSource, TControl> UpdateSource(Expression<Func<TSource, TProp>> sourceMember, DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged);
    }
}