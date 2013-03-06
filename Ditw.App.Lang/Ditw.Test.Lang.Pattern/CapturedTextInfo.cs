/*
 * Created by SharpDevelop.
 * User: jiajiwu
 * Date: 1/4/2013
 * Time: 5:08 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using Ditw.App.Lang.Pattern;

namespace Ditw.Test.Lang.Pattern
{
	/// <summary>
	/// Description of CapturedTextInfo.
	/// </summary>
	public static class MatchCollection
	{
		private static Dictionary<String, List<MatchInfo>> _matches =
			new Dictionary<String, List<MatchInfo>>();
		
		public static IEnumerable<String> MatchTexts
		{
			get
			{
				return _matches.Keys;
			}
		}

		private static Dictionary<String, List<MatchInfo>> _orderedMatches;
		public static IEnumerable<String> MatchTextsInOrder
		{
			get
			{
				if (_orderedMatches == null)
				{
					_orderedMatches = _matches.OrderByDescending(m => m.Value.Count).ToDictionary(
						p => p.Key, p => p.Value);
				}
				return _orderedMatches.Keys;
			}
		}
		
		
		public static void Add(String text, MatchInfo match)
		{
			if (String.IsNullOrEmpty(text))
			{
				return;
			}
			List<MatchInfo> textMatches;
			if (!_matches.ContainsKey(text))
			{
				lock(_matches)
				{
					if (!_matches.ContainsKey(text))
					{
						_matches.Add(text, new List<MatchInfo>());
					}
				}
			}
			
			textMatches = _matches[text];
			if (textMatches.Where(
				m => m.SrcText.Equals(match.SrcText, StringComparison.Ordinal)).Count() > 0)
			{
				// same source text, do NOT add
				return;
			}
			
			lock (textMatches)
			{
				textMatches.Add(match);
			}
		}
		
		public static List<MatchInfo> GetMatchList(String text)
		{
			return _matches[text];
		}
		
		public static Int32 GetMatchCount(String text)
		{
			return GetMatchList(text).Count;
		}
		
	}
}
