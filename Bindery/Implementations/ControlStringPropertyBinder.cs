using System;
using System.Linq.Expressions;
using System.Windows.Forms;
using Bindery.Extensions;
using Bindery.Interfaces;
using Bindery.Interfaces.Binders;

namespace Bindery.Implementations
{
    internal class ControlStringPropertyBinder<TSource, TControl> : IControlStringPropertyBinder<TSource,TControl> where TControl : IBindableComponent
    {
        private readonly ControlBinder<TSource, TControl> _parent;
        private readonly string _memberName;

        public ControlStringPropertyBinder(ControlBinder<TSource, TControl> parent, Expression<Func<TControl, string>> member)
        {
            _parent = parent;
            _memberName = member.GetAccessorName();
        }

        public IControlBinder<TSource, TControl> Bind<TSourceMember>(Expression<Func<TSource, TSourceMember>> sourceMember, 
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged)
        {
            var binding = _parent.CreateBinding(_memberName, sourceMember.GetAccessorName(), true, ControlUpdateMode.OnPropertyChanged, dataSourceUpdateMode);
            _parent.AddDataBinding(binding);
            return _parent;
        }

        public IControlBinder<TSource, TControl> Set<TSourceMember>(Expression<Func<TSource, TSourceMember>> sourceMember, 
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged)
        {
            var binding = _parent.CreateBinding(_memberName, sourceMember.GetAccessorName(), true, ControlUpdateMode.Never, dataSourceUpdateMode);
            _parent.AddDataBinding(binding);
            return _parent;
        }

        public IControlBinder<TSource, TControl> Get<TSourceMember>(Expression<Func<TSource, TSourceMember>> sourceMember)
        {
            var binding = _parent.CreateBinding(_memberName, sourceMember.GetAccessorName(), true, ControlUpdateMode.OnPropertyChanged, DataSourceUpdateMode.Never);
            _parent.AddDataBinding(binding);
            return _parent;
        }
    }
}