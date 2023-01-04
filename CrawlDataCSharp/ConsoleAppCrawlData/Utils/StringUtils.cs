using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppCrawlData.Utils
{
    internal static class StringUtils
    {
        public static string NormalizeString(this string str)
        {
            return str?.Trim()?.Replace("\r", "")?.Replace("\n", "");
        }
    }
}
