using System;
using System.IO;
using System.Runtime.Serialization;

namespace DevTrends.MvcDonutCaching
{
    public class DataContractSerializerWrapper : IDataContractSerializer
    {
        private readonly DataContractSerializer _dataContractSerializer;

        public DataContractSerializerWrapper(DataContractSerializer dataContractSerializer)
        {
            if (dataContractSerializer == null) { throw new ArgumentNullException("dataContractSerializer"); }

            _dataContractSerializer = dataContractSerializer;
        }

        public void WriteObject(Stream stream, object graph)
        {
            _dataContractSerializer.WriteObject(stream, graph);
        }

        public object ReadObject(Stream stream)
        {
            return _dataContractSerializer.ReadObject(stream);
        }
    }
}