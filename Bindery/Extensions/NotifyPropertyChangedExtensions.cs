using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

        public static IObservable<PropertyChangedEventArgs> CreatePropertyChangedObservable(this INotifyPropertyChanged source, string propertyName)
        {
            return source.CreatePropertyChangedObservable().Where(e => e.PropertyName == propertyName);
        }

        public static IObservable<PropertyChangedEventArgs> CreatePropertyChangedObservable(this INotifyPropertyChanged source, IEnumerable<string> propertyNames)
        {
            return source.CreatePropertyChangedObservable().Where(e => propertyNames.Contains(e.PropertyName));
        }
    }
}
