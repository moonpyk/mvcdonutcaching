using System;

namespace DevTrends.MvcDonutCaching.Mlidbom
{
    public interface IDonutBuilder
    {
        void ResultExecuted(bool wasException);
        IDonut CacheAbleValue();
        string PrepareChildOutput(Guid childId, string childOutput);
        void AddChild(IDonut child);
    }
}