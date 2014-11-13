using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;
using PerfectBound.WinForms.Interfaces;

namespace PerfectBound.WinForms.Implementations
{
    internal class ObservableSourceControlProperty<TSource, TControl, TProp> : IObservableSourceControlProperty<TSource, TControl, TProp> 
        where TSource : INotifyPropertyChanged 
        where TControl : Control
    {
        private readonly ObservableSourceControl<TSource, TControl> _parent;
        private readonly string _memberName;

        public ObservableSourceControlProperty(ObservableSourceControl<TSource, TControl> parent, Expression<Func<TControl, TProp>> member)
        {
            _parent = parent;
            _memberName = member.GetAccessorName();
        }

        public IObservableSourceControl<TSource, TControl> BindTo(
            Expression<Func<TSource, TProp>> sourceMember,
            ControlUpdateMode controlUpdateMode = ControlUpdateMode.OnPropertyChanged,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged)
        {
            var binding = _parent.CreateBinding(_memberName, sourceMember.GetAccessorName(), controlUpdateMode, dataSourceUpdateMode);
            _parent.AddDataBinding(binding);
            return _parent;
        }

        public IObservableSourceControl<TSource, TControl> UpdateSource(
            Expression<Func<TSource, TProp>> sourceMember,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged)
        {
            var binding = _parent.CreateBinding(_memberName, sourceMember.GetAccessorName(), ControlUpdateMode.Never, dataSourceUpdateMode);
            _parent.AddDataBinding(binding);
            return _parent;
        }

        public IObservableSourceControl<TSource, TControl> UpdateControlFrom(Expression<Func<TSource, TProp>> sourceMember)
        {
            var binding = _parent.CreateBinding(_memberName, sourceMember.GetAccessorName(), ControlUpdateMode.OnPropertyChanged, DataSourceUpdateMode.Never);
            _parent.AddDataBinding(binding);
            return _parent;
        }

        public ITwoWayUpdateProperty<TSource, TControl, TSourceProp> Convert<TSourceProp>(Func<TProp, TSourceProp> to, Func<TSourceProp, TProp> from)
        {
            return new ConversionControlProperty<TSource, TControl, TProp, TSourceProp>(_parent, _memberName) { ConvertToFunc = to, ConvertFromFunc = from };
        }

        public IControlToSourceUpdateProperty<TSource, TControl, TSourceProp> ConvertTo<TSourceProp>(Func<TProp, TSourceProp> func)
        {
            return new ConversionControlProperty<TSource, TControl, TProp, TSourceProp>(_parent, _memberName) {ConvertToFunc = func};
        }

        public IControlFromSourceUpdateProperty<TSource, TControl, TSourceProp> ConvertFrom<TSourceProp>(Func<TSourceProp, TProp> func)
        {
            return new ConversionControlProperty<TSource, TControl, TProp, TSourceProp>(_parent, _memberName) { ConvertFromFunc = func };
        }
    }
}