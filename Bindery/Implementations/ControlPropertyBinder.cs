using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;
using Bindery.Extensions;
using Bindery.Interfaces;

namespace Bindery.Implementations
{
    internal class ControlPropertyBinder<TSource, TControl, TControlProp> : IControlPropertyBinder<TSource, TControl, TControlProp> 
        where TSource : INotifyPropertyChanged 
        where TControl : IBindableComponent
    {
        private readonly ControlBinder<TSource, TControl> _parent;
        private readonly string _memberName;

        public ControlPropertyBinder(ControlBinder<TSource, TControl> parent, Expression<Func<TControl, TControlProp>> member)
        {
            _parent = parent;
            _memberName = member.GetAccessorName();
        }

        public IControlBinder<TSource, TControl> BindTo(
            Expression<Func<TSource, TControlProp>> sourceMember,
            ControlUpdateMode controlUpdateMode = ControlUpdateMode.OnPropertyChanged,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged)
        {
            var binding = _parent.CreateBinding(_memberName, sourceMember.GetAccessorName(), controlUpdateMode, dataSourceUpdateMode);
            _parent.AddDataBinding(binding);
            return _parent;
        }


        public IControlBinder<TSource, TControl> BindTo<TSourceProp>(
            Expression<Func<TSource, TSourceProp>> sourceMember,
            Func<TSourceProp, TControlProp> convertToControlPropertyType,
            Func<TControlProp, TSourceProp> convertToSourcePropertyType,
            ControlUpdateMode controlUpdateMode = ControlUpdateMode.OnPropertyChanged,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged)
        {
            var binding = _parent.CreateBinding(_memberName, sourceMember.GetAccessorName(), controlUpdateMode,dataSourceUpdateMode);
            ConvertEventHandler formatHandler = (sender, e) => e.Value = convertToControlPropertyType((TSourceProp)e.Value);
            ConvertEventHandler parseHandler = (sender, e) => e.Value = convertToSourcePropertyType((TControlProp)e.Value);
            _parent.AddDataBinding(binding, formatHandler, parseHandler);
            return _parent;
        }

        public IControlBinder<TSource, TControl> UpdateSource(
            Expression<Func<TSource, TControlProp>> sourceMember,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged)
        {
            var binding = _parent.CreateBinding(_memberName, sourceMember.GetAccessorName(), ControlUpdateMode.Never, dataSourceUpdateMode);
            _parent.AddDataBinding(binding);
            return _parent;
        }

        public IControlBinder<TSource, TControl> UpdateSource<TSourceProp>(
            Expression<Func<TSource, TSourceProp>> sourceMember, 
            Func<TControlProp, TSourceProp> convertToSourcePropertyType,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged)
        {
            var binding = _parent.CreateBinding(_memberName, sourceMember.GetAccessorName(), ControlUpdateMode.Never, dataSourceUpdateMode);
            ConvertEventHandler parseHandler = (sender, e) => e.Value = convertToSourcePropertyType((TControlProp) e.Value);
            _parent.AddDataBinding(binding, parseHandler: parseHandler);
            return _parent;
        }

        public IControlBinder<TSource, TControl> UpdateControlFrom(Expression<Func<TSource, TControlProp>> sourceMember)
        {
            var binding = _parent.CreateBinding(_memberName, sourceMember.GetAccessorName(), ControlUpdateMode.OnPropertyChanged, DataSourceUpdateMode.Never);
            _parent.AddDataBinding(binding);
            return _parent;
        }

        public IControlBinder<TSource, TControl> UpdateControlFrom<TSourceProp>(
            Expression<Func<TSource, TSourceProp>> sourceMember, 
            Func<TSourceProp,TControlProp> convertToControlPropertyType)
        {
            var binding = _parent.CreateBinding(_memberName, sourceMember.GetAccessorName(), ControlUpdateMode.OnPropertyChanged, DataSourceUpdateMode.Never);
            ConvertEventHandler formatHandler = (sender, e) => e.Value = convertToControlPropertyType((TSourceProp) e.Value);
            _parent.AddDataBinding(binding, formatHandler);
            return _parent;
        }
    }
}