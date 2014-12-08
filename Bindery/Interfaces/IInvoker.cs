using System;
using System.Windows.Forms;

namespace Bindery.Interfaces
{
    /// <summary>
    /// Facade to enable testing of control invoke
    /// </summary>
    public interface IInvoker
    {
        /// <summary>
        /// Invoke an action on a control's thread
        /// </summary>
        /// <param name="control">The control</param>
        /// <param name="action">The action</param>
        void Invoke(Control control, Action action);
    }
}