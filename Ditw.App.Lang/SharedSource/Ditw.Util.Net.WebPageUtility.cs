using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ditw.Util.Net
{
    public static class WebPageUtility
    {
        private const String CharSetString = "charset=";

        public static readonly String[] AcceptedCharsets =
        {
            "utf-8",
            "gb2312",
            "gbk",
            "big5"
        };

        public static String GetPageCharset(String pageSource)
        {
            return GetPageCharset(pageSource, AcceptedCharsets);
        }

        public static String GetPageCharset(String pageSource, String[] acceptedCharsets)
        {
            Int32 idx = pageSource.IndexOf(CharSetString);
            if (idx >= 0)
            {
                idx += CharSetString.Length;
                Int32 minLen = Math.Min(32, pageSource.Length - idx);

                String contentCs = pageSource.Substring(idx, minLen); // temp longest

                foreach (String cs in acceptedCharsets)
                {
                    if (contentCs.StartsWith(cs, StringComparison.OrdinalIgnoreCase) ||
                        contentCs.StartsWith(String.Format("\"{0}\"", cs), StringComparison.Ordinal) ||
                        contentCs.StartsWith(String.Format("'{0}'", cs), StringComparison.Ordinal)
                        )
                    {
                        return cs;
                    }
                }
            }
            return String.Empty;
        }
    }
}
