using System;
using System.Linq.Expressions;
using System.Windows.Forms;
using Bindery.Extensions;
using Bindery.Interfaces.Binders;

namespace Bindery.Implementations
{
    internal class ControlPropertyBinder<TSource, TControl, TControlProp> :
        IControlPropertyBinder<TSource, TControl, TControlProp>
        where TControl : IBindableComponent
    {
        private readonly ControlBinder<TSource, TControl> _parent;
        private readonly string _memberName;

        public ControlPropertyBinder(ControlBinder<TSource, TControl> parent, Expression<Func<TControl, TControlProp>> member)
        {
            _parent = parent;
            _memberName = member.GetAccessorName();
        }

        public IControlBinder<TSource, TControl> Bind(
            Expression<Func<TSource, TControlProp>> sourceMember,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged)
        {
            var binding = _parent.CreateBinding(_memberName, sourceMember.GetAccessorName(), ControlUpdateMode.OnPropertyChanged, dataSourceUpdateMode);
            _parent.AddDataBinding(binding);
            return _parent;
        }

        public IControlBinder<TSource, TControl> Bind<TSourceProp>(
            Expression<Func<TSource, TSourceProp>> sourceMember,
            Func<TSourceProp, TControlProp> convertToControlPropertyType,
            Func<TControlProp, TSourceProp> convertToSourcePropertyType,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged)
        {
            var binding = _parent.CreateBinding(_memberName, sourceMember.GetAccessorName(), ControlUpdateMode.OnPropertyChanged, dataSourceUpdateMode);
            ConvertEventHandler formatHandler = (sender, e) => e.Value = convertToControlPropertyType((TSourceProp) e.Value);
            ConvertEventHandler parseHandler = (sender, e) => e.Value = convertToSourcePropertyType((TControlProp) e.Value);
            _parent.AddDataBinding(binding, formatHandler, parseHandler);
            return _parent;
        }

        public IControlBinder<TSource, TControl> Get(Expression<Func<TSource, TControlProp>> sourceMember)
        {
            var binding = _parent.CreateBinding(_memberName, sourceMember.GetAccessorName(), ControlUpdateMode.OnPropertyChanged,
                DataSourceUpdateMode.Never);
            _parent.AddDataBinding(binding);
            return _parent;
        }

        public IControlBinder<TSource, TControl> Get<TSourceProp>(
            Expression<Func<TSource, TSourceProp>> sourceMember,
            Func<TSourceProp, TControlProp> convertToControlPropertyType)
        {
            var binding = _parent.CreateBinding(_memberName, sourceMember.GetAccessorName(), ControlUpdateMode.OnPropertyChanged,
                DataSourceUpdateMode.Never);
            ConvertEventHandler formatHandler = (sender, e) => e.Value = convertToControlPropertyType((TSourceProp) e.Value);
            _parent.AddDataBinding(binding, formatHandler);
            return _parent;
        }

        public IControlBinder<TSource, TControl> Set(Expression<Func<TSource, TControlProp>> sourceMember,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged)
        {
            var binding = _parent.CreateBinding(_memberName, sourceMember.GetAccessorName(), ControlUpdateMode.Never, dataSourceUpdateMode);
            _parent.AddDataBinding(binding);
            return _parent;
        }

        public IControlBinder<TSource, TControl> Set<TSourceProp>(Expression<Func<TSource, TSourceProp>> sourceMember,
            Func<TControlProp, TSourceProp> convertToSourcePropertyType,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged)
        {
            var binding = _parent.CreateBinding(_memberName, sourceMember.GetAccessorName(), ControlUpdateMode.Never, dataSourceUpdateMode);
            ConvertEventHandler parseHandler = (sender, e) => e.Value = convertToSourcePropertyType((TControlProp) e.Value);
            _parent.AddDataBinding(binding, parseHandler: parseHandler);
            return _parent;
        }
    }
}