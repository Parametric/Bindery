using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using Bindery.Test.Properties;

namespace Bindery.Tests.TestClasses
{
    public class TestViewModel : INotifyPropertyChanged
    {
        private int _intValue;
        private string _stringValue;

        public TestViewModel()
        {
            MyObservable = Observable.Return(5);
            ComplexValue = new Inner();
        }

        public int IntValue
        {
            get => _intValue;
            set
            {
                if (value == _intValue) return;
                _intValue = value;
                OnPropertyChanged();
            }
        }

        public string StringValue
        {
            get => _stringValue;
            set
            {
                if (value == _stringValue) return;
                _stringValue = value;
                OnPropertyChanged();
            }
        }

        public Inner ComplexValue { get; }

        public IObservable<int> MyObservable { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public class Inner : INotifyPropertyChanged
        {
            private decimal _decValue;

            public decimal DecValue
            {
                get => _decValue;
                set
                {
                    if (value == _decValue) return;
                    _decValue = value;
                    OnPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            [Test.Annotations.NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public object MyMethod()
        {
            throw new NotImplementedException();
        }
    }
}