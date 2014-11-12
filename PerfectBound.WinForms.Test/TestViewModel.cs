using System.ComponentModel;
using System.Runtime.CompilerServices;
using PerfectBound.WinForms.Test.Annotations;

namespace PerfectBound.WinForms.Test
{
    public class TestViewModel : INotifyPropertyChanged
    {
        public TestViewModel()
        {
            Command = new TestCommand(this);
        }

        private int _value;

        public int Value
        {
            get { return _value; }
            set
            {
                if (value == _value) return;
                _value = value;
                OnPropertyChanged();
            }
        }

        public TestCommand Command { get; set; }
        public int CommandExecutedCount { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}