using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PerfectBound.WinForms.Interfaces
{
    public interface IObservableSourceControlProperty<TSource, TControl, TProp> :
        IControlFromSourceUpdateProperty<TSource, TControl, TProp>,
        IControlToSourceUpdateProperty<TSource, TControl, TProp>,
        ITwoWayUpdateProperty<TSource, TControl, TProp>
        where TSource : INotifyPropertyChanged
        where TControl : Control
    {
        ITwoWayUpdateProperty<TSource, TControl, TSourceProp> Convert<TSourceProp>(Func<TProp, TSourceProp> to,Func<TSourceProp, TProp> from);
        
        IControlToSourceUpdateProperty<TSource, TControl, TSourceProp> ConvertTo<TSourceProp>(Func<TProp, TSourceProp> func);

        IControlFromSourceUpdateProperty<TSource, TControl, TSourceProp> ConvertFrom<TSourceProp>(Func<TSourceProp, TProp> func);
    }
}