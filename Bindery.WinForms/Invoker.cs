using System;
using System.Windows.Forms;
using Bindery.Interfaces;

namespace Bindery
{
    /// <summary>
    /// Facade to enable testing of control invoke
    /// </summary>
    public sealed class Invoker : IInvoker
    {
        static Invoker()
        {
            Current = new Invoker();
        }

        private Invoker()
        {
        }

        internal static IInvoker Current { get; private set; }

        void IInvoker.Invoke(Control control, Action action)
        {
            if (control.InvokeRequired)
                control.BeginInvoke(action);
            else
                action();
        }

        public static void Override(IInvoker invoker)
        {
            Current = invoker;
        }
    }
}
