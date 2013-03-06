/*
 * Created by SharpDevelop.
 * User: jiajiwu
 * Date: 1/5/2013
 * Time: 7:58 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ditw.App.MediaSource.DbUtil;

namespace Ditw.App.Lang.Pattern
{
	/// <summary>
	/// Description of PatternMatchCollection.
	/// </summary>
	public class PatternMatchCollection
	{
		private Dictionary<String, List<MatchInfo>> _matches =
			new Dictionary<String, List<MatchInfo>>();
		
		public IEnumerable<String> MatchTexts
		{
			get
			{
				return _matches.Keys;
			}
		}

		private Dictionary<String, List<MatchInfo>> _orderedMatches;
		public IEnumerable<String> MatchTextsInOrder
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
		
		
		public void Add(String text, MatchInfo match)
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
		
		public void AddTextInBetween(IEnumerable<MatchInfo> matches)
		{
			foreach(var m in matches)
			{
				Add(m.TextsInBetween[0], m);
			}
		}
		
		public List<MatchInfo> GetMatchList(String text)
		{
			return _matches[text];
		}
		
		public Int32 GetMatchCount(String text)
		{
			return GetMatchList(text).Count;
		}
		
		private static String TruncateIfNeeded(String t, Int32 maxLength)
		{
			return t.Length < maxLength ? t : t.Substring(0, maxLength);
		}
		
		#region data layer method
		public IDataStore GetMatchListData(String text, Int32 filterId)
		{
			var m = GetMatchList(text);
			IDataStore dataStore = new DataStoreBase();
			
			dataStore["filterId"] = filterId;
			dataStore["capturedText"] = TruncateIfNeeded(text, 128);
			dataStore["status"] = -1; // Undefined
			dataStore["count"] = m.Count;
			#region context text examples
			StringBuilder builder = new StringBuilder();
			m.ForEach(
				mi => builder.AppendFormat("[{0}]|", mi.Text)
			);
			dataStore["contextText"] = TruncateIfNeeded(builder.ToString(), 256);
			#endregion
			
			return dataStore;
		}
		#endregion
	}
}
