using System.ComponentModel;
using PerfectBound.WinForms.Implementations;
using PerfectBound.WinForms.Interfaces;

namespace PerfectBound.WinForms
{
    public static class Bindery
    {
        public static IObservableSource<T> ObservableSource<T>(T source) where T : INotifyPropertyChanged
        {
            return new ObservableSource<T>(source);
        }
    }
}
