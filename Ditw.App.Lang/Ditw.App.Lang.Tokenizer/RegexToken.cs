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

        [XmlAttribute("isInternal")]
        public String _IsInternal
        {
            get;
            set;
        }

        [XmlIgnore]
        public Boolean IsInternal
        {
            get
            {
                return "0" != _IsInternal;
            }
        }

        [XmlText]
        public String ExprText
        {
            get;
            set;
        }

        [XmlIgnore]
        private Regex _expr;
        public Regex Expr
        {
            get { return _expr; }
        }

        internal void InitializeRegex()
        {
            _expr = new Regex(ExprText);
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
        private RegexXml[] _regexes;
        //private Boolean _initilized = false;
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
                        _regexes = new RegexXml[regexCount];
                        _regexMapping = new Dictionary<String, String>(regexCount);
                        Int32 regexIndex = 0;
                        for (Int32 i = 0; i < RegexGroups.Length; i++)
                        {
                            var groupi = RegexGroups[i];
                            for (Int32 j = 0; j < groupi.Regexes.Length; j++)
                            {
                                groupi.Regexes[j].ExprText = ReplaceVariables(groupi.Regexes[j].ExprText);
                                if (!String.IsNullOrEmpty(groupi.Regexes[j].Id))
                                {
                                    _regexMapping.Add(groupi.Regexes[j].Id, groupi.Regexes[j].ExprText);
                                }
                                groupi.Regexes[j].InitializeRegex();
                                _regexes[regexIndex++] = groupi.Regexes[j];
                            }
                        }

                        //_initilized = true;
                    }
                }
            }
        }

        [XmlIgnore]
        public RegexXml[] RegexArray
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
        public RegexXml RegexDef
        {
            get;
            private set;
        }

        public RegexMatchSegment(
            RegexXml regexDef,
            String source,
            Int32 index,
            Int32 length)
            : base(source, index, length)
        {
            RegexDef = regexDef;
        }

        public override bool IsInternal
        {
            get
            {
                return RegexDef.IsInternal;
            }
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

        private static Boolean MatchContain(Match m1, Match m2)
        {
            return m1.Index <= m2.Index && m1.Index + m1.Length >= m2.Index + m2.Length;
        }

        private List<Match> MergeMatches(Dictionary<Int32, List<Match>> matchList, MatchCollection matchColl)
        {
            List<Match> toRemoveList = new List<Match>();
            List<Match> result = new List<Match>(matchList.Count + matchColl.Count);
            for (Int32 i = 0; i < matchColl.Count; i++)
            {
                Boolean removeCurrent = false;
                foreach (var ml in matchList.Keys)
                {
                    foreach(var m in matchList[ml])
                    {
                        if (MatchContain(matchColl[i], m))
                        {
                            toRemoveList.Add(m);
                        }
                        else if (MatchContain(m, matchColl[i]))
                        {
                            removeCurrent = true;
                            break;
                        }
                    }
                    if (removeCurrent)
                    {
                        break;
                    }
                    else
                    {
                        toRemoveList.ForEach(
                            m => matchList[ml].Remove(m)
                            );
                        toRemoveList.Clear();
                    }
                }
                if (!removeCurrent)
                {
                    result.Add(matchColl[i]);
                }
            }

            return result;
        }

        private List<RegexMatchSegment> _Match(String rawText)
        {
            //BuildRegexes();
            
            List<RegexMatchSegment> matchSegments = new List<RegexMatchSegment>();
            Dictionary<Int32, List<Match>> regexMatches = new Dictionary<Int32, List<Match>>();
            for (Int32 i = 0; i < Xml.RegexArray.Length; i++)
            {
                var matches = Xml.RegexArray[i].Expr.Matches(rawText);
                regexMatches[i] = MergeMatches(regexMatches, matches);
            }
            foreach (var i in regexMatches.Keys)
            {
                foreach (var m in regexMatches[i])
                {
                    AddMatch(Xml.RegexArray[i], matchSegments, m, rawText);
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

        private void AddMatch(RegexXml regex, List<RegexMatchSegment> segments, Match m, String src)
        {
            Int32 matchLastIndex = m.Index + m.Length - 1;
            RegexMatchSegment matchSeg = new RegexMatchSegment(regex, src, m.Index, m.Length);
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
