using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;
using PerfectBound.WinForms.Interfaces;

namespace PerfectBound.WinForms.Implementations
{
    internal class ConversionControlProperty<TSource, TBindable, TControlProp, TSourceProp> : 
        IControlToSourceUpdateProperty<TSource,TBindable, TSourceProp>, 
        IControlFromSourceUpdateProperty<TSource,TBindable,TSourceProp>,
        ITwoWayUpdateProperty<TSource, TBindable, TSourceProp>
        where TSource : INotifyPropertyChanged
        where TBindable: IBindableComponent
    {
        private readonly ObservableSourceBindable<TSource, TBindable> _parent;
        private readonly string _memberName;

        public ConversionControlProperty(ObservableSourceBindable<TSource, TBindable> parent, string memberName)
        {
            _parent = parent;
            _memberName = memberName;
        }

        public Func<TControlProp, TSourceProp> ConvertToFunc { get; set; }

        public Func<TSourceProp, TControlProp> ConvertFromFunc { get; set; }

        public IObservableSourceBindable<TSource, TBindable> UpdateSource(Expression<Func<TSource, TSourceProp>> sourceMember, DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged)
        {
            var binding = _parent.CreateBinding(_memberName, sourceMember.GetAccessorName(), ControlUpdateMode.Never, dataSourceUpdateMode);
            binding.Parse += (sender, e) => e.Value = ConvertToFunc((TControlProp) e.Value);
            _parent.AddDataBinding(binding);
            return _parent;
        }

        public IObservableSourceBindable<TSource, TBindable> UpdateBindable(Expression<Func<TSource, TSourceProp>> sourceMember)
        {
            var binding = _parent.CreateBinding(_memberName, sourceMember.GetAccessorName(), ControlUpdateMode.OnPropertyChanged, DataSourceUpdateMode.Never);
            binding.Format += (sender, e) => e.Value = ConvertFromFunc((TSourceProp)e.Value);
            _parent.AddDataBinding(binding);
            return _parent;
        }

        public IObservableSourceBindable<TSource, TBindable> BindTo(Expression<Func<TSource, TSourceProp>> sourceMember, ControlUpdateMode controlUpdateMode = ControlUpdateMode.OnPropertyChanged,
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