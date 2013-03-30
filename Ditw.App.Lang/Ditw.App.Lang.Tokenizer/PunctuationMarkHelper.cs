using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Ditw.App.Lang.Pattern;

namespace Ditw.App.Lang.Tokenizer
{
    public class TextSegment
    {
        public Int32 StartIndex;
        public Int32 Length;

        public Nullable<Char> OpenMark;
        public Nullable<Char> CloseMark;

        public Int32 EndIndex
        {
            get { return StartIndex + Length - 1; }
        }

        public String Source;

        public String FullText
        {
            get { return Source.Substring(StartIndex, Length); }
        }

        public String RawText
        {
            get
            {
                Int32 startIdx = StartIndex;
                Int32 len = Length;
                if (OpenMark.HasValue)
                {
                    startIdx ++;
                    len --;
                }
                if (CloseMark.HasValue)
                {
                    len --;
                }
                return Source.Substring(startIdx, len);
            }
        }

        public List<TextSegment> ChildSegments
        {
            get;
            set;
        }

        public void ChildSegmentPass()
        {
            if (ChildSegments == null)
                return;
            foreach (var cs in ChildSegments)
            {
                cs.ChildSegments = PunctuationMarkHelper.OnePass(cs.RawText)
                    .Where(s => s.Length < cs.RawText.Length)
                    .ToList();
            }
        }

        #region DEBUG
        public void TraceOutput()
        {
            Trace.Write("{ {");
            Trace.Write(FullText);
            Trace.Write("} {");
            Trace.Write(RawText);
            Trace.Write("} ");
            if (ChildSegments != null && ChildSegments.Count > 0)
            {
                Trace.Write("CHILDREN ----");
                ChildSegments.ForEach(
                    s => s.TraceOutput()
                    );
            }
            Trace.WriteLine("} ");
        }
        #endregion
    }

    public static class PunctuationMarkHelper
    {
#region Punctuation Marks
        public const Char COMMA = ',';
        public const Char COMMA_CHINESE = '，';

        public const Char PERIOD = '.';
        public const Char PERIOD_CHINESE = '。';

        public const Char SEMICOLON = ';';
        public const Char SEMICOLON_CHINESE = '；';

        public const Char COLON = ':';
        public const Char COLON_CHINESE = '：';

        public const Char QUESTION = '?';
        public const Char QUESTION_CHINESE = '？';

        public const Char EXCLAMATION = '!';
        public const Char EXCLAMATION_CHINESE = '！';

        public const Char QUOTATIONII = '"';
        public const Char QUOTATIONII_CHINESE_OPEN = '“';
        public const Char QUOTATIONII_CHINESE_CLOSE = '”';

        public const Char QUOTATIONI = '\'';
        public const Char QUOTATIONI_CHINESE_OPEN = '‘';
        public const Char QUOTATIONI_CHINESE_CLOSE = '’';

        public const Char TITLE_CHINESE_OPEN = '《';
        public const Char TITLE_CHINESE_CLOSE = '》';

        public const Char BRACKET_OPEN = '(';
        public const Char BRACKET_CLOSE = ')';
        public const Char BRACKET_CHINESE_OPEN = '（';
        public const Char BRACKET_CHINESE_CLOSE = '）';

        public const Char SQUARE_BRACKET_OPEN = '[';
        public const Char SQUARE_BRACKET_CLOSE = ']';
        public const Char SQUARE_BRACKET_CHINESE_OPEN = '【';
        public const Char SQUARE_BRACKET_CHINESE_CLOSE = '】';

        public const Char DOT_CHINESE = '、';

#endregion

        private static readonly RegexExpr REGEX_NUMBER = new RegexExpr(@"\d*\.\d+");

        #region DEBUG pre-processing
        public static void TracePreProcess(String rawText)
        {
            Trace.WriteLine(rawText);
            foreach (var m in REGEX_NUMBER.Match(rawText))
            {
                Trace.WriteLine(m.Text);
            }
            Trace.WriteLine("-----------------------------------");
            Trace.WriteLine(String.Empty);
        }
        #endregion

        public static readonly Char[] SingleSeparators = new Char[]
        {
            COMMA,
            COMMA_CHINESE,
            COLON,
            COLON_CHINESE,
            SEMICOLON,
            SEMICOLON_CHINESE,
            PERIOD,
            PERIOD_CHINESE,
            QUESTION,
            QUESTION_CHINESE,
            EXCLAMATION,
            EXCLAMATION_CHINESE
        };

