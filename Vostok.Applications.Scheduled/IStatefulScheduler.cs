namespace Vostok.Applications.Scheduled
{
    internal interface IStatefulScheduler : IScheduler
    {
        void TryCopyStateFrom(IStatefulScheduler other);
    }
}