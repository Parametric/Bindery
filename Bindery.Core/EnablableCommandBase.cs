namespace Bindery
{
    /// <summary>
    /// Basic abstract implementation of ICommand with Enabled property to control CanExecute results externally
    /// </summary>
    public abstract class EnablableCommandBase : CommandBase
    {
        private bool _enabled;

        /// <summary>
        /// Is the command enabled?
        /// </summary>
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (value == _enabled) return;
                _enabled = value;
                OnCanExecuteChanged();
            }
        }

        public override bool CanExecute(object parameter)
        {
            return Enabled;
        }
    }
}
