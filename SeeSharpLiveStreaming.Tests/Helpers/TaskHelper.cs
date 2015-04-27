using System;
using System.Threading.Tasks;

namespace SeeSharpHttpLiveStreaming.Tests.Helpers
{
    /// <summary>
    /// A helper class for task creation for unit tests.
    /// </summary>
    internal static class TaskHelper
    {
        internal static Task<T> CreateTaskFrom<T>(T result)
        {
            return Task.Delay(10).ContinueWith(res => result);
        }

        internal static Task<T> CreateCompletedTaskFrom<T>(T result)
        {
            return Task.FromResult(result);
        }

        internal static Task<T> ThrowFromTask<T, TException>(T result) where TException : Exception
        {
            return Task.Delay(10).ContinueWith<T>(res =>
            {
                throw Activator.CreateInstance<TException>();
            });
        }
    }
}
