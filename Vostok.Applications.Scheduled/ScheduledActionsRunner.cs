using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vostok.Commons.Helpers.Extensions;
using Vostok.Logging.Abstractions;

// ReSharper disable AccessToDisposedClosure

namespace Vostok.Applications.Scheduled
{
    internal class ScheduledActionsRunner : IScheduledActionsRunner
    {
        private readonly IReadOnlyList<ScheduledActionRunner> runners;
        private readonly ILog log;

        public ScheduledActionsRunner(IReadOnlyList<ScheduledActionRunner> runners, ILog log)
        {
            this.runners = runners;
            this.log = log;
        }

        public IEnumerable<ScheduledActionRunner> Runners => runners;

        public async Task RunAsync(CancellationToken token)
        {
            if (!runners.Any())
            {
                log.Warn("No actions have been scheduled: nothing to run.");
                return;
            }

            using (var sharedCancellation = new CancellationTokenSource())
            using (var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(token, sharedCancellation.Token))
            {
                var runnerTasks = runners.Select(runner => runner.RunAsync(linkedCancellation.Token)).ToList();
                var runnerTasksSilent = runnerTasks.Select(task => task.SilentlyContinue());

                var firstCompletedTask = await Task.WhenAny(runnerTasks);

                sharedCancellation.Cancel();

                await Task.WhenAll(runnerTasksSilent);

                try
                {
                    await firstCompletedTask;
                }
                catch (OperationCanceledException)
                {
                }
            }
        }
    }
}