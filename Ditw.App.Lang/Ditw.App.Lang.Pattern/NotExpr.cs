/*
 * Created by SharpDevelop.
 * User: jiajiwu
 * Date: 12/29/2012
 * Time: 10:58 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Ditw.App.Lang.Pattern
{
	/// <summary>
	/// Description of NotExpr.
	/// </summary>
	public class NotExpr : ExprBase
	{
		private ExprBase _expr;

		public NotExpr(ExprBase expr)
		{			
			_expr = expr;
		}
		
		public override String Text
		{
			get
			{
				return String.Format("[^({0})]*", _expr.Text);
			}
		}
		
		public override bool IsMatch(string text)
		{
			return !_expr.IsMatch(text);
		}
		
		public override IEnumerable<MatchInfo> Match(string text)
		{
			throw new NotImplementedException("NotExpr");
		}
	}
}
