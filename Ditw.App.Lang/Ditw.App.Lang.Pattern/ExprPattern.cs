/*
 * Created by SharpDevelop.
 * User: jiajiwu
 * Date: 1/5/2013
 * Time: 7:15 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace Ditw.App.Lang.Pattern
{
	public static class ExprPatternID
	{
		// Chinese
		public const Int32 CEO_1 = 10100;
	}

	/// <summary>
	/// Description of ExprPattern.
	/// </summary>
	public class ExprPattern
	{
		public Int32 Id
		{
			get;
			set;
		}
		
		public String Desc
		{
			get;
			set;
		}
		
		public ExprBase Expr
		{
			get;
			set;
		}
		public ExprPattern(Int32 id, ExprBase expr)
		{
			Id = id;
			Expr = expr;
			Desc = expr.Text;// temp
		}
		
		public void Apply(String text, Action<IEnumerable<MatchInfo>> handleMatches)
		{
			String[] sentences = text.Split(
                new String[] { "\n", "\r", "\t", "。", "？", "！" },
                StringSplitOptions.RemoveEmptyEntries
                );
			
            if (sentences != null)
            {
                foreach (String s in sentences)
                {
                	handleMatches(Expr.Match(s));
                }
            }
		}
		
	}
}
