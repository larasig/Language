using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ditw.App.Util.Algorithm;

namespace Ditw.App.Lang.Pattern
{
	public struct MatchInfo
	{
		public String SrcText
		{
			get;
			set;
		}
		
		private Int32 _index;
		public Int32 Index
		{
			get
			{
				if (SubMatches != null && SubMatches.Count > 0)
				{
					return SubMatches[0].Index;
				}
				else
				{
					return _index;
				}
			}
			set
			{
				_index = value;
			}
		}
		
		private Int32 _length;
		public Int32 Length
		{
			get
			{
				if (SubMatches != null && SubMatches.Count > 0)
				{
					MatchInfo miLast = SubMatches.Last();
					return miLast.Index + miLast.Length - SubMatches[0].Index;
				}
				else
				{
					return _length;
				}
			}
			set
			{
				_length = value;
			}
		}

        public Boolean RangeEqual(MatchInfo mi)
        {
            return mi.SrcText.Equals(SrcText, StringComparison.Ordinal)
                && mi.Index == Index
                && mi.Length == Length;
        }

        public Boolean RangeContains(MatchInfo mi)
        {
            return mi.SrcText.Equals(SrcText, StringComparison.Ordinal)
                && Index <= mi.Index
                && Index + Length >= mi.Index + mi.Length;
        }
		
		public List<MatchInfo> SubMatches
		{
			get;
			set;
		}
		
		public List<String> TextsInBetween
		{
			get
			{
				List<String> result = new List<String>();
				
				if (SubMatches == null || SubMatches.Count == 0)
				{
					return result;
				}
				
				var it = SubMatches.GetEnumerator();
				MatchInfo m1;
				if (it.MoveNext())
				{
					m1 = it.Current;
					while (it.MoveNext())
					{
						var m2 = it.Current;
						result.Add(SrcText.Substring(
							m1.Index + m1.Length,
							m2.Index - m1.Index - m1.Length)
						);
					}
				}
				return result;
			}
		}
		
		public String Text
		{
			get { return SrcText.Substring(Index, Length); }
		}
		
		private const Int32 _extraCharCount = 4;
		public String DebugText
		{
			get {
				String prefix = SrcText.Substring(
						(Index >= _extraCharCount ? Index - _extraCharCount : 0),
						(Index >= _extraCharCount ? _extraCharCount : Index)
					);
				String postfix = SrcText.Substring(
						Index + Length,
						(Index + Length + _extraCharCount > SrcText.Length ? 
						 SrcText.Length - Index - Length : _extraCharCount)
					);
				return String.Format("{0}{1}{2}{1}{3}", prefix, '*', Text, postfix);
			}
		}
		
		public void Adjust(Int32 index, Int32 length)
		{
			Index = index;
			Length = length;
		}

        private Boolean CheckIndexes(
            IEnumerable<KeywordWithPositionInfo> tokenPosList,
            Int32 start,
            Int32 length
            )
        {
            Int32 end = start + length - 1;
            Boolean startOk = false, endOk = false;
            foreach (var t in tokenPosList)
            {
                if (t.FirstCharIndex == start
                    || t.LastCharIndex + 1 == start // relax restriction
                    )
                {
                    startOk = true;
                    //continue;
                }
                if (t.LastCharIndex == end
                    || t.FirstCharIndex - 1 == end // relax restriction
                    )
                {
                    endOk = true;
                    //continue;
                }
            }

            return startOk && endOk;
        }

        public Boolean CheckAgainstTokens(IEnumerable<KeywordWithPositionInfo> tokenPosList)
        {
            if (SubMatches == null)
            {
                return CheckIndexes(tokenPosList, Index, Length);
            }

            foreach (var subMatch in SubMatches)
            {
                if (!subMatch.CheckAgainstTokens(tokenPosList))
                {
                    return false;
                }
            }

            return true;
        }
	}
	
	public abstract class ExprBase : IExpr
	{
		public abstract String Text
		{
			get;
		}
		
		private Regex _regex;
		protected virtual Regex RegExpr
		{
			get
			{
				if (_regex == null)
					_regex = new Regex(Text);
				return _regex;
			}
			set
			{
				_regex = value;
			}
		}
		
		public virtual Boolean IsMatch(String text)
		{
			return RegExpr.IsMatch(text);
		}
		
		public virtual IEnumerable<MatchInfo> Match(String text)
		{
			var mc = RegExpr.Matches(text);
			List<MatchInfo> matches = new List<MatchInfo>(mc.Count);
			foreach (Match m in mc)
			{
				matches.Add(
					new MatchInfo()
					{
						SrcText = text,
						Index = m.Index,	
						Length = m.Length
					}
				);
			}
			return matches;
		}

		public Regex ToRegex()
		{
			return new Regex(Text);
		}
	}
}
