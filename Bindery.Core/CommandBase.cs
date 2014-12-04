using System;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Bindery
{
    /// <summary>
    ///     Abstract base implementation of ICommand
    /// </summary>
    public abstract class CommandBase : ICommand
    {
        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        public abstract void Execute(object parameter);

        public event EventHandler CanExecuteChanged;

        /// <summary>
        ///     Get an observable for changes in the command's CanExecute value
        /// </summary>
        /// <param name="parameter">(optional) Parameter to use when evaluating CanExecute</param>
        /// <returns>An observable of CanExecute state</returns>
        public IObservable<bool> ObserveCanExecuteChanges(object parameter = null)
        {
            var eventObservable = Observable.FromEvent<EventHandler, EventArgs>(
                argsAction => (sender, e) => argsAction(e),
                ev => CanExecuteChanged += ev,
                ev => CanExecuteChanged -= ev);
            return eventObservable
                .Select(e => CanExecute(parameter))
                .DistinctUntilChanged();
        }

        protected virtual void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}