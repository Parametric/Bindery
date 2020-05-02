using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Windows.Forms;
using Bindery.Extensions;
using NUnit.Framework;

namespace Bindery.Tests.Tests
{
    [TestFixture]
    public class ObservableObserverTest
    {
        private IObservable<int> _observable;
        private int _received;
        private int _receivedOnThreadId;

        [SetUp]
        public void BeforeEach()
        {
            _observable = new Subject<int>();
            _received = 0;
        }

        [Test]
        public void SendOnCurrentThread()
        {
            using (_observable.Subscribe(ReceiveValue))
            {
                var thread = new Thread(() => _observable.Send(5));
                thread.Start();
                ConditionalWait.WaitFor(() => _received > 0);
                Assert.That(_received, Is.EqualTo(5));
                Assert.That(_receivedOnThreadId, Is.EqualTo(thread.ManagedThreadId));
            }
        }

        [Test]
        [Ignore("Only passes in isolation")]
        public void SendToSynchronizationContextAsynchronously()
        {
            SynchronizationContext.SetSynchronizationContext(new WindowsFormsSynchronizationContext());
            var currentThread = Thread.CurrentThread;
            var threadId = currentThread.ManagedThreadId;
            var syncCtx = SynchronizationContext.Current;
            using (_observable.Subscribe(ReceiveValue))
            {
                var thread = new Thread(() => _observable.Post(5, syncCtx));
                thread.Start();
                Application.DoEvents();
                ConditionalWait.WaitFor(() => _received > 0);
                Assert.That(_received, Is.EqualTo(5));
                Assert.That(_receivedOnThreadId, Is.EqualTo(threadId));
            }
        }

        [Test]
        [Ignore("Only passes in isolation")]
        public void SendToSynchronizationContextSynchronously()
        {
            SynchronizationContext.SetSynchronizationContext(new WindowsFormsSynchronizationContext());
            var threadId = Thread.CurrentThread.ManagedThreadId;
            var syncCtx = SynchronizationContext.Current;
            using (_observable.Subscribe(ReceiveValue))
            {
                var thread = new Thread(() => _observable.Send(5, syncCtx));
                thread.Start();
                Application.DoEvents();
                ConditionalWait.WaitFor(() => _received > 0);
                Assert.That(_received, Is.EqualTo(5));
                Assert.That(_receivedOnThreadId, Is.EqualTo(threadId));
            }
        }

        private void ReceiveValue(int value)
        {
            _received = value;
            _receivedOnThreadId = Thread.CurrentThread.ManagedThreadId;
        }
    }
}
