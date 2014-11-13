using System.ComponentModel;
using System.Runtime.CompilerServices;
using PerfectBound.WinForms.Test.Annotations;

namespace PerfectBound.WinForms.Test.TestClasses
{
    public class TestViewModel : INotifyPropertyChanged
    {
        public TestViewModel()
        {
            Command = new TestCommand(this);
        }

        private int _intValue;
        private string _stringValue;

        public int IntValue
        {
            get { return _intValue; }
            set
            {
                if (value == _intValue) return;
                _intValue = value;
                OnPropertyChanged();
            }
        }

        public string StringValue
        {
            get { return _stringValue; }
            set
            {
                if (value == _stringValue) return;
                _stringValue = value;
                OnPropertyChanged();
            }
        }

        public TestCommand Command { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}