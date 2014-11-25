using System;
using System.Reactive.Linq;
using System.Windows.Forms;

namespace Bindery.Test.TestClasses
{
    public class TestButton : Button
    {
        public TestButton()
        {
            MouseMoveButton = Create.ObservableFor(this).Event<MouseEventArgs>("MouseMove").Select(arg => Convert.ToString(arg.Button));
            ClickObservable = Create.ObservableFor(this).Event("Click");
        }

        public IObservable<string> MouseMoveButton { get; private set; }
        public IObservable<EventArgs> ClickObservable { get; private set; }
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
}