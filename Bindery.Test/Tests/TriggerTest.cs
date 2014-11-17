using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Bindery.Test.Tests
{
    [TestFixture]
    public class TriggerTest
    {
        [Test]
        public void Push()
        {
            // Arrange
            var trigger = Create.Trigger<int>();
            var list = new List<int>();
            using (trigger.Observable.Subscribe(list.Add))
            {
                trigger.Push(1);
                trigger.Push(2);
                Assert.That(list, Is.EquivalentTo(new[] { 1, 2 }));
            }
        }

        [Test]
        public void PushWithDispose()
        {
            // Arrange
            var trigger = Create.Trigger<int>();
            var list = new List<int>();
            using (var subscription = trigger.Observable.Subscribe(list.Add))
            {
                trigger.Push(1);
                subscription.Dispose();
                trigger.Push(2);
                Assert.That(list, Is.EquivalentTo(new[] { 1 }));
            }
        }

        [Test]
        public void MultipleSubscriptions()
        {
            // Arrange
            var trigger = Create.Trigger<int>();
            var list1 = new List<int>();
            var list2 = new List<int>();
            using (var subscription1 = trigger.Observable.Subscribe(list1.Add))
            using (trigger.Observable.Subscribe(list2.Add))
            {
                trigger.Push(1);
                subscription1.Dispose();
                trigger.Push(2);
                Assert.That(list1, Is.EquivalentTo(new[] { 1 }));
                Assert.That(list2, Is.EquivalentTo(new[] { 1, 2 }));
            }
        }
    }
}
