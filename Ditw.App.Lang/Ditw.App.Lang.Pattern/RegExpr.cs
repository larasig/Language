/*
 * Created by SharpDevelop.
 * User: jiajiwu
 * Date: 12/31/2012
 * Time: 5:14 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Ditw.App.Lang.Pattern
{
	/// <summary>
	/// Description of RegExpr.
	/// </summary>
	public class RegexExpr : ExprBase
	{
		//private Regex _regex;
		
		public RegexExpr(String regex)
		{
			_regexText = regex;
		}
		
		private String _regexText;
		public override String Text
		{
			get
			{
				return _regexText;
			}
		}
	}
}
