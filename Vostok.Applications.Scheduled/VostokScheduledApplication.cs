using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Vostok.Hosting.Abstractions;

namespace Vostok.Applications.Scheduled
{
    [PublicAPI]
    public class VostokScheduledApplication : IVostokApplication
    {
        public Task InitializeAsync(IVostokHostingEnvironment environment) =>
            throw new NotImplementedException();

        public Task RunAsync(IVostokHostingEnvironment environment) =>
            throw new NotImplementedException();
    }
}
