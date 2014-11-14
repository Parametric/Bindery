using System;
using System.Windows.Forms;

namespace PerfectBound.WinForms.Test.TestClasses
{
    public class TestButton : Button
    {
        public event EventHandler<TestEventArgs> Test;

        protected virtual void OnTest(TestEventArgs e)
        {
            var handler = Test;
            if (handler != null) handler(this, e);
        }

        public void PerformTest(TestEventArgs e)
        {
            OnTest(e);
        }

        public void PerformMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }
    }

    public class TestEventArgs
    {
    }
}
