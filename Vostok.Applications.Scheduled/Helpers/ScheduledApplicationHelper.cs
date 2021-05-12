using System;
using System.Threading.Tasks;
using Vostok.Hosting.Abstractions;

namespace Vostok.Applications.Scheduled.Helpers
{
    internal static class ScheduledApplicationHelper
    {
        public static Task<IScheduledActionsRunner> InitializeAsync(IVostokHostingEnvironment environment, Action<IScheduledActionsBuilder, IVostokHostingEnvironment> setup)
        {
            return InitializeAsync(
                environment,
                (builder, env) =>
                {
                    setup(builder, env);
                    return Task.CompletedTask;
                });
        }

        public static async Task<IScheduledActionsRunner> InitializeAsync(IVostokHostingEnvironment environment, Func<IScheduledActionsBuilder, IVostokHostingEnvironment, Task> setup)
        {
            environment.HostExtensions.TryGet<IVostokApplicationDiagnostics>(out var diagnostics);

            var builder = new ScheduledActionsBuilder(environment.Log, environment.Tracer, diagnostics);

            await setup(builder, environment);

            return builder.BuildRunner();
        }
    }
}