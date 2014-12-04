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
        ///     Bind the control's Click event to a command
        /// </summary>
        /// <param name="command">The command</param>
        /// <param name="parameter">(optional) The parameter passed to ICommand.Execute</param>
        /// <returns>The control binder</returns>
        /// <remarks>The control's Enabled property will also be bound to ICommand.CanExecute</remarks>
        /// <exception cref="NotSupportedException">TControl must inherit from System.Windows.Form.Control</exception>
        IControlBinder<TSource, TControl> OnClick(ICommand command, object parameter = null);

        /// <summary>
        ///     Bind the control's Click event to a command
        /// </summary>
        /// <param name="command">The command</param>
        /// <param name="getParameter">
        ///     A function that determines the parameter passed to ICommand.Execute, evaluated at the time
        ///     the Click event occurs
        /// </param>
        /// <returns>The control binder</returns>
        /// <remarks>The control's Enabled property will also be bound to ICommand.CanExecute</remarks>
        /// <exception cref="NotSupportedException">TControl must inherit from System.Windows.Form.Control</exception>
        IControlBinder<TSource, TControl> OnClick(ICommand command, Func<object> getParameter);
    }
}