using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Ditw.App.Lang.Tokenizer;

namespace Ditw.Test.Lang.Segmentation
{
    [XmlRoot("segment")]
    public class Segment
    {
        private TextSegment _Segment = new TextSegment();

        [XmlElement("startindex")]
        public Int32 StartIndex
        {
            get { return _Segment.StartIndex; }
            set { _Segment.StartIndex = value; }
        }

        [XmlElement("fulltext")]
        public String FullText
        {
            get;
            set;
        }

        [XmlElement("openmark")]
        public String OpenMark
        {
            get;
            set;
        }

        [XmlElement("closemark")]
        public String CloseMark
        {
            get;
            set;
        }

        [XmlElement("child")]
        public List<Segment> Children
        {
            get;
            set;
        }

        public static Segment FromTextSegment(TextSegment textSeg)
        {
            Segment s = new Segment();
            s.OpenMark = textSeg.OpenMark.HasValue ? textSeg.OpenMark.Value.ToString() : String.Empty;
            s.StartIndex = textSeg.StartIndex;
            s.CloseMark = textSeg.CloseMark.HasValue ? textSeg.CloseMark.Value.ToString() : String.Empty;
            s.FullText = textSeg.FullText;
            if (textSeg.ChildSegments != null && textSeg.ChildSegments.Count > 0)
            {
                s.Children = new List<Segment>(textSeg.ChildSegments.Count);
                foreach (var c in textSeg.ChildSegments)
                {
                    s.Children.Add(FromTextSegment(c));
                }
            }
            return s;
        }
    }

    [XmlRoot("testcase")]
    public class SegmentationTestCase
    {
        [XmlElement("text")]
        public String TestText
        {
            get;
            set;
        }

        [XmlElement("segments")]
        public List<Segment> Segments
        {
            get;
            set;
        }
    }

    [XmlRoot("testcases")]
    public class SegmentationTestCases
    {
        [XmlElement("testcase")]
        public List<SegmentationTestCase> TestCases
        {
            get;
            set;
        }
    }
}
