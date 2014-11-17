using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Bindery.Extensions
{
    public static class NotifyPropertyChangedExtensions
    {
        public static IObservable<PropertyChangedEventArgs> CreatePropertyChangedObservable(this INotifyPropertyChanged source)
        {
            return Observable.FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                argsAction => (sender, e) => argsAction(e),
                handler => source.PropertyChanged += handler,
                handler => source.PropertyChanged -= handler);
        }
    }
}
