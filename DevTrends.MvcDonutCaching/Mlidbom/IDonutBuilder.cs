using System;

namespace DevTrends.MvcDonutCaching.Mlidbom
{
    public interface IDonutBuilder
    {
        IDonut GetDonut();
        void ResultExecuted(bool wasException);        
        string PrepareChildOutput(Guid childId, string childOutput);
        void AddChild(IDonut child);
    }
}