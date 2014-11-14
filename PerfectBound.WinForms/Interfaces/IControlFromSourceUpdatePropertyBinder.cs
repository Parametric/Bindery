using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace PerfectBound.WinForms.Interfaces
{
    public interface IControlFromSourceUpdatePropertyBinder<TSource, TBindable, TProp>
        where TSource : INotifyPropertyChanged
        where TBindable : IBindableComponent
    {
        IBindableBinder<TSource, TBindable> UpdateControlFrom(Expression<Func<TSource, TProp>> sourceMember);
    }
}