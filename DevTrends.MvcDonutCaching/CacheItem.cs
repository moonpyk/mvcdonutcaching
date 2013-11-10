using System;
using System.Runtime.Serialization;

namespace DevTrends.MvcDonutCaching
{
    [Serializable, DataContract]
    public class CacheItem : ICacheItem
    {
        [DataMember(Order = 1)]
        public string ContentType { get; set; }

        [DataMember(Order = 2)]
        public string Content { get; set; }
    }
}
