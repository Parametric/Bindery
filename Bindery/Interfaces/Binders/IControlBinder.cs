using System;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Windows.Input;

namespace Bindery.Interfaces.Binders
{
    /// <summary>
    ///     Binder for a control
    /// </summary>
    /// <typeparam name="TSource">The type of the binding source</typeparam>
    /// <typeparam name="TControl">The type of the control</typeparam>
    public interface IControlBinder<TSource, TControl> : ITargetBinder<TSource, TControl>
        where TControl : IBindableComponent
    {
        /// <summary>
        ///     Bind one of the control's properties
        /// </summary>
        /// <typeparam name="TProp">The property's type</typeparam>
        /// <param name="member">Expression that specifies the control's property</param>
        /// <returns>A control property binder</returns>
        new IControlPropertyBinder<TSource, TControl, TProp> Property<TProp>(Expression<Func<TControl, TProp>> member);

        /// <summary>
        ///     Bind one of the control's string properties
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        IControlStringPropertyBinder<TSource, TControl> Property(Expression<Func<TControl, string>> member);

        /// <summary>
        ///     Create an observable for the control's Click event
        /// </summary>
        /// <returns>An observable binder</returns>
        IObservableBinder<TSource,EventArgs> OnClick();

        [Obsolete("ICommand support removed in version 3", true)]
        IControlBinder<TSource, TControl> OnClick(ICommand command, object parameter = null);
        [Obsolete("ICommand support removed in version 3", true)]
        IControlBinder<TSource, TControl> OnClick(ICommand command, Func<object> getParameter);
    }
}