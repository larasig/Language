/*
 * Created by SharpDevelop.
 * User: jiajiwu
 * Date: 12/29/2012
 * Time: 9:10 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using System.Text;

namespace Ditw.App.Lang.Pattern
{
	/// <summary>
	/// Description of OrExpr.
	/// </summary>
	public class OrExpr : ExprBase
	{
		private ExprBase[] _expressions;

		public OrExpr(params ExprBase[] exprs)
		{
			if (exprs.Length <= 1)
			{
				throw new ArgumentException("Parameter 'exprs' should at least contain 2 expressions!");
			}
			
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
						"|({0})", _expressions[i].Text);
				}
				
				Trace.WriteLine(builder.ToString());
				return builder.ToString();
			}
		}
	}
}
