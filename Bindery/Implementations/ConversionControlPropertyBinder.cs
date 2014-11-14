using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;
using Bindery.Interfaces;

namespace Bindery.Implementations
{
    internal class ConversionControlPropertyBinder<TSource, TBindable, TControlProp, TSourceProp> : 
        IControlToSourceUpdatePropertyBinder<TSource,TBindable, TSourceProp>, 
        IControlFromSourceUpdatePropertyBinder<TSource,TBindable,TSourceProp>,
        ITwoWayUpdatePropertyBinder<TSource, TBindable, TSourceProp>
        where TSource : INotifyPropertyChanged
        where TBindable: IBindableComponent
    {
        private readonly BindableBinder<TSource, TBindable> _parent;
        private readonly string _memberName;

        public ConversionControlPropertyBinder(BindableBinder<TSource, TBindable> parent, string memberName)
        {
            _parent = parent;
            _memberName = memberName;
        }

        public Func<TControlProp, TSourceProp> ConvertToFunc { get; set; }

        public Func<TSourceProp, TControlProp> ConvertFromFunc { get; set; }

        public IBindableBinder<TSource, TBindable> UpdateSource(Expression<Func<TSource, TSourceProp>> sourceMember, DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged)
        {
            var binding = _parent.CreateBinding(_memberName, sourceMember.GetAccessorName(), ControlUpdateMode.Never, dataSourceUpdateMode);
            binding.Parse += (sender, e) => e.Value = ConvertToFunc((TControlProp) e.Value);
            _parent.AddDataBinding(binding);
            return _parent;
        }

        public IBindableBinder<TSource, TBindable> UpdateControlFrom(Expression<Func<TSource, TSourceProp>> sourceMember)
        {
            var binding = _parent.CreateBinding(_memberName, sourceMember.GetAccessorName(), ControlUpdateMode.OnPropertyChanged, DataSourceUpdateMode.Never);
            binding.Format += (sender, e) => e.Value = ConvertFromFunc((TSourceProp)e.Value);
            _parent.AddDataBinding(binding);
            return _parent;
        }

        public IBindableBinder<TSource, TBindable> BindTo(Expression<Func<TSource, TSourceProp>> sourceMember, ControlUpdateMode controlUpdateMode = ControlUpdateMode.OnPropertyChanged,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged)
        {
            var binding = _parent.CreateBinding(_memberName, sourceMember.GetAccessorName(), controlUpdateMode, dataSourceUpdateMode);
            binding.Parse += (sender, e) => e.Value = ConvertToFunc((TControlProp)e.Value);
            binding.Format += (sender, e) => e.Value = ConvertFromFunc((TSourceProp)e.Value);
            _parent.AddDataBinding(binding);
            return _parent;
        }
    }
}