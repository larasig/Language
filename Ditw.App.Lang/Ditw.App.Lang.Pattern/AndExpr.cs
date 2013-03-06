/*
 * Created by SharpDevelop.
 * User: jiajiwu
 * Date: 12/25/2012
 * Time: 8:41 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace Ditw.App.Lang.Pattern
{
    public class AndExprN : ExprBase
    {
        private AndExpr _andExpr;
        private Int32 _n;

        public override string Text
        {
            get { return String.Empty; }
        }

        public AndExprN(AndExpr andExpr, Int32 n)
        {
            _andExpr = andExpr;
            _n = n;
        }

        public override IEnumerable<MatchInfo> Match(string text)
        {
            var matches = _andExpr.Match(text);
            var checkedMatches = new List<MatchInfo>(matches.Count());
            foreach (var match in matches)
            {
                var m1 = match.SubMatches[0];
                var m2 = match.SubMatches[1];
                if (m2.Index - m1.Index -m1.Length <= _n)
                {
                    checkedMatches.Add(match);
                }
            }
            return checkedMatches;
        }
    }
	/// <summary>
	/// Description of ExprAnd.
	/// </summary>
	public class AndExpr : ExprBase
	{
		private ExprBase[] _expressions;
		private ExprBase _textAllowedInBetween;
		
		public AndExpr(ExprBase textInBetween, params ExprBase[] exprs)
		{
			if (exprs.Length <= 1)
			{
				throw new ArgumentException("Parameter 'exprs' should at least contain 2 expressions!");
			}
			
			_textAllowedInBetween = textInBetween == null ?
				CommonExprs.AC // any char 
				:
				textInBetween;
			
			_expressions = exprs;
		}
		
		public override String Text
		{
			get
			{
				StringBuilder builder = new StringBuilder();
				
				builder.AppendFormat(
					"({0})", _expressions[0].Text);
				
				for (Int32 i = 1; i < _expressions.Length; i++)
				{
					builder.AppendFormat(
						"({0})({1})", _textAllowedInBetween.Text, _expressions[i].Text);
				}
				
				//Trace.WriteLine(builder.ToString());
				return builder.ToString();
			}
		}
		
		private Boolean IsNotExpr(ExprBase expr)
		{
			return expr is NotExpr;
		}
		
		private List<MatchInfo> RecursiveExprArrayMatch(
			String text,
			Int32 textStartIndex,
			Int32 exprStartIndex,
			MatchInfo prevMatch
		)
		{
			List<MatchInfo> currMatches = new List<MatchInfo>();
			if (exprStartIndex == _expressions.Length)
			{
				currMatches.Add(prevMatch);
			}
			else
			{
				ExprBase firstExpr = _expressions[exprStartIndex];
				if (IsNotExpr(firstExpr))
				{
					// skip not expression and handle it at the end.
					return RecursiveExprArrayMatch(
						text,
						textStartIndex,
						exprStartIndex+1,
						prevMatch
					);
				}
				
				String remText = text.Substring(textStartIndex);
				if (!firstExpr.IsMatch(remText))
				{
					return null;
				}
				foreach (MatchInfo m in firstExpr.Match(remText))
				{
					//String subText = text.Substring(m.Index + m.Length);
					MatchInfo currMatch = new MatchInfo()
					{
						SrcText = prevMatch.SrcText,
						Index = prevMatch.Index,
						Length = prevMatch.Length,
						SubMatches = new List<MatchInfo>(prevMatch.SubMatches)
					};
					var m2 = m;
					m2.Index += textStartIndex;
					m2.SrcText = text;
					currMatch.SubMatches.Add(m2);
					List<MatchInfo> subMatch = RecursiveExprArrayMatch(
						text,
						m.Index + m.Length + textStartIndex,
						exprStartIndex + 1,
						currMatch
					);
					if (subMatch != null)
					{
						currMatches.AddRange(subMatch);
					}
				}
			}
			
			return currMatches;
		}
		
		private Boolean CheckMatchAgainstNotExpressions(MatchInfo mi)
		{
			Int32 notExprCount = 0;
			for (Int32 i = 1; i < _expressions.Length - 1; i++)
			{
				ExprBase exp = _expressions[i];
				if (!IsNotExpr(exp))
				{
					continue;
				}
				
				notExprCount ++;
				Int32 idxLeft = i - notExprCount;
				MatchInfo miSubLeft = mi.SubMatches[idxLeft];
				MatchInfo miSubRight = mi.SubMatches[idxLeft+1];
				String toTest = mi.SrcText.Substring(
					miSubLeft.Index + miSubLeft.Length,
					miSubRight.Index - miSubLeft.Index + miSubLeft.Length
				);
				if (!exp.IsMatch(toTest))
				{
					return false;
				}
			}
			return true;
		}
		
		private List<MatchInfo> CheckAgainstNotExpressions(List<MatchInfo> primResult)
		{
			List<MatchInfo> result = new List<MatchInfo>(primResult.Count);
			
			foreach (var mi in primResult)
			{
				if (CheckMatchAgainstNotExpressions(mi))
				{
					result.Add(mi);
				}
			}
			return result;
		}
		
		public override IEnumerable<MatchInfo> Match(String text)
		{
			List<MatchInfo> rt = RecursiveExprArrayMatch(
				text,
				0, // text start index
				0, // expression start index
				new MatchInfo()
				{
					SrcText = text,
					SubMatches = new List<MatchInfo>()
				}
			);
			
//			rt[0].
//			rt.ForEach(
//				mi =>
//				{
//					MatchInfo miLast = mi.SubMatches[mi.SubMatches.Count-1];
//					mi.Adjust(mi.SubMatches[0].Index
//					mi.Length = miLast.Index + miLast.Length - mi.SubMatches[0].Index;
//				}
//			);
			
			if (rt != null && rt.Count > 0)
			{
				return CheckAgainstNotExpressions(rt);
			}
			return new List<MatchInfo>();
		}
		
		public override Boolean IsMatch(String text)
		{
			throw new NotImplementedException();
		}
	}
}
