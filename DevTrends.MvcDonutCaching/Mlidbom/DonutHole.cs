using System;
using System.Text.RegularExpressions;

namespace DevTrends.MvcDonutCaching.Mlidbom
{
    //todo: rename
    public static class DonutHole
    {
        private const string DonutHoleStart = "#StartDonut#18C1E8F8-B296-44BF-A768-89D4F41D14A6#ID:";
        private const string DonutHoleMiddle = "#18C1E8F8-B296-44BF-A768-89D4F41D14A6#";
        private const string DonutHoleEnd = "#18C1E8F8-B296-44BF-A768-89D4F41D14A6#EndDonut#";

        private static readonly string DonutCreationPattern = string.Format("{0}{{0}}{1}{{1}}{2}", DonutHoleStart, DonutHoleMiddle, DonutHoleEnd);

        private static readonly string DonutHolesPattern = string.Format("{0}(.*?){1}(.*?){2}", DonutHoleStart, DonutHoleMiddle, DonutHoleEnd);

        private static readonly Regex DonutHolesRegexp = new Regex(DonutHolesPattern, RegexOptions.Compiled | RegexOptions.Singleline);

        public static string WrapInDonut(Guid donutId, string output)
        {
            return string.Format(DonutCreationPattern, donutId, output);
        }

        //todo:Encapsulate regexp logic
        public static MatchCollection FindDonuts(string content)
        {
            return DonutHolesRegexp.Matches(content);
        }
    }
}