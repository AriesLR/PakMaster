namespace PakMaster.Core.Extensions
{
    public static class DispatcherExtensions
    {
        // Invoke Safe
        public static void InvokeSafe(this Dispatcher dispatcher, Action action)
        {
            if (dispatcher == null) return;
            if (dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                dispatcher.Invoke(action);
            }
        }

        // Invoke Safe Async
        public static Task InvokeSafeAsync(this Dispatcher dispatcher, Action action)
        {
            if (dispatcher == null) return Task.CompletedTask;
            if (dispatcher.CheckAccess())
            {
                action();
                return Task.CompletedTask;
            }
            else
            {
                return dispatcher.InvokeAsync(action).Task;
            }
        }

        public static async Task InvokeSafeAsync(this Dispatcher dispatcher, Func<Task> action)
        {
            if (dispatcher == null) return;
            if (dispatcher.CheckAccess())
            {
                await action();
            }
            else
            {
                await dispatcher.InvokeAsync(action).Task.Unwrap();
            }
        }

        public static async Task<T> InvokeSafeAsync<T>(this Dispatcher dispatcher, Func<Task<T>> action)
        {
            if (dispatcher == null) return default!;
            if (dispatcher.CheckAccess())
            {
                return await action();
            }
            else
            {
                return await dispatcher.InvokeAsync(action).Task.Unwrap();
            }
        }
    }
}