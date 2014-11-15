using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace Bindery.Interfaces
{
    public interface IControlFromSourceUpdatePropertyBinder<TSource, TControl, TProp>
        where TSource : INotifyPropertyChanged
        where TControl : IBindableComponent
    {
        IControlBinder<TSource, TControl> UpdateControlFrom(Expression<Func<TSource, TProp>> sourceMember);
    }
}