using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Vostok.Hosting.Abstractions;
using Vostok.Hosting.Abstractions.Composite;

namespace Vostok.Applications.Scheduled
{
    [PublicAPI]
    public static class ICompositeApplicationBuilderExtensions
    {
        [NotNull]
        public static ICompositeApplicationBuilder AddScheduled(
            [NotNull] this ICompositeApplicationBuilder builder,
            [NotNull] Action<IScheduledActionsBuilder, IVostokHostingEnvironment> setup)
            => builder.AddApplication(new AdHocScheduledApplication(setup));

        [NotNull]
        public static ICompositeApplicationBuilder AddScheduled(
            [NotNull] this ICompositeApplicationBuilder builder,
            [NotNull] Action<IScheduledActionsBuilder, IVostokHostingEnvironment> setup,
            [CanBeNull] Func<IVostokHostingEnvironment, Task> warmupBeforeSetup,
            [CanBeNull] Func<IVostokHostingEnvironment, Task> warmupAfterSetup)
            => builder.AddApplication(new AdHocScheduledApplication(setup, warmupBeforeSetup, warmupAfterSetup));

        [NotNull]
        public static ICompositeApplicationBuilder AddScheduled(
            [NotNull] this ICompositeApplicationBuilder builder,
            [NotNull] Action<IScheduledActionsBuilder> setup)
            => builder.AddApplication(new AdHocScheduledApplication((b, _) => setup(b)));

        [NotNull]
        public static ICompositeApplicationBuilder AddScheduled(
            [NotNull] this ICompositeApplicationBuilder builder,
            [NotNull] Action<IScheduledActionsBuilder> setup,
            [CanBeNull] Func<IVostokHostingEnvironment, Task> warmupBeforeSetup,
            [CanBeNull] Func<IVostokHostingEnvironment, Task> warmupAfterSetup)
            => builder.AddApplication(new AdHocScheduledApplication((b, _) => setup(b), warmupBeforeSetup, warmupAfterSetup));

        private class AdHocScheduledApplication : VostokScheduledApplication
        {
            private readonly Action<IScheduledActionsBuilder, IVostokHostingEnvironment> setup;
            private readonly Func<IVostokHostingEnvironment, Task> warmupBeforeSetup;
            private readonly Func<IVostokHostingEnvironment, Task> warmupAfterSetup;

            public AdHocScheduledApplication(
                Action<IScheduledActionsBuilder, IVostokHostingEnvironment> setup, 
                Func<IVostokHostingEnvironment, Task> warmupBeforeSetup = null, 
                Func<IVostokHostingEnvironment, Task> warmupAfterSetup = null)
            {
                this.setup = setup;
                this.warmupBeforeSetup = warmupBeforeSetup;
                this.warmupAfterSetup = warmupAfterSetup;
            }

            public override void Setup(IScheduledActionsBuilder builder, IVostokHostingEnvironment environment)
                => setup(builder, environment);

            public override Task WarmupBeforeSetupAsync(IVostokHostingEnvironment environment)
                => warmupBeforeSetup == null ? Task.CompletedTask : warmupBeforeSetup(environment);

            public override Task WarmupAfterSetupAsync(IVostokHostingEnvironment environment)
                => warmupAfterSetup == null ? Task.CompletedTask : warmupAfterSetup(environment);
        }
    }
}
