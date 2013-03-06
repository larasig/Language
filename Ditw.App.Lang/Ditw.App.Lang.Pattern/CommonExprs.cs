/*
 * Created by SharpDevelop.
 * User: jiajiwu
 * Date: 12/29/2012
 * Time: 9:18 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ditw.App.Lang.Pattern
{
	/// <summary>
	/// Description of CommonExprs.
	/// </summary>
	public static class CommonExprs
	{
		#region Zero or More
		/// <summary>
		/// Zero or more (*)
		/// </summary>
		public class ZeroOrMore : ExprBase
		{
			private ExprBase _expr;
			
			internal ZeroOrMore(ExprBase expr)
			{
				_expr = expr;
			}
			
			public override String Text
			{
				get
				{
					return String.Format("({0})*", _expr.Text);
				}
			}
			
//			public override Boolean Match(String text)
//			{
//				return null != Regex.Match(text, Text);
//			}
		}
		
		public static ZeroOrMore ZOM(Char c)
		{
			return new ZeroOrMore(LitExpr.FromChar(c));
		}
		
		public static ZeroOrMore ZOM(String word)
		{
			return new ZeroOrMore(LitExpr.FromString(word));
		}
		#endregion
		
		#region Any char
		/// <summary>
		/// Any char
		/// </summary>
		public class AnyChar : ExprBase
		{			
			public override String Text
			{
				get
				{
					return ".*";
				}
			}
			
//			public override Boolean Match(String text)
//			{
//				return true;
//			}
		}
		
		public readonly static AnyChar AC = new AnyChar();
		#endregion

        #region No Char
        public class NoChar : ExprBase
        {
            public override IEnumerable<MatchInfo> Match(string text)
            {
                return base.Match(text);
            }

            public override string Text
            {
                get { return string.Empty; }
            }

            public override bool IsMatch(string text)
            {
                return text == String.Empty;
            }
        }

        public readonly static NoChar NC = new NoChar();
        #endregion

        #region Word Non-CHAR Word Non-CHAR Word
        public static ExprBase CreatePtnWNW(
			String separators,
			params String[][] wordgroups
		)
		{
			if (wordgroups.Length < 2)
			{
				throw new ArgumentException("wordgroups");
			}
			
			StringBuilder builder = new StringBuilder();
			
			String sep = String.Format("[^({0})]*", separators);
			
			builder.AppendFormat(
				"({0})", CommonFuncs.RegexStringFromWords(wordgroups[0]));
			
			for (Int32 i = 1; i < wordgroups.Length; i++)
			{
				CommonFuncs.RegexStringFromWords(wordgroups[i]);
				
				builder.AppendFormat("({0})({1})",
					sep, CommonFuncs.RegexStringFromWords(wordgroups[i])
				);
			}
			
			RegexExpr n = new RegexExpr(builder.ToString());

			return n;
		}
		
		
		public static ExprBase CreatePtnWNW(
			String[] separators,
			params String[][] wordgroups
		)
		{
			if (wordgroups.Length < 2)
				throw new ArgumentException("wordgroups");
		
			NotExpr notExpr = new NotExpr(
				LitExpr.FromStrings(separators)
			);
			
			List<ExprBase> exprs = new List<ExprBase>(wordgroups.Length*2 - 1);
			exprs.Add(LitExpr.FromStrings(wordgroups[0]));
			
			for (Int32 i = 1; i < wordgroups.Length; i ++)
			{
				exprs.Add(notExpr);
				exprs.Add(LitExpr.FromStrings(wordgroups[i]));
			}
			
			return new AndExpr(
				null,
				exprs.ToArray()
			);
		}
		#endregion
	}
}
