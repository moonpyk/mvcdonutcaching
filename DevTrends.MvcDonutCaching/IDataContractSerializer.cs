using System.IO;

namespace DevTrends.MvcDonutCaching
{
    public interface IDataContractSerializer
    {
        void WriteObject(Stream stream, object graph);
        object ReadObject(Stream stream);
    }
}