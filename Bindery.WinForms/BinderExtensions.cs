using System.Windows.Forms;
using Bindery.Implementations;
using Bindery.Interfaces.Binders;

namespace Bindery
{
    /// <summary>
    /// Binder extension methods
    /// </summary>
    public static class BinderExtensions
    {
        /// <summary>
        ///     Bind to an IBindableComponent
        /// </summary>
        /// <typeparam name="TSource">The source type</typeparam>
        /// <typeparam name="TControl">The control type</typeparam>
        /// <param name="sourceBinder">The binder for the source</param>
        /// <param name="control">The control</param>
        /// <returns>A control binder</returns>
        public static IControlBinder<TSource, TControl> Control<TSource, TControl>(this ISourceBinder<TSource> sourceBinder, TControl control)
            where TControl : IBindableComponent
        {
            var sourceBinderImpl = ((ISourceBinderAccess<TSource>)sourceBinder).GetSourceBinder();
            var controlBinder = new ControlBinder<TSource, TControl>(sourceBinderImpl, control);
            return controlBinder;
        }
    }
}