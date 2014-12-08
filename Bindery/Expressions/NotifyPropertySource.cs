using System.Collections.Generic;
using System.ComponentModel;

namespace Bindery.Expressions
{
    internal class NotifyPropertySource
    {
        public NotifyPropertySource(INotifyPropertyChanged obj)
        {
            Object = obj;
            PropertyNames = new HashSet<string>();
        }

        public INotifyPropertyChanged Object { get; private set; }
        public HashSet<string> PropertyNames { get; private set; }
    }
}