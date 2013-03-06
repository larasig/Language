/*
 * Created by SharpDevelop.
 * User: jiajiwu
 * Date: 12/31/2012
 * Time: 5:55 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Text;

namespace Ditw.App.Lang.Pattern
{
	/// <summary>
	/// Description of CommonFuncs.
	/// </summary>
	public static class CommonFuncs
	{
		public static String RegexStringFromWords(params String[] words)
		{
			if (words.Length < 1)
			{
				throw new ArgumentException("words");
			}
			
			if (words.Length == 1)
			{
				return words[0];
			}
			
			StringBuilder builder = new StringBuilder();
			builder.Append(words[0]);
			for (Int32 i = 1; i < words.Length; i++)
			{
				builder.Append('|');
				builder.Append(words[i]);
			}
			return builder.ToString();
		}
	}
}
