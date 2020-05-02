using Bindery.Tests.TestClasses;
using NUnit.Framework;

namespace Bindery.Tests.Tests
{
    [TestFixture]
    public class ChainingTest
    {
        [Test]
        public void ChainingFromControl()
        {
            // Arrange
            var viewModel = new TestViewModel();
            var control = new TestControl();
            var binder = Binder.Source(viewModel);

            // Act
            binder.Control(control).OnClick().Subscribe(e => { var a = 1; })
                .Target(control).Property(c => c.Text).Get(vm => vm.StringValue)
                .Control(control).Property(c => c.Top).Get(vm => vm.IntValue);
        }
    }
}
