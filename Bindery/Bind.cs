using System.ComponentModel;
using Bindery.Implementations;
using Bindery.Interfaces;

namespace Bindery
{
    public static class Bind
    {
        public static ISourceBinder<T> Source<T>(T source) where T : INotifyPropertyChanged
        {
            return new SourceBinder<T>(source);
        }
    }
}
