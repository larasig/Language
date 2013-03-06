using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ditw.Util.Algorithm
{
    public static class StringUtil
    {
        public static Int32 LevenshteinDistance(String s1, String s2)
        {
            Int32[,] d = new Int32[s2.Length + 1, s1.Length + 1];
            for (Int32 i = 0; i < s1.Length; i++)
            {
                d[0, i] = i;
            }
            for (Int32 i = 0; i < s2.Length; i++)
            {
                d[i, 0] = i;
            }

            for (Int32 i = 1; i <= s2.Length; i++)
            {
                for (Int32 j = 1; j <= s1.Length; j++)
                {
                    Int32 c = s2[i - 1] == s1[j - 1] ? 0 : 1;
                    d[i, j] = Math.Min(
                        d[i - 1, j] + 1,
                        Math.Min(
                            d[i, j - 1] + 1,
                            d[i - 1, j - 1] + c
                            )
                        );
                }
            }

            return d[s2.Length - 1, s1.Length - 1];
        }
    }
}
