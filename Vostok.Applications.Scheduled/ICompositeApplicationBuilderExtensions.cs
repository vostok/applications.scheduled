using System;
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
            [NotNull] Action<IScheduledActionsBuilder> setup)
            => builder.AddApplication(new AdHocScheduledApplication((b, _) => setup(b)));

        private class AdHocScheduledApplication : VostokScheduledApplication
        {
            private readonly Action<IScheduledActionsBuilder, IVostokHostingEnvironment> setup;

            public AdHocScheduledApplication(Action<IScheduledActionsBuilder, IVostokHostingEnvironment> setup)
                => this.setup = setup;

            public override void Setup(IScheduledActionsBuilder builder, IVostokHostingEnvironment environment)
                => setup(builder, environment);
        }
    }
}
