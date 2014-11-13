using System;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Windows.Input;

namespace PerfectBound.WinForms.Binding
{
    public interface IControlBinding<TSource, TControl> where TSource : class where TControl : IBindableComponent
    {
        TSource Source { get; }
        TControl Control { get; }

        /// <summary>
        /// Bind control to underlying source (defaults to two-way binding)
        /// </summary>
        /// <typeparam name="TProp">The type of the bound property</typeparam>
        /// <param name="controlMember">Specifies the bound control member</param>
        /// <param name="sourceMember">Specifies the bound data source member</param>
        /// <param name="controlUpdateMode">(optional) Control update mode</param>
        /// <param name="dataSourceUpdateMode">(optional) Data source update mode</param>
        /// <returns></returns>
        IControlBinding<TSource, TControl> Bind<TProp>(
            Expression<Func<TControl, TProp>> controlMember,
            Expression<Func<TSource, TProp>> sourceMember,
            ControlUpdateMode controlUpdateMode = ControlUpdateMode.OnPropertyChanged,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged);

        /// <summary>
        /// Bind control to underlying source with one-way binding that updates the control
        /// </summary>
        /// <typeparam name="TProp">The type of the bound property</typeparam>
        /// <param name="controlMember">Specifies the bound control member</param>
        /// <param name="sourceMember">Specifies the bound data source member</param>
        /// <returns></returns>
        IControlBinding<TSource, TControl> UpdateControl<TProp>(
            Expression<Func<TControl, TProp>> controlMember,
            Expression<Func<TSource, TProp>> sourceMember);

        /// <summary>
        /// Bind control to underlying source with one-way binding that updates the source
        /// </summary>
        /// <typeparam name="TProp">The type of the bound property</typeparam>
        /// <param name="controlMember">Specifies the bound control member</param>
        /// <param name="sourceMember">Specifies the bound data source member</param>
        /// <param name="dataSourceUpdateMode">(optional) Data source update mode</param>
        /// <returns></returns>
        IControlBinding<TSource, TControl> UpdateSource<TProp>(
            Expression<Func<TControl, TProp>> controlMember,
            Expression<Func<TSource, TProp>> sourceMember,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged);

        IControlBinding<TSource, TControl> Command(Func<TSource, ICommand> commandAccessor);
    }

    public class ControlBinding<TSource, TControl> : IControlBinding<TSource, TControl> where TSource : class
        where TControl : IBindableComponent
    {
        public ControlBinding(TSource source, TControl control)
        {
            Source = source;
            Control = control;
        }

        public TSource Source { get; private set; }

        public TControl Control { get; private set; }

        /// <summary>
        /// Bind control to underlying source (defaults to two-way binding)
        /// </summary>
        /// <typeparam name="TProp">The type of the bound property</typeparam>
        /// <param name="controlMember">Specifies the bound control member</param>
        /// <param name="sourceMember">Specifies the bound data source member</param>
        /// <param name="controlUpdateMode">(optional) Control update mode</param>
        /// <param name="dataSourceUpdateMode">(optional) Data source update mode</param>
        /// <returns></returns>
        public IControlBinding<TSource, TControl> Bind<TProp>(
            Expression<Func<TControl, TProp>> controlMember,
            Expression<Func<TSource, TProp>> sourceMember,
            ControlUpdateMode controlUpdateMode = ControlUpdateMode.OnPropertyChanged,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged)
        {
            AddBinding(controlMember, sourceMember, controlUpdateMode, dataSourceUpdateMode);
            return this;
        }

        /// <summary>
        /// Bind control to underlying source with one-way binding that updates the control
        /// </summary>
        /// <typeparam name="TProp">The type of the bound property</typeparam>
        /// <param name="controlMember">Specifies the bound control member</param>
        /// <param name="sourceMember">Specifies the bound data source member</param>
        /// <returns></returns>
        public IControlBinding<TSource, TControl> UpdateControl<TProp>(
            Expression<Func<TControl, TProp>> controlMember,
            Expression<Func<TSource, TProp>> sourceMember)
        {
            return Bind(controlMember, sourceMember, ControlUpdateMode.OnPropertyChanged, DataSourceUpdateMode.Never);
        }

        /// <summary>
        /// Bind control to underlying source with one-way binding that updates the source
        /// </summary>
        /// <typeparam name="TProp">The type of the bound property</typeparam>
        /// <param name="controlMember">Specifies the bound control member</param>
        /// <param name="sourceMember">Specifies the bound data source member</param>
        /// <param name="dataSourceUpdateMode">(optional) Data source update mode</param>
        /// <returns></returns>
        public IControlBinding<TSource, TControl> UpdateSource<TProp>(
            Expression<Func<TControl, TProp>> controlMember,
            Expression<Func<TSource, TProp>> sourceMember,
            DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnPropertyChanged)
        {
            return Bind(controlMember, sourceMember, ControlUpdateMode.Never, dataSourceUpdateMode);
        }

        private void AddBinding<TProp>(
            Expression<Func<TControl, TProp>> controlMember,
            Expression<Func<TSource, TProp>> sourceMember,
            ControlUpdateMode controlUpdateMode,
            DataSourceUpdateMode dataSourceUpdateMode)
        {
            var binding = CreateBinding(controlMember, sourceMember);
            binding.ControlUpdateMode = controlUpdateMode;
            binding.DataSourceUpdateMode = dataSourceUpdateMode;
            Control.DataBindings.Add(binding);
        }

        private System.Windows.Forms.Binding CreateBinding<TProp>(Expression<Func<TControl, TProp>> controlMember, Expression<Func<TSource, TProp>> sourceMember)
        {
            var propertyName = controlMember.GetAccessorName();
            var dataMember = sourceMember.GetAccessorName();

            var dataSourceType = Source.GetType();

            var sourceProperty = dataSourceType.GetProperty(dataMember);

            if (sourceProperty == null)
                throw new ArgumentException(string.Format("{0} is not a member of {1}", dataMember, dataSourceType.Name), "sourceMember");


            var binding = new System.Windows.Forms.Binding(propertyName, Source, sourceProperty.Name);
            return binding;
        }

        public IControlBinding<TSource, TControl> Command(Func<TSource, ICommand> commandMember) 
        {
            var control = Control as Control;
            if (control==null)
                throw new NotSupportedException("TControl must be an instance of System.Windows.Forms.Control");
            var command = commandMember(Source);
            control.Click += (sender, e) => command.Execute(null);
            command.CanExecuteChanged += (sender, e) => control.Enabled = command.CanExecute(null);
            return this;
        }

        public IControlBinding<TSource, TControl> Observe<TProp>(Func<TControl, IObservable<TProp>> observableMember,
            Func<TSource, ICommand> commandMember)
        {
            var observable = observableMember(Control);
            var command = commandMember(Source);
            var disposable = observable.Subscribe(p => command.Execute(p));
            return this;
        }
    }
}
