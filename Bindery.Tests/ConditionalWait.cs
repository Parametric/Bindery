using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bindery.Tests
{
    public static class ConditionalWait
    {
        public static void WaitFor(Func<bool> condition)
        {
            WaitFor(condition, TimeSpan.FromSeconds(1));
        }

        public static void WaitFor(Func<bool> condition, TimeSpan timeout)
        {
            Task.Run(() =>
            {
                while (!condition())
                    Thread.Sleep(TimeSpan.FromMilliseconds(10));
            }).Wait(timeout);
            if (!condition())
                throw new TimeoutException(string.Format("Failed to meet condition within {0}", timeout));
        }
    }
}
