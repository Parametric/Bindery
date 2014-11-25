﻿using System.Windows.Forms;
using Bindery.Implementations;
using Bindery.Interfaces.Binders;

namespace Bindery
{
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
            return new ControlBinder<TSource, TControl>((SourceBinder<TSource>) sourceBinder, control);
        }
    }
}