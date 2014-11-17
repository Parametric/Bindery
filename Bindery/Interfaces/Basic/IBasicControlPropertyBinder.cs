using System;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace Bindery.Interfaces.Basic
{
    public interface IBasicControlPropertyBinder<TSource, TControl, TControlProp>
        where TControl : IBindableComponent
    {
        IBasicControlBinder<TSource, TControl> Set(Expression<Func<TSource, TControlProp>> sourceMember, DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged);
        IBasicControlBinder<TSource, TControl> Set<TSourceProp>(Expression<Func<TSource, TSourceProp>> sourceMember, Func<TControlProp, TSourceProp> convertToSourcePropertyType, DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged);
    }
}