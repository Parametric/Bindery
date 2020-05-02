using System;
using System.Reactive.Linq;
using System.Windows.Forms;

namespace Bindery.Tests.TestClasses
{
    public class TestButton : Button
    {
        public TestButton()
        {
            MouseMoveButton = Binder.Observe(this).Event<MouseEventArgs>("MouseMove").Select(x => Convert.ToString(x.Args.Button));
            ClickObservable = Binder.Observe(this).Event("Click").Select(x=>x.Args);
        }

        public IObservable<string> MouseMoveButton { get; }
        public IObservable<EventArgs> ClickObservable { get; }
        public event EventHandler<TestEventArgs> Test;

        protected virtual void OnTest(TestEventArgs e)
        {
            Test?.Invoke(this, e);
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
}