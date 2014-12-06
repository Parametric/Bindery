using System;
using System.Reactive.Linq;
using System.Windows.Forms;

namespace Bindery
{
    internal static class ControlExtensions
    {
        public static IObservable<EventArgs> CreateClickObservable(this Control command)
        {
            return Observable.FromEvent<EventHandler, EventArgs>(
                argsAction => (sender, e) => argsAction(e),
                handler => command.Click += handler,
                handler => command.Click -= handler);
        }
    }
}
