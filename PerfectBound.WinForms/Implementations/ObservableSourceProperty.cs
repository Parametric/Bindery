using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Linq;
using PerfectBound.WinForms.Interfaces;

namespace PerfectBound.WinForms.Implementations
{
    internal class ObservableSourceProperty<TSource, TProp> : IObservableSourceProperty<TSource, TProp> where TSource : INotifyPropertyChanged
    {
        private readonly ObservableSource<TSource> _source;
        private readonly string _memberName;
        private readonly Func<TSource, TProp> _memberAccessor;

        public ObservableSourceProperty(ObservableSource<TSource> source, Expression<Func<TSource, TProp>> member)
        {
            _source = source;
            _memberName = member.GetAccessorName();
            _memberAccessor = member.Compile();
        }

        public IObservableSource<TSource> OnChanged(Action<TProp> action)
        {
            var subscription = _source.PropertyChangedObservable.Where(args => args.PropertyName == _memberName)
                .Select(x => _memberAccessor.Invoke(_source.Object))
                .Subscribe(action);
            _source.AddSubscription(subscription);
            return _source;
        }
    }
}