using System;
using System.Reactive.Linq;
using System.Windows.Forms;

namespace Bindery.Test.TestClasses
{
    public class TestButton : Button
    {
        public event EventHandler<TestEventArgs> Test;

        public TestButton()
        {
            MouseMoveButton = Observable.FromEvent<MouseEventHandler, MouseEventArgs>(
                argsAction => (sender, e) => argsAction(e),
                addHandler => this.MouseMove += addHandler,
                removeHandler => this.MouseMove -= removeHandler).Select(arg => Convert.ToString(arg.Button));
        }

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

        public IObservable<string> MouseMoveButton { get; private set; }
    }
}
