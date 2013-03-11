/*
 * Created by SharpDevelop.
 * User: jiaji
 * Date: 12/17/2012
 * Time: 2:33 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using Ditw.Util.Xml;

namespace Ditw.App.Lang.StaticData
{
	[XmlRoot("WordSet")]
	/// <summary>
	/// Description of MyClass.
	/// </summary>
	public class WordSet
	{
        [XmlAttribute("Name")]
        public String Name
        {
            get;
            set;
        }

		[XmlElement("w")]
		public List<String> Words
		{
			get;
			set;
		}
		
		[XmlIgnore()]
		public Int32 Count
		{
			get { return Words.Count; }
		}
		
		public static WordSet FromFile(String path)
		{
			return XmlUtil.DeserializeFromFile<WordSet>(path);
		}
		
		public static WordSet FromString(String content)
		{
			return XmlUtil.DeserializeString<WordSet>(content);
		}
	}
}