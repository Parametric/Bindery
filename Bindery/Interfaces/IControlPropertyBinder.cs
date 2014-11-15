using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace Bindery.Interfaces
{
    public interface IControlPropertyBinder<TSource, TControl, TControlProp> 
        where TSource : INotifyPropertyChanged
        where TControl : IBindableComponent
    {
        IControlBinder<TSource, TControl> UpdateControlFrom(Expression<Func<TSource, TControlProp>> sourceMember);
        IControlBinder<TSource, TControl> UpdateControlFrom<TSourceProp>(Expression<Func<TSource, TSourceProp>> sourceMember, Func<TSourceProp, TControlProp> convertToControlPropertyType);

        IControlBinder<TSource, TControl> UpdateSource(
            Expression<Func<TSource, TControlProp>> sourceMember, 
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged);
        
        IControlBinder<TSource, TControl> UpdateSource<TSourceProp>(
            Expression<Func<TSource, TSourceProp>> sourceMember, 
            Func<TControlProp, TSourceProp> convertToSourcePropertyType, 
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged);

        IControlBinder<TSource, TControl> BindTo(
            Expression<Func<TSource, TControlProp>> sourceMember,
            ControlUpdateMode controlUpdateMode = ControlUpdateMode.OnPropertyChanged,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged);

        IControlBinder<TSource, TControl> BindTo<TSourceProp>(
            Expression<Func<TSource, TSourceProp>> sourceMember,
            Func<TSourceProp, TControlProp> convertToControlPropertyType,
            Func<TControlProp, TSourceProp> convertToSourcePropertyType,
            ControlUpdateMode controlUpdateMode = ControlUpdateMode.OnPropertyChanged,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged);
    }
}
