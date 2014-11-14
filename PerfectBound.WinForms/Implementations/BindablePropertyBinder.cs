using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;
using PerfectBound.WinForms.Interfaces;

namespace PerfectBound.WinForms.Implementations
{
    internal class BindablePropertyBinder<TSource, TBindable, TProp> : IBindablePropertyBinder<TSource, TBindable, TProp> 
        where TSource : INotifyPropertyChanged 
        where TBindable : IBindableComponent
    {
        private readonly BindableBinder<TSource, TBindable> _parent;
        private readonly string _memberName;

        public BindablePropertyBinder(BindableBinder<TSource, TBindable> parent, Expression<Func<TBindable, TProp>> member)
        {
            _parent = parent;
            _memberName = member.GetAccessorName();
        }

        public IBindableBinder<TSource, TBindable> BindTo(
            Expression<Func<TSource, TProp>> sourceMember,
            ControlUpdateMode controlUpdateMode = ControlUpdateMode.OnPropertyChanged,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged)
        {
            var binding = _parent.CreateBinding(_memberName, sourceMember.GetAccessorName(), controlUpdateMode, dataSourceUpdateMode);
            _parent.AddDataBinding(binding);
            return _parent;
        }

        public IBindableBinder<TSource, TBindable> UpdateSource(
            Expression<Func<TSource, TProp>> sourceMember,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged)
        {
            var binding = _parent.CreateBinding(_memberName, sourceMember.GetAccessorName(), ControlUpdateMode.Never, dataSourceUpdateMode);
            _parent.AddDataBinding(binding);
            return _parent;
        }

        public IBindableBinder<TSource, TBindable> UpdateControlFrom(Expression<Func<TSource, TProp>> sourceMember)
        {
            var binding = _parent.CreateBinding(_memberName, sourceMember.GetAccessorName(), ControlUpdateMode.OnPropertyChanged, DataSourceUpdateMode.Never);
            _parent.AddDataBinding(binding);
            return _parent;
        }

        public ITwoWayUpdatePropertyBinder<TSource, TBindable, TSourceProp> Convert<TSourceProp>(Func<TProp, TSourceProp> to, Func<TSourceProp, TProp> from)
        {
            return new ConversionControlPropertyBinder<TSource, TBindable, TProp, TSourceProp>(_parent, _memberName) { ConvertToFunc = to, ConvertFromFunc = from };
        }

        public IControlToSourceUpdatePropertyBinder<TSource, TBindable, TSourceProp> ConvertTo<TSourceProp>(Func<TProp, TSourceProp> func)
        {
            return new ConversionControlPropertyBinder<TSource, TBindable, TProp, TSourceProp>(_parent, _memberName) {ConvertToFunc = func};
        }

        public IControlFromSourceUpdatePropertyBinder<TSource, TBindable, TSourceProp> ConvertFrom<TSourceProp>(Func<TSourceProp, TProp> func)
        {
            return new ConversionControlPropertyBinder<TSource, TBindable, TProp, TSourceProp>(_parent, _memberName) { ConvertFromFunc = func };
        }
    }
}