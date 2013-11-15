using System;

namespace DevTrends.MvcDonutCaching.Mlidbom
{
    /// <summary/>    
    public static class EnumExtensions
    {
        /// <summary>
        /// True if <paramref name="value"/> contains the bit flag <paramref name="flag"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static bool HasFlag(this Enum value, Enum flag)
        {
            Contract.Parameter.NotNull(value, flag);

            if (!value.GetType().Equals(flag.GetType()))
            {
                throw new ArgumentOutOfRangeException();
            }
            var longValue = Convert.ToInt64(value);
            var longFlag = Convert.ToInt64(flag);

            return (longValue & longFlag) == longFlag;
        }
    }
}