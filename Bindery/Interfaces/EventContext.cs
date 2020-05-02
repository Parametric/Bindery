using System;

namespace Bindery.Interfaces
{
    /// <summary>
    /// Full event information
    /// </summary>
    /// <typeparam name="TEventArgs"></typeparam>
    [Obsolete("No longer supported. Use a tuple of (object Sender, EventArgs Args) instead.", true)]
    public class EventContext<TEventArgs>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sender">The object that generated the event</param>
        /// <param name="args">The event arguments</param>
        public EventContext(object sender, TEventArgs args)
        {
            Sender = sender;
            Args = args;
        }

        /// <summary>
        /// The object that generated the event
        /// </summary>
        public object Sender { get; set; }

        /// <summary>
        /// The event arguments
        /// </summary>
        public TEventArgs Args { get; set; }
    }
}