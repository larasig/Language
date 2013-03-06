/*
 * Created by SharpDevelop.
 * User: jiajiwu
 * Date: 12/29/2012
 * Time: 8:51 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Ditw.App.Lang.Pattern
{
	/// <summary>
	/// Description of LitExpr.
	/// </summary>
	public class LitExpr : ExprBase
	{
		private String[] _literals;
		
		public LitExpr(String word)
		{
			_literals = new String[1];
			_literals[0] = word;
		}
		
		public LitExpr(String[] words)
		{
			if (words.Length <= 0)
			{
				throw new ArgumentException("Parameter 'words' should at least contain one string!");
			}
			_literals = words;
		}
		
		public override String Text
		{
			get
			{
				return CommonFuncs.RegexStringFromWords(_literals);
			}
		}
		
		public static LitExpr FromChar(Char c)
		{
			return new LitExpr(c.ToString());
		}
		
		public static LitExpr FromString(String word)
		{
			return new LitExpr(word);
		}
		
		public static LitExpr FromStrings(String[] words)
		{
			return new LitExpr(words);
		}
		
		
	}
}
