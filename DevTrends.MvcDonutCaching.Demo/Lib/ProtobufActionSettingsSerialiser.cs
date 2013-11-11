using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.Web.Routing;
using ProtoBuf;

namespace DevTrends.MvcDonutCaching.Demo.Lib
{
    public class ProtobufActionSettingsSerialiser : IActionSettingsSerialiser
    {
        static ProtobufActionSettingsSerialiser()
        {
            HtmlHelperExtensions.Serialiser = new EncryptingActionSettingsSerialiser(new ProtobufActionSettingsSerialiser(), new Encryptor());
        }

        public string Serialise(ActionSettings actionSettings)
        {
            using (var m = new MemoryStream())
            {
                Serializer.Serialize(m, new ProtobufActionSettings(actionSettings));
                return Encoding.UTF8.GetString(m.ToArray());
            }
        }

        public ActionSettings Deserialise(string serialisedActionSettings)
        {
            var b = Encoding.UTF8.GetBytes(serialisedActionSettings);

            using (var m = new MemoryStream(b))
            {
                var a = Serializer.Deserialize<ProtobufActionSettings>(m);

                return a.ToActionSettings();
            }
        }

        [DataContract]
        private class ProtobufActionSettings
        {
            public ProtobufActionSettings()
            { }

            public ProtobufActionSettings(ActionSettings s)
            {
                ActionName = s.ActionName;
                ControllerName = s.ControllerName;

                if (s.RouteValues == null)
                {
                    return;
                }

                RoutesValues = new Dictionary<string, string>();
                foreach (var rv in s.RouteValues)
                {
                    RoutesValues[rv.Key] = rv.Value.ToString();
                }
            }

            [DataMember(Order = 1)]
            public string ControllerName { get; set; }

            [DataMember(Order = 2)]
            public string ActionName { get; set; }

            [DataMember(Order = 3)]
            public IDictionary<string, string> RoutesValues
            {
                get;
                set;
            }

            public ActionSettings ToActionSettings()
            {
                var retVal = new ActionSettings
                {
                    ActionName = ActionName,
                    ControllerName = ControllerName,
                };

                if (RoutesValues == null)
                {
                    return retVal;
                }

                retVal.RouteValues = new RouteValueDictionary();
                foreach (var rv in RoutesValues)
                {
                    retVal.RouteValues.Add(rv.Key, rv.Value);
                }
                return retVal;
            }
        }
    }
}