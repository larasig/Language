using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace Ditw.App.Lang.Tokenizer
{
    [XmlRoot("regex")]
    public class RegexXml
    {
        [XmlAttribute("id")]
        public String Id
        {
            get;
            set;
        }

        [XmlText]
        public String Expr
        {
            get;
            set;
        }
    }

    [XmlRoot("regexgroup")]
    public class RegexGroup
    {
        [XmlAttribute("name")]
        public String Name
        {
            get;
            set;
        }

        [XmlElement("regex")]
        public RegexXml[] Regexes
        {
            get;
            set;
        }
    }

    [XmlRoot("regextokens")]
    public class RegexTokenXml
    {
        [XmlAttribute("id")]
        public String Id
        {
            get;
            set;
        }

        [XmlElement("regexgroup")]
        public RegexGroup[] RegexGroups
        {
            get;
            set;
        }

        #region reg expression
        private Regex[] _regexes;
        private Object _lock = new Object();
        private Dictionary<String, String> _regexMapping;
        private static readonly Regex _VariablePattern = new Regex(@"\{\{([^{}]*)}}");
        private const String _ReplaceValuePattern = @"({0})";
        private String ReplaceVariables(String expr)
        {
            var mc = _VariablePattern.Matches(expr);
            Dictionary<String, String> replaceDict = new Dictionary<String, String>(mc.Count);
            for (Int32 i = 0; i < mc.Count; i ++)
            {
                String varName = mc[i].Groups[1].Value;
                if (!replaceDict.ContainsKey(varName))
                {
                    String varValue = String.Format(_ReplaceValuePattern, _regexMapping[varName]);
                    replaceDict[mc[i].Value] = varValue;
                }
            }

            String result = expr;
            foreach (var v in replaceDict.Keys)
            {
                result = result.Replace(v, replaceDict[v]);
            }

            return result;
        }
        private void BuildRegexes()
        {
            if (_regexes == null)
            {
                lock (_lock)
                {
                    if (_regexes == null)
                    {
                        Int32 regexCount = RegexGroups.Sum(rg => rg.Regexes.Length);
                        _regexes = new Regex[regexCount];
                        _regexMapping = new Dictionary<String, String>(regexCount);
                        Int32 regexIndex = 0;
                        for (Int32 i = 0; i < RegexGroups.Length; i++)
                        {
                            var groupi = RegexGroups[i];
                            for (Int32 j = 0; j < groupi.Regexes.Length; j++)
                            {
                                groupi.Regexes[j].Expr = ReplaceVariables(groupi.Regexes[j].Expr);
                                if (!String.IsNullOrEmpty(groupi.Regexes[j].Id))
                                {
                                    _regexMapping.Add(groupi.Regexes[j].Id, groupi.Regexes[j].Expr);
                                }
                                _regexes[regexIndex++] = new Regex(groupi.Regexes[j].Expr);
                            }
                        }
                    }
                }
            }
        }

        [XmlIgnore]
        public Regex[] RegexArray
        {
            get
            {
                if (_regexes == null)
                {
                    BuildRegexes();
                }
                return _regexes;
            }
        }

        #endregion

    }

    public class RegexMatchSegment : BasicTextSegment
    {
        public RegexMatchSegment(
            String source,
            Int32 index,
            Int32 length)
            : base(source, index, length)
        {
        }

        public override bool Dividable
        {
            get
            {
                return false;
            }
        }
    }

    public class RegexToken
    {
        public RegexTokenXml Xml
        {
            get;
            set;
        }

        public ITextSegment MatchText(ITextSegment input)
        {
            if (!input.Dividable)
            {
                return input;
            }

            var dividedList = input.Decompose();
            if (dividedList.Count > 1)
            {
                throw new NotImplementedException();
            }
            else
            {
                // process the text
                var segs = _Match(input.Text);
                CompoundTextSegment cts = new CompoundTextSegment(input.Text, 0, input.Length);
                cts.InsertSegments(segs.ToArray());
                return cts;
            }
        }

        private List<RegexMatchSegment> _Match(String rawText)
        {
            //BuildRegexes();
            
            List<RegexMatchSegment> matchSegments = new List<RegexMatchSegment>();
            for (Int32 i = 0; i < Xml.RegexArray.Length; i++)
            {
                var matches = Xml.RegexArray[i].Matches(rawText);
                for (Int32 j = 0; j < matches.Count; j++)
                {
                    AddMatch(matchSegments, matches[j], rawText);
                }
            }
            return matchSegments;
        }

        private void RemoveShorterMatches(List<RegexMatchSegment> segments, RegexMatchSegment matchSeg)
        {
            List<RegexMatchSegment> newList = new List<RegexMatchSegment>(segments.Count);
            for (Int32 i = 0; i < segments.Count; i++)
            {
                if (!BasicTextSegment.Contain(matchSeg, segments[i]))
                {
                    newList.Add(segments[i]);
                }
            }
            segments.Clear();
            segments.AddRange(newList);
        }

        private void AddMatch(List<RegexMatchSegment> segments, Match m, String src)
        {
            Int32 matchLastIndex = m.Index + m.Length - 1;
            RegexMatchSegment matchSeg = new RegexMatchSegment(src, m.Index, m.Length);
            foreach (var s in segments)
            {
                if (BasicTextSegment.Contain(s, matchSeg))
                {
                    // contained within existing matches, not added!
                    return;
                }
                if (BasicTextSegment.Contain(matchSeg, s))
                {
                    // replace the current one
                    RemoveShorterMatches(segments, matchSeg);
                    break;
                }
                if ((s.StartIndex <= m.Index && m.Index < s.StartIndex + s.Length) ||
                    (s.StartIndex <= matchLastIndex && matchLastIndex < s.StartIndex + s.Length))
                {
                    // overlapp?
                    throw new NotImplementedException();
                }
            }
            segments.Add(matchSeg);
        }
    }
}
