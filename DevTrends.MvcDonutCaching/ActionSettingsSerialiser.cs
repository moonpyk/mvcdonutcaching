using System;
using System.IO;
using System.Text;

namespace DevTrends.MvcDonutCaching
{
    public class ActionSettingsSerialiser : IActionSettingsSerialiser
    {
        private readonly IDataContractSerializer _dataContractSerializer;

        public ActionSettingsSerialiser(IDataContractSerializer dataContractSerializer)
        {
            if (dataContractSerializer == null) { throw new ArgumentNullException("dataContractSerializer"); }

            _dataContractSerializer = dataContractSerializer;
        }

        public string Serialise(ActionSettings actionSettings)
        {
            if (actionSettings == null) { throw new ArgumentNullException("actionSettings"); }

            using (var memoryStream = new MemoryStream())
            {
                _dataContractSerializer.WriteObject(memoryStream, actionSettings);

                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }

        public ActionSettings Deserialise(string serialisedActionSettings)
        {
            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(serialisedActionSettings)))
            {
                return (ActionSettings) _dataContractSerializer.ReadObject(memoryStream);
            }
        }
    }
}