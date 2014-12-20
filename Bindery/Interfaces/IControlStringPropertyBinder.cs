using System;
using System.Linq.Expressions;
using System.Windows.Forms;
using Bindery.Interfaces.Binders;

namespace Bindery.Interfaces
{
    /// <summary>
    ///     Binder for a control String property
    /// </summary>
    /// <typeparam name="TSource">Type of the binding source</typeparam>
    /// <typeparam name="TControl">Type of the binding control</typeparam>
    public interface IControlStringPropertyBinder<TSource, TControl> where TControl : IBindableComponent
    {
        /// <summary>
        ///     Create two-way binding between the control property and source property
        /// </summary>
        /// <param name="sourceMember">Expression that specifies the source property</param>
        /// <param name="dataSourceUpdateMode">(optional) When the source property is updated</param>
        /// <returns>The control binder</returns>
        IControlBinder<TSource, TControl> Bind<TSourceMember>(Expression<Func<TSource, TSourceMember>> sourceMember,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged);

        /// <summary>
        ///     Create one-way binding that updates the source's property value from the control
        /// </summary>
        /// <param name="sourceMember">Expression that specifies the source property</param>
        /// <param name="dataSourceUpdateMode">(optional) When the source property is updated</param>
        /// <returns>The control binder</returns>
        IControlBinder<TSource, TControl> Set<TSourceMember>(Expression<Func<TSource, TSourceMember>> sourceMember,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged);

        /// <summary>
        ///     Create one-way binding that updates the control's property value from the source
        /// </summary>
        /// <param name="sourceMember">Expression that specifies the source property</param>
        /// <returns>The control binder</returns>
        IControlBinder<TSource, TControl> Get<TSourceMember>(Expression<Func<TSource, TSourceMember>> sourceMember);
    }
}