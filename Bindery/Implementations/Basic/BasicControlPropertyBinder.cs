using System;
using System.Linq.Expressions;
using System.Windows.Forms;
using Bindery.Extensions;
using Bindery.Interfaces;
using Bindery.Interfaces.Basic;

namespace Bindery.Implementations.Basic
{
    internal class BasicControlPropertyBinder<TSource, TControl, TControlProp> : IBasicControlPropertyBinder<TSource, TControl, TControlProp>
        where TControl : IBindableComponent
    {
        protected BasicControlBinder<TSource, TControl> Parent { get; private set; }
        protected string MemberName { get; private set; }

        public BasicControlPropertyBinder(BasicControlBinder<TSource, TControl> parent, Expression<Func<TControl, TControlProp>> member)
        {
            Parent = parent;
            MemberName = member.GetAccessorName();
        }

        public IBasicControlBinder<TSource, TControl> Set(Expression<Func<TSource, TControlProp>> sourceMember, DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged)
        {
            var binding = Parent.CreateBinding(MemberName, sourceMember.GetAccessorName(), ControlUpdateMode.Never, dataSourceUpdateMode);
            Parent.AddDataBinding(binding);
            return Parent;
        }

        public IBasicControlBinder<TSource, TControl> Set<TSourceProp>(Expression<Func<TSource, TSourceProp>> sourceMember, Func<TControlProp, TSourceProp> convertToSourcePropertyType, DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged)
        {
            var binding = Parent.CreateBinding(MemberName, sourceMember.GetAccessorName(), ControlUpdateMode.Never, dataSourceUpdateMode);
            ConvertEventHandler parseHandler = (sender, e) => e.Value = convertToSourcePropertyType((TControlProp)e.Value);
            Parent.AddDataBinding(binding, parseHandler: parseHandler);
            return Parent;
        }

    }
}