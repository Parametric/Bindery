using System;
using System.Windows.Forms;
using Bindery.Interfaces;

namespace Bindery.Tests.TestClasses
{
    public class TestInvoker : IInvoker
    {
        public void Invoke(Control control, Action action)
        {
            action();
            Invoked = true;
        }

        public bool Invoked { get; private set; }
    }
}