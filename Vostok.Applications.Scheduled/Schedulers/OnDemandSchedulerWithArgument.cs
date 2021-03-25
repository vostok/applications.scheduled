using System.Threading.Tasks;

namespace Vostok.Applications.Scheduled.Schedulers
{
    internal class OnDemandSchedulerWithArgument<TArg> : OnDemandScheduler
    {
        private TArg lastArgument;
        
        public void Demand(TArg argument)
        {
            lock (Sync)
            {
                lastArgument = argument;
                base.Demand();
            }
        }

        public Task DemandWithFeedback(TArg argument)
        {
            lock (Sync)
            {
                lastArgument = argument;
                return base.DemandWithFeedback();
            }
        }

        public TArg GetLastArgumentValue() => lastArgument;
    }
}