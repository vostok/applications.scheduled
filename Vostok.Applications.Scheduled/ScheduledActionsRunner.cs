using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Commons.Helpers.Extensions;

// ReSharper disable AccessToDisposedClosure

namespace Vostok.Applications.Scheduled
{
    internal class ScheduledActionsRunner : IScheduledActionsRunner
    {
        private readonly IReadOnlyList<ScheduledActionRunner> runners;

        public ScheduledActionsRunner(IReadOnlyList<ScheduledActionRunner> runners)
            => this.runners = runners;

        public async Task RunAsync(CancellationToken token)
        {
            using (var sharedCancellation = new CancellationTokenSource())
            using (var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(token, sharedCancellation.Token))
            {
                var runnerTasks = runners.Select(runner => runner.RunAsync(linkedCancellation.Token));
                var runnerTasksSilent = runnerTasks.Select(task => task.SilentlyContinue());

                var firstCompletedTask = await Task.WhenAny(runnerTasks).ConfigureAwait(false);

                sharedCancellation.Cancel();

                await Task.WhenAll(runnerTasksSilent).ConfigureAwait(false);

                try
                {
                    await firstCompletedTask.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                }
            }
        }
    }
}
