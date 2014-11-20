using System;
using System.Linq.Expressions;
using System.Windows.Forms;
using Bindery.Extensions;
using Bindery.Interfaces;

namespace Bindery.Implementations
{
    internal class ControlPropertyBinder<TSource, TControl, TControlProp> :
        IControlPropertyBinder<TSource, TControl, TControlProp> 
        where TControl : IBindableComponent
    {
        public ControlBinder<TSource, TControl> Parent { get; private set; }
        public string MemberName { get; private set; }

        public ControlPropertyBinder(ControlBinder<TSource, TControl> parent, Expression<Func<TControl, TControlProp>> member)
        {
            Parent = parent;
            MemberName = member.GetAccessorName();
        }


        public IControlBinder<TSource, TControl> Bind(
            Expression<Func<TSource, TControlProp>> sourceMember,
            ControlUpdateMode controlUpdateMode = ControlUpdateMode.OnPropertyChanged,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged)
        {
            var binding = Parent.CreateBinding(MemberName, sourceMember.GetAccessorName(), controlUpdateMode, dataSourceUpdateMode);
            Parent.AddDataBinding(binding);
            return Parent;
        }

        public IControlBinder<TSource, TControl> Bind<TSourceProp>(
            Expression<Func<TSource, TSourceProp>> sourceMember,
            Func<TSourceProp, TControlProp> convertToControlPropertyType,
            Func<TControlProp, TSourceProp> convertToSourcePropertyType,
            ControlUpdateMode controlUpdateMode = ControlUpdateMode.OnPropertyChanged,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged)
        {
            var binding = Parent.CreateBinding(MemberName, sourceMember.GetAccessorName(), controlUpdateMode,dataSourceUpdateMode);
            ConvertEventHandler formatHandler = (sender, e) => e.Value = convertToControlPropertyType((TSourceProp)e.Value);
            ConvertEventHandler parseHandler = (sender, e) => e.Value = convertToSourcePropertyType((TControlProp)e.Value);
            Parent.AddDataBinding(binding, formatHandler, parseHandler);
            return Parent;
        }

        public IControlBinder<TSource, TControl> Get(Expression<Func<TSource, TControlProp>> sourceMember)
        {
            var binding = Parent.CreateBinding(MemberName, sourceMember.GetAccessorName(), ControlUpdateMode.OnPropertyChanged, DataSourceUpdateMode.Never);
            Parent.AddDataBinding(binding);
            return Parent;
        }

        public IControlBinder<TSource, TControl> Get<TSourceProp>(
            Expression<Func<TSource, TSourceProp>> sourceMember, 
            Func<TSourceProp,TControlProp> convertToControlPropertyType)
        {
            var binding = Parent.CreateBinding(MemberName, sourceMember.GetAccessorName(), ControlUpdateMode.OnPropertyChanged, DataSourceUpdateMode.Never);
            ConvertEventHandler formatHandler = (sender, e) => e.Value = convertToControlPropertyType((TSourceProp) e.Value);
            Parent.AddDataBinding(binding, formatHandler);
            return Parent;
        }

        public IControlBinder<TSource, TControl> Set(Expression<Func<TSource, TControlProp>> sourceMember, DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged)
        {
            var binding = Parent.CreateBinding(MemberName, sourceMember.GetAccessorName(), ControlUpdateMode.Never, dataSourceUpdateMode);
            Parent.AddDataBinding(binding);
            return Parent;
        }

        public IControlBinder<TSource, TControl> Set<TSourceProp>(Expression<Func<TSource, TSourceProp>> sourceMember, Func<TControlProp, TSourceProp> convertToSourcePropertyType, DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged)
        {
            var binding = Parent.CreateBinding(MemberName, sourceMember.GetAccessorName(), ControlUpdateMode.Never, dataSourceUpdateMode);
            ConvertEventHandler parseHandler = (sender, e) => e.Value = convertToSourcePropertyType((TControlProp)e.Value);
            Parent.AddDataBinding(binding, parseHandler: parseHandler);
            return Parent;
        }

    }
}