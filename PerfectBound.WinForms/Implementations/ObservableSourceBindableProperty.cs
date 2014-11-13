using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;
using PerfectBound.WinForms.Interfaces;

namespace PerfectBound.WinForms.Implementations
{
    internal class ObservableSourceBindableProperty<TSource, TBindable, TProp> : IObservableSourceBindableProperty<TSource, TBindable, TProp> 
        where TSource : INotifyPropertyChanged 
        where TBindable : IBindableComponent
    {
        private readonly ObservableSourceBindable<TSource, TBindable> _parent;
        private readonly string _memberName;

        public ObservableSourceBindableProperty(ObservableSourceBindable<TSource, TBindable> parent, Expression<Func<TBindable, TProp>> member)
        {
            _parent = parent;
            _memberName = member.GetAccessorName();
        }

        public IObservableSourceBindable<TSource, TBindable> BindTo(
            Expression<Func<TSource, TProp>> sourceMember,
            ControlUpdateMode controlUpdateMode = ControlUpdateMode.OnPropertyChanged,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged)
        {
            var binding = _parent.CreateBinding(_memberName, sourceMember.GetAccessorName(), controlUpdateMode, dataSourceUpdateMode);
            _parent.AddDataBinding(binding);
            return _parent;
        }

        public IObservableSourceBindable<TSource, TBindable> UpdateSource(
            Expression<Func<TSource, TProp>> sourceMember,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged)
        {
            var binding = _parent.CreateBinding(_memberName, sourceMember.GetAccessorName(), ControlUpdateMode.Never, dataSourceUpdateMode);
            _parent.AddDataBinding(binding);
            return _parent;
        }

        public IObservableSourceBindable<TSource, TBindable> UpdateControlFrom(Expression<Func<TSource, TProp>> sourceMember)
        {
            var binding = _parent.CreateBinding(_memberName, sourceMember.GetAccessorName(), ControlUpdateMode.OnPropertyChanged, DataSourceUpdateMode.Never);
            _parent.AddDataBinding(binding);
            return _parent;
        }

        public ITwoWayUpdateProperty<TSource, TBindable, TSourceProp> Convert<TSourceProp>(Func<TProp, TSourceProp> to, Func<TSourceProp, TProp> from)
        {
            return new ConversionControlProperty<TSource, TBindable, TProp, TSourceProp>(_parent, _memberName) { ConvertToFunc = to, ConvertFromFunc = from };
        }

        public IControlToSourceUpdateProperty<TSource, TBindable, TSourceProp> ConvertTo<TSourceProp>(Func<TProp, TSourceProp> func)
        {
            return new ConversionControlProperty<TSource, TBindable, TProp, TSourceProp>(_parent, _memberName) {ConvertToFunc = func};
        }

        public IControlFromSourceUpdateProperty<TSource, TBindable, TSourceProp> ConvertFrom<TSourceProp>(Func<TSourceProp, TProp> func)
        {
            return new ConversionControlProperty<TSource, TBindable, TProp, TSourceProp>(_parent, _memberName) { ConvertFromFunc = func };
        }
    }
}