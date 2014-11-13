using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace PerfectBound.WinForms.Binding
{
    class ObservableSourceControlProperty<TSource, TControl, TProp> : IObservableSourceControlProperty<TSource, TControl, TProp> 
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
            AddBinding(_memberName, sourceMember, controlUpdateMode, dataSourceUpdateMode);
            return _parent;
        }

        public IObservableSourceControl<TSource, TControl> Updates(
            Expression<Func<TSource, TProp>> sourceMember,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged)
        {
            AddBinding(_memberName, sourceMember, ControlUpdateMode.Never, dataSourceUpdateMode);
            return _parent;
        }

        public IObservableSourceControl<TSource, TControl> UpdatedBy(Expression<Func<TSource, TProp>> sourceMember)
        {
            AddBinding(_memberName, sourceMember, ControlUpdateMode.OnPropertyChanged, DataSourceUpdateMode.Never);
            return _parent;
        }

        private void AddBinding(string controlPropertyName, Expression<Func<TSource, TProp>> sourceMember, ControlUpdateMode controlUpdateMode, DataSourceUpdateMode dataSourceUpdateMode)
        {
            var binding = CreateBinding(controlPropertyName, sourceMember, controlUpdateMode, dataSourceUpdateMode);
            _parent.AddDataBinding(binding);
        }

        private System.Windows.Forms.Binding CreateBinding(string controlPropertyName, Expression<Func<TSource, TProp>> sourceMember, ControlUpdateMode controlUpdateMode,
            DataSourceUpdateMode dataSourceUpdateMode)
        {
            var sourcePropertyName = sourceMember.GetAccessorName();
            return new System.Windows.Forms.Binding(controlPropertyName, _parent.Source.Object, sourcePropertyName)
            {
                ControlUpdateMode = controlUpdateMode,
                DataSourceUpdateMode = dataSourceUpdateMode
            };
        }
    }
}