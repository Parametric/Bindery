﻿using System.Threading;
using System.Threading.Tasks;
using Bindery.Interfaces.Binders;
using Bindery.Tests.TestClasses;
using NUnit.Framework;

namespace Bindery.Tests.Tests
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class OnClickTest
    {
        [SetUp]
        public void BeforeEach()
        {
            _viewModel = new TestViewModel();
            _command = new TestCommand(_viewModel);
            _binder = Create.Binder(_viewModel);
            _control = new TestControl();
            _invoker = new TestInvoker();
            Invoker.Override(_invoker);
        }

        [TearDown]
        public void AfterEach()
        {
            _binder.Dispose();
            _control.Dispose();
        }

        private TestViewModel _viewModel;
        private TestControl _control;
        private ISourceBinder<TestViewModel> _binder;
        private TestCommand _command;
        private TestInvoker _invoker;

        [Test]
        public void ClickingTheControlExecutesTheCommand()
        {
            // Arrange
            var executedCount = 0;
            _command.ExecuteAction = vm => executedCount++;
            _binder.Control(_control).OnClick(_command);

            // Act
            _control.PerformClick();
            _control.PerformClick();

            // Assert
            Assert.That(executedCount, Is.EqualTo(2));
        }

        [Test]
        public void EnableIsUpdatedBasedOnCanExecute()
        {
            // Arrange
            _command.CanExecuteCondition = vm => vm.IntValue > 0;

            // Act & Assert
            _binder.Control(_control).OnClick(_command);
            Assert.That(_control.Enabled, Is.False, "Enabled should be set based off initial condition");
            _viewModel.IntValue = 5;
            ConditionalWait.WaitFor(() => _control.Enabled);
            Assert.That(_control.Enabled, Is.True);
            _viewModel.IntValue = -1;
            ConditionalWait.WaitFor(() => !_control.Enabled);
            Assert.That(_control.Enabled, Is.False);
            _viewModel.IntValue = 5;
            ConditionalWait.WaitFor(() => _control.Enabled);
            Assert.That(_control.Enabled, Is.True);
        }

        [Test]
        public void EnableChangesOccurOnSameThreadWhereBindingOccurred()
        {
            // Arrange
            _command.CanExecuteCondition = vm => vm.IntValue > 0;
            _binder.Control(_control).OnClick(_command);

            // Act
            var task = Task.Factory.StartNew(() => _viewModel.IntValue = 5);
            task.Wait();
            ConditionalWait.WaitFor(()=>_control.Enabled);
            Assert.That(_control.Enabled, Is.True);
            Assert.That(_invoker.Invoked, Is.True);
        }

        [Test]
        public void OnClickObservable()
        {
            // Arrange
            var actionCalled = false;
            _binder.Control(_control).OnClick().Subscribe(e => actionCalled = true);

            // Act
            _control.PerformClick();

            // Assert
            Assert.That(actionCalled, Is.True);
        }

        [Test]
        public void OnClickCommandWithParameter()
        {
            // Arrange
            const string parameter = "test";
            _binder.Control(_control).OnClick(_command, parameter);

            // Act
            _control.PerformClick();

            // Assert
            Assert.That(_command.ExecutionParameter, Is.EqualTo(parameter));
        }

        [Test]
        public void OnClickCommandWithParameterFunction()
        {
            // Arrange
            const string parameter = "test";
            _binder.Control(_control).OnClick(_command, () => parameter);

            // Act
            _control.PerformClick();

            // Assert
            Assert.That(_command.ExecutionParameter, Is.EqualTo(parameter));
        }
    }
}