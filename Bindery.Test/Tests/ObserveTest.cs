using System;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bindery.Test.TestClasses;
using NUnit.Framework;

namespace Bindery.Test.Tests
{
    [TestFixture]
    public class ObserveTest
    {
        [TestCase(true, true, true)]
        [TestCase(false, false, false)]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        public void ObserveControlAndSendToCommand(bool commandEnabled, bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Arrange
            var viewModel = new TestViewModel();
            string mouseMoveButton = null;
            viewModel.Command.ExecuteAction = parm => { mouseMoveButton = parm; };
            viewModel.Command.CanExecuteCondition = vm => commandEnabled;

            using (var button = new TestButton())
            using (var binder = Bind.Source(viewModel))
            {
                binder.Control(button).Observe(c => c.MouseMoveButton).Executes(vm => vm.Command);

                // Act
                if (!binderActiveDuringEvent) binder.Dispose();
                button.PerformMouseMove(new MouseEventArgs(MouseButtons.Right, 0, 0, 0, 0));

                var expected = expectUpdated ? "Right" : null;

                // Assert
                Assert.That(mouseMoveButton, Is.EqualTo(expected));
            }
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void ObserveControlAndSetSourceValue(bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Arrange
            var viewModel = new TestViewModel();

            using (var button = new TestButton())
            using (var binder = Bind.Source(viewModel))
            {
                binder.Control(button).Observe(c => c.MouseMoveButton).UpdateSource(vm => vm.StringValue);

                // Act
                if (!binderActiveDuringEvent) binder.Dispose();
                button.PerformMouseMove(new MouseEventArgs(MouseButtons.Right, 0, 0, 0, 0));

                var expected = expectUpdated ? "Right" : null;

                // Assert
                Assert.That(viewModel.StringValue, Is.EqualTo(expected));
            }
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void ObserveSourceAndTakeAction(bool binderActiveDuringEvent, bool expectUpdated)
        {
            // Arrange
            var task = new Task<int>(()=>5);
            var viewModel = new TestViewModel {MyObservable = task.ToObservable()};
            var expected = expectUpdated ? 5 : 0;

            using (var binder = Bind.Source(viewModel))
            {
                var result = 0;
                binder.Observe(vm => vm.MyObservable)
                    .OnNext(arg => result = arg)
                    .OnComplete(() => Assert.That(result, Is.EqualTo(expected)))
                    .Subscribe();
                if (!binderActiveDuringEvent) 
                    binder.Dispose();

                // Act
                task.Start();
                task.Wait();
            }
        }

        [Test]
        public void ObserveSourceWithException()
        {
            // Arrange
            var task = new Task<int>(() => { throw new NotImplementedException(); });
            var viewModel = new TestViewModel { MyObservable = task.ToObservable() };

            using (var binder = Bind.Source(viewModel))
            {
                binder.Observe(vm => vm.MyObservable)
                    .OnNext(arg => { })
                    .OnError(ex=> Assert.That(ex, Is.InstanceOf<NotImplementedException>()))
                    .Subscribe();

                // Act
                task.Start();
                ConditionalWait(() => task.IsFaulted);
            }
        }

        [Test]
        public void ObserveSourceWithCancellation()
        {
            // Arrange
            var tokenSource = new CancellationTokenSource();
            var ct = tokenSource.Token;
            var task = new Task<int>(() => { Thread.Sleep(TimeSpan.FromMilliseconds(10)); ct.ThrowIfCancellationRequested(); return 5; }, tokenSource.Token);
            var viewModel = new TestViewModel { MyObservable = task.ToObservable() };
            const int expected = 5;

            using (var binder = Bind.Source(viewModel))
            {
                var result = 0;
                binder.Observe(vm => vm.MyObservable)
                    .OnNext(arg => result = arg)
                    .CancellationToken(tokenSource.Token)
                    .Subscribe();

                // Act
                task.Start();
                tokenSource.Cancel();
                Assert.Throws<OperationCanceledException>(() => task.Wait(tokenSource.Token));
                Assert.That(result, Is.Not.EqualTo(expected));
            }
        }

        private static void ConditionalWait(Func<bool> condition)
        {
            Task.Run(() =>
            {
                while (!condition())
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(10));
                }
            }).Wait(TimeSpan.FromSeconds(1));
            Assert.That(condition(), Is.True, "Failed to meet condition before timeout.");
        }

    }
}
