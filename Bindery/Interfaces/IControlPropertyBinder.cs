using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Bindery.Interfaces
{
    public interface IControlPropertyBinder<TSource, TControl, TProp> :
        IControlFromSourceUpdatePropertyBinder<TSource, TControl, TProp>,
        IControlToSourceUpdatePropertyBinder<TSource, TControl, TProp>,
        ITwoWayUpdatePropertyBinder<TSource, TControl, TProp>
        where TSource : INotifyPropertyChanged
        where TControl : IBindableComponent
    {
        ITwoWayUpdatePropertyBinder<TSource, TControl, TSourceProp> Convert<TSourceProp>(Func<TProp, TSourceProp> to, Func<TSourceProp, TProp> from);
        IControlToSourceUpdatePropertyBinder<TSource, TControl, TSourceProp> ConvertTo<TSourceProp>(Func<TProp, TSourceProp> func);
        IControlFromSourceUpdatePropertyBinder<TSource, TControl, TSourceProp> ConvertFrom<TSourceProp>(Func<TSourceProp, TProp> func);
    }
}
