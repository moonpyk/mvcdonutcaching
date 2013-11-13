using System;

namespace DevTrends.MvcDonutCaching
{
    public static class Contract
    {
        public static class Parameter 
        {
            public static void NotNull(params object[] parameters)
            {
                //Rare case where low level performance optimization matters. This should be callable in extremely tight loops etc. No Linq here please.
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach(var parameter in parameters)
                {
                    if(parameter == null)
                    {
                        throw new ArgumentNullException();
                    }
                }
            }
        }
    }
}