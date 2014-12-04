using System;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace Bindery.Interfaces.Binders
{
    /// <summary>
    /// Binder for a control property
    /// </summary>
    /// <typeparam name="TSource">Type of the binding source</typeparam>
    /// <typeparam name="TControl">Type of the binding control</typeparam>
    /// <typeparam name="TProp">Type of the bound control property</typeparam>
    public interface IControlPropertyBinder<TSource, TControl, TProp>
        where TControl : IBindableComponent
    {
        /// <summary>
        /// Create one-way binding that updates the control's property value from the source
        /// </summary>
        /// <param name="sourceMember">Expression that specifies the source property</param>
        /// <returns>The control binder</returns>
        IControlBinder<TSource, TControl> Get(Expression<Func<TSource, TProp>> sourceMember);

        /// <summary>
        /// Create one-way binding that updates the control's property value from the source
        /// </summary>
        /// <param name="sourceMember">Expression that specifies the source property</param>
        /// <param name="convertToControlPropertyType">Function for converting the source property value to the control property's type</param>
        /// <returns>The control binder</returns>
        IControlBinder<TSource, TControl> Get<TSourceProp>(Expression<Func<TSource, TSourceProp>> sourceMember,
            Func<TSourceProp, TProp> convertToControlPropertyType);

        /// <summary>
        /// Create one-way binding that updates the source's property value from the control
        /// </summary>
        /// <param name="sourceMember">Expression that specifies the source property</param>
        /// <param name="dataSourceUpdateMode">(optional) When the source property is updated</param>
        /// <returns>The control binder</returns>
        IControlBinder<TSource, TControl> Set(Expression<Func<TSource, TProp>> sourceMember,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged);

        /// <summary>
        /// Create one-way binding that updates the source's property value from the control
        /// </summary>
        /// <param name="sourceMember">Expression that specifies the source property</param>
        /// <param name="convertToSourcePropertyType">Function for converting the control property value to the source property's type</param>
        /// <param name="dataSourceUpdateMode">(optional) When the source property is updated</param>
        /// <returns>The control binder</returns>
        IControlBinder<TSource, TControl> Set<TSourceProp>(Expression<Func<TSource, TSourceProp>> sourceMember,
            Func<TProp, TSourceProp> convertToSourcePropertyType,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged);

        /// <summary>
        /// Create two-way binding between the control property and source property
        /// </summary>
        /// <param name="sourceMember">Expression that specifies the source property</param>
        /// <param name="dataSourceUpdateMode">(optional) When the source property is updated</param>
        /// <returns>The control binder</returns>
        IControlBinder<TSource, TControl> Bind(
            Expression<Func<TSource, TProp>> sourceMember,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged);

        /// <summary>
        /// Create two-way binding between the control property and source property
        /// </summary>
        /// <param name="sourceMember">Expression that specifies the source property</param>
        /// <param name="convertToControlPropertyType">Function for converting the source property value to the control property's type</param>
        /// <param name="convertToSourcePropertyType">Function for converting the control property value to the source property's type</param>
        /// <param name="dataSourceUpdateMode">(optional) When the source property is updated</param>
        /// <returns>The control binder</returns>
        IControlBinder<TSource, TControl> Bind<TSourceProp>(
            Expression<Func<TSource, TSourceProp>> sourceMember,
            Func<TSourceProp, TProp> convertToControlPropertyType,
            Func<TProp, TSourceProp> convertToSourcePropertyType,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged);
    }
}