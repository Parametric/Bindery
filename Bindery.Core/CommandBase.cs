using System;
using System.Reactive.Linq;
using System.Windows.Input;
using Bindery.Extensions;

namespace Bindery
{
    /// <summary>
    ///     Abstract base implementation of ICommand
    /// </summary>
    public abstract class CommandBase : ICommand
    {
        /// <summary>
        ///     Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns>
        ///     true if this command can be executed; otherwise, false.
        /// </returns>
        /// <param name="parameter">
        ///     Data used by the command.  If the command does not require data to be passed, this object can
        ///     be set to null.
        /// </param>
        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        ///     Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">
        ///     Data used by the command.  If the command does not require data to be passed, this object can
        ///     be set to null.
        /// </param>
        public abstract void Execute(object parameter);

        /// <summary>
        ///     Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        ///     Get an observable for changes in the command's CanExecute value
        /// </summary>
        /// <param name="parameter">(optional) Parameter to use when evaluating CanExecute</param>
        /// <returns>An observable of CanExecute state</returns>
        public IObservable<bool> ObserveCanExecuteChanges(object parameter = null)
        {
            return this.CreateCanExecuteChangedObservable()
                .Select(e => CanExecute(parameter))
                .DistinctUntilChanged();
        }

        /// <summary>
        ///     Invocator method for CanExecuteChanged event
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}