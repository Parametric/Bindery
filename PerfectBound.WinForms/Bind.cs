using System.ComponentModel;
using PerfectBound.WinForms.Implementations;
using PerfectBound.WinForms.Interfaces;

namespace PerfectBound.WinForms
{
    public static class Bind
    {
        public static ISourceBinder<T> Source<T>(T source) where T : INotifyPropertyChanged
        {
            return new SourceBinder<T>(source);
        }
    }
}
