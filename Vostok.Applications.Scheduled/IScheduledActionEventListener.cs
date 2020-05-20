using System;
using JetBrains.Annotations;

namespace Vostok.Applications.Scheduled
{
    internal interface IScheduledActionEventListener
    {
        void OnSuccessfulIteration([NotNull] IScheduler source);

        void OnFailedIteration([NotNull] IScheduler source, [NotNull] Exception error);
    }
}
