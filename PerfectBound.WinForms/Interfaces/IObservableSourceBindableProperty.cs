using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PerfectBound.WinForms.Interfaces
{
    public interface IObservableSourceBindableProperty<TSource, TBindable, TProp> :
        IControlFromSourceUpdateProperty<TSource, TBindable, TProp>,
        IControlToSourceUpdateProperty<TSource, TBindable, TProp>,
        ITwoWayUpdateProperty<TSource, TBindable, TProp>
        where TSource : INotifyPropertyChanged
        where TBindable : IBindableComponent
    {
        ITwoWayUpdateProperty<TSource, TBindable, TSourceProp> Convert<TSourceProp>(Func<TProp, TSourceProp> to, Func<TSourceProp, TProp> from);
        IControlToSourceUpdateProperty<TSource, TBindable, TSourceProp> ConvertTo<TSourceProp>(Func<TProp, TSourceProp> func);
        IControlFromSourceUpdateProperty<TSource, TBindable, TSourceProp> ConvertFrom<TSourceProp>(Func<TSourceProp, TProp> func);
    }
}
