using System;

namespace DevTrends.MvcDonutCaching
{
    [Flags]
    public enum OutputCacheOptions
    {
        None                  = 0x0,
        IgnoreQueryString = 0x1,
        IgnoreFormData        = 0x2,
    }
}