        public static readonly Dictionary<Char, Char> MarkPairs = new Dictionary<Char, Char>()
        {
            {QUOTATIONI_CHINESE_OPEN, QUOTATIONI_CHINESE_CLOSE},
            {QUOTATIONII_CHINESE_OPEN, QUOTATIONII_CHINESE_CLOSE},
            {TITLE_CHINESE_OPEN, TITLE_CHINESE_CLOSE},
            {BRACKET_OPEN, BRACKET_CLOSE},
            {BRACKET_CHINESE_OPEN, BRACKET_CHINESE_CLOSE},
            {SQUARE_BRACKET_OPEN, SQUARE_BRACKET_CLOSE},
            {SQUARE_BRACKET_CHINESE_OPEN, SQUARE_BRACKET_CHINESE_CLOSE},
            {QUOTATIONI, QUOTATIONI},
            {QUOTATIONII, QUOTATIONII},
        };

        internal static IList<TextSegment> OnePass(String text)
        {
            Int32 currIndex = 0;
            //Int32 prevIndex = 0;
            List<TextSegment> segList = new List<TextSegment>();
            TextSegment currSeg = new TextSegment()
            {
                StartIndex = 0,
                Source = text
            };
            while (currIndex < text.Length)
            {
                Char currChar = text[currIndex];
                Int32 endIndex = -1;
                if (SingleSeparators.Contains(currChar))
                {
                    endIndex = currIndex;

                    Int32 len = endIndex - currSeg.StartIndex;
                    if (len > 0)
                    {
                        currSeg.Length = len;
                        if (currSeg.ChildSegments != null)
                        {
                            currSeg.ChildSegments.ForEach(
                                s => s.Source = currSeg.FullText
                                );
                        }
                        segList.Add(currSeg);
                        currSeg = new TextSegment();
                        currSeg.Source = text;
                        currSeg.OpenMark = currChar;
                        currSeg.StartIndex = currIndex;
                    }
                    else
                    {
                        Trace.WriteLine("Empty segment!");
                    }
                    currIndex++;
                }
                else
                {
                    if (MarkPairs.ContainsKey(currChar))
                    {
                        // if we found "(...)" or '"..."', etc., make it a sub-segment of the current.
                        Int32 closeIndex = text.IndexOf(MarkPairs[currChar], currIndex + 1);
                        Boolean hasClosingMark = true;
                        if (closeIndex < 0)
                        {
                            closeIndex = text.Length; // the rest of the whole string
                            hasClosingMark = false;
                        }
                        else
                        {
                            closeIndex++; // including the closing punctuation mark also
                        }

                        if (currSeg.ChildSegments == null)
                        {
                            currSeg.ChildSegments = new List<TextSegment>();
                        }
                        currSeg.ChildSegments.Add(
                            new TextSegment()
                            {
                                OpenMark = currChar,
                                CloseMark = hasClosingMark ?
                                    new Nullable<Char>(MarkPairs[currChar]) : null,
                                StartIndex = currIndex - currSeg.StartIndex, // relative to parent source 
                                Length = closeIndex - currIndex
                            }
                            );

                        currIndex = closeIndex;
                    }
                    else
                    {
                        currIndex++;
                    }
                }

            }

            Int32 leftLen = text.Length - currSeg.StartIndex;
            if (leftLen > 0)
            {
                currSeg.Length = leftLen;
                // remove the last segment, which contains only a period, question, ...
                if (!String.IsNullOrEmpty(currSeg.RawText))
                {
                    if (currSeg.ChildSegments != null)
                    {
                        currSeg.ChildSegments.ForEach(
                            s => s.Source = currSeg.FullText
                            );
                    }
                    segList.Add(currSeg);
                }
            }

            return segList;
        }

#if false
        private static IList<TextSegment> FirstPass(String text)
        {
            String[] segments = text.Split(SingleSeparators, StringSplitOptions.RemoveEmptyEntries);

            List<TextSegment> segList = new List<TextSegment>(segments.Length);
            Int32 currIndex = 0;
            for (Int32 i = 0; i < segments.Length; i++)
            {
                Int32 hitIndex = text.IndexOf(segments[i], currIndex);
                if (hitIndex >= 0)
                {
                    TextSegment newSeg = new TextSegment()
                        {
                            StartIndex = hitIndex,
                            Length = segments[i].Length,
                            Source = text
                        };
                    currIndex = newSeg.EndIndex + 1;
                    segList.Add(newSeg);
                }
                else
                {
                    throw new Exception("Unexpected!");
                }
            }
            return segList;
        }
#endif
        public static IList<TextSegment> Segmentation(String text)
        {
            var segList = OnePass(text);
            foreach (var s in segList)
            {
                s.ChildSegmentPass();
            }
            return segList;
        }

        #region DEBUG
        public static void TraceSegmentation(String text)
        {
            Trace.WriteLine(text);
            foreach (var s in Segmentation(text))
            {
                s.TraceOutput();
            }
            Trace.WriteLine(String.Empty);
        }
        #endregion
    }
}
