using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace PerfectBound.WinForms.Binding
{
    public class BindingSource<TSource> : IDisposable where TSource : class
    {
        private readonly TSource _source;
        private Dictionary<string, Action> _monitorActions;

        public BindingSource(TSource source)
        {
            _source = source;
        }

        public IControlBinding<TSource, TControl> Control<TControl>(TControl control) where TControl : IBindableComponent
        {
            return new ControlBinding<TSource, TControl>(_source, control);
        }

        /// <summary>
        /// Monitor a source property for changes
        /// </summary>
        /// <typeparam name="TProp">The property type</typeparam>
        /// <param name="sourceMember">The source property to monitor</param>
        /// <param name="action">An action to take when the property changes</param>
        /// <remarks>Be sure to dispose of the source binder to unsubscribe from monitoring</remarks>
        /// <returns></returns>
        public BindingSource<TSource> MonitorSource<TProp>(Expression<Func<TSource, TProp>> sourceMember, Action<TProp> action)
        {
            var notify = _source as INotifyPropertyChanged;
            if (notify == null)
                throw new NotSupportedException("Source must implement INotifyPropertyChanged.");
            // Get info on the source property
            var propertyName = Bindery.GetAccessorName(sourceMember);
            if (_monitorActions != null && _monitorActions.ContainsKey(propertyName))
                throw new ArgumentException(string.Format("{0} is already being monitored by this BindingSource.", propertyName), "sourceMember");
            var dataSourceType = _source.GetType();
            var sourceProperty = dataSourceType.GetProperty(propertyName);
            if (sourceProperty == null)
                throw new ArgumentException(String.Format("{0} is not a member of {1}", propertyName, dataSourceType.Name), "sourceMember");
            // Set up event to handle updates
            if (_monitorActions == null)
            {
                _monitorActions = new Dictionary<string, Action>();
                notify.PropertyChanged += INotifyPropertyChanged_PropertyChanged;
            }
            // Define action to fire on property change and add to dictionary
            Action onChange = () => action((TProp)sourceProperty.GetValue(_source));
            _monitorActions.Add(propertyName, onChange);
            // Evaluate action immediately to handle initial value
            onChange();
            return this;
        }

        void INotifyPropertyChanged_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_monitorActions == null || !_monitorActions.ContainsKey(e.PropertyName))
                return;
            var action = _monitorActions[e.PropertyName];
            action();
        }

        public void Dispose()
        {
            if (_monitorActions == null)
                return;
            _monitorActions = null;
            var notify = (INotifyPropertyChanged)_source;
            notify.PropertyChanged -= INotifyPropertyChanged_PropertyChanged;
        }
    }
}
