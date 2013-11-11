using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DevTrends.MvcDonutCaching.Demo.Lib
{
    public class ProtoBufDonutCacheAttribute : DonutOutputCacheAttribute
    {
        public ProtoBufDonutCacheAttribute()
            : this(new KeyBuilder())
        {
        }

        public ProtoBufDonutCacheAttribute(IKeyBuilder b)
            : base(
                new KeyGenerator(b),
                new OutputCacheManager(OutputCache.Instance, b), new DonutHoleFiller(new EncryptingActionSettingsSerialiser(new ProtobufActionSettingsSerialiser(), new Encryptor())),
                new CacheSettingsManager(),
                new CacheHeadersHelper()
                )
        {
        }
    }
}