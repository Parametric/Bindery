using System;
using System.Reactive.Linq;

namespace Bindery.Test.TestClasses
{
    public class TestBasicViewModel
    {
        public TestBasicViewModel()
        {
            Command = new TestBasicCommand(this);
            MyObservable = Observable.Return(5);
        }

        public TestBasicCommand Command { get; set; }

        public int IntValue { get; set; }

        public string StringValue { get; set; }

        public IObservable<int> MyObservable { get; set; }
    }
}