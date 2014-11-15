using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;
using Bindery.Interfaces;

namespace Bindery.Implementations
{
    internal class ControlPropertyBinder<TSource, TControl, TProp> : IControlPropertyBinder<TSource, TControl, TProp> 
        where TSource : INotifyPropertyChanged 
        where TControl : IBindableComponent
    {
        private readonly ControlBinder<TSource, TControl> _parent;
        private readonly string _memberName;

        public ControlPropertyBinder(ControlBinder<TSource, TControl> parent, Expression<Func<TControl, TProp>> member)
        {
            _parent = parent;
            _memberName = member.GetAccessorName();
        }

        public IControlBinder<TSource, TControl> BindTo(
            Expression<Func<TSource, TProp>> sourceMember,
            ControlUpdateMode controlUpdateMode = ControlUpdateMode.OnPropertyChanged,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged)
        {
            var binding = _parent.CreateBinding(_memberName, sourceMember.GetAccessorName(), controlUpdateMode, dataSourceUpdateMode);
            _parent.AddDataBinding(binding);
            return _parent;
        }

        public IControlBinder<TSource, TControl> UpdateSource(
            Expression<Func<TSource, TProp>> sourceMember,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged)
        {
            var binding = _parent.CreateBinding(_memberName, sourceMember.GetAccessorName(), ControlUpdateMode.Never, dataSourceUpdateMode);
            _parent.AddDataBinding(binding);
            return _parent;
        }

        public IControlBinder<TSource, TControl> UpdateControlFrom(Expression<Func<TSource, TProp>> sourceMember)
        {
            var binding = _parent.CreateBinding(_memberName, sourceMember.GetAccessorName(), ControlUpdateMode.OnPropertyChanged, DataSourceUpdateMode.Never);
            _parent.AddDataBinding(binding);
            return _parent;
        }

        public ITwoWayUpdatePropertyBinder<TSource, TControl, TSourceProp> Convert<TSourceProp>(Func<TProp, TSourceProp> to, Func<TSourceProp, TProp> from)
        {
            return new ConversionControlPropertyBinder<TSource, TControl, TProp, TSourceProp>(_parent, _memberName) { ConvertToFunc = to, ConvertFromFunc = from };
        }

        public IControlToSourceUpdatePropertyBinder<TSource, TControl, TSourceProp> ConvertTo<TSourceProp>(Func<TProp, TSourceProp> func)
        {
            return new ConversionControlPropertyBinder<TSource, TControl, TProp, TSourceProp>(_parent, _memberName) {ConvertToFunc = func};
        }

        public IControlFromSourceUpdatePropertyBinder<TSource, TControl, TSourceProp> ConvertFrom<TSourceProp>(Func<TSourceProp, TProp> func)
        {
            return new ConversionControlPropertyBinder<TSource, TControl, TProp, TSourceProp>(_parent, _memberName) { ConvertFromFunc = func };
        }
    }
}