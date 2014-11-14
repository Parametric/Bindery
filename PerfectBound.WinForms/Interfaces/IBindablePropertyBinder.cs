using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PerfectBound.WinForms.Interfaces
{
    public interface IBindablePropertyBinder<TSource, TBindable, TProp> :
        IControlFromSourceUpdatePropertyBinder<TSource, TBindable, TProp>,
        IControlToSourceUpdatePropertyBinder<TSource, TBindable, TProp>,
        ITwoWayUpdatePropertyBinder<TSource, TBindable, TProp>
        where TSource : INotifyPropertyChanged
        where TBindable : IBindableComponent
    {
        ITwoWayUpdatePropertyBinder<TSource, TBindable, TSourceProp> Convert<TSourceProp>(Func<TProp, TSourceProp> to, Func<TSourceProp, TProp> from);
        IControlToSourceUpdatePropertyBinder<TSource, TBindable, TSourceProp> ConvertTo<TSourceProp>(Func<TProp, TSourceProp> func);
        IControlFromSourceUpdatePropertyBinder<TSource, TBindable, TSourceProp> ConvertFrom<TSourceProp>(Func<TSourceProp, TProp> func);
    }
}
