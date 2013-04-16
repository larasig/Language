using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Ditw.App.Lang.Tokenizer
{
    public static class TextPreProcessor
    {
        private static readonly Regex SentenceSeparator = new Regex(
            @"(\.\.\.)|(\."")|\.|\?|!"
            );
        private static String[] SentenceSeparatorExceptions = new String[] {
            "U.S.",
            "i.e.",
            "co.",
            "Yahoo!",
        };

        private static Int32 CheckException(String text, Int32 startIndex,
            Int32 matchStartIndex, Int32 matchLength)
        {
            for (Int32 i = 0; i < SentenceSeparatorExceptions.Length; i++)
            {
                Int32 idx = text.IndexOf(SentenceSeparatorExceptions[i], startIndex);
                if (idx >= 0)
                {
                    if (idx <= matchStartIndex &&
                        idx + SentenceSeparatorExceptions[i].Length >= matchStartIndex + matchLength)
                    {
                        Trace.WriteLine("Exception: " + SentenceSeparatorExceptions[i]);
                        return idx + SentenceSeparatorExceptions[i].Length;
                    }
                }
            }
            return -1;
        }

        private static Int32 NextPuncMark(String text, Int32 startIndex)
        {
            var m = SentenceSeparator.Match(text, startIndex);
            Int32 eidx = CheckException(text, startIndex, m.Index, m.Length);
            while (eidx >= 0)
            {
                startIndex = eidx;
                if (startIndex >= text.Length)
                {
                    // Out of range
                    return -1;
                }
                m = SentenceSeparator.Match(text, startIndex);
                eidx = CheckException(text, startIndex, m.Index, m.Length);
            }
            return m.Index + m.Length;
        }

        private static ITextSegment MakeSentence(String text, Int32 startIndex, Int32 length)
        {
            //Trace.WriteLine(text.Substring(startIndex, length));
            return new BasicTextSegment(text, startIndex, length);
        }

        public static IEnumerable<ITextSegment> GetSentences(String text)
        {
            List<ITextSegment> result = new List<ITextSegment>();
            Int32 idx = 0;
            while (idx < text.Length)
            {
                Int32 nextStart = NextPuncMark(text, idx);
                if (nextStart > 0)
                {
                    result.Add(MakeSentence(text, idx, nextStart - idx));
                }
                else
                {
                    result.Add(MakeSentence(text, idx, text.Length - idx));
                    break;
                }
                idx = nextStart;
            }

            return result;
        }
    }
}
