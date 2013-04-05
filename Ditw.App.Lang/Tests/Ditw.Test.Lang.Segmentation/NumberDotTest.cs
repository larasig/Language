using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ditw.Util.Xml;
using Ditw.App.Lang.Tokenizer;

namespace Ditw.Test.Lang.Segmentation
{
    [TestClass]
    public class NumberDotTest
    {
        const String _pathToRFTestCaseFiles = @"C:\git\LANG\Language\Ditw.App.Lang\TestText";
        private static SegmentationTestCases LoadTestCases(String fileName)
        {
            return XmlUtil.DeserializeFromFile<SegmentationTestCases>(
                _pathToRFTestCaseFiles + @"\" + fileName);
        }

        private static Boolean TestSegment(TextSegment tseg, Segment seg)
        {
            if (seg.FullText != tseg.FullText ||
                seg.OpenMark != tseg.OpenMark.ToString() ||
                seg.CloseMark != tseg.CloseMark.ToString() ||
                seg.StartIndex != tseg.StartIndex)
            {
                return false;
            }

            if (tseg.ChildSegments == null && (seg.Children == null || seg.Children.Count == 0))
            {
                return true;
            }

            if (tseg.ChildSegments != null &&
                seg.Children != null && 
                tseg.ChildSegments.Count == seg.Children.Count)
            {
                for (Int32 i = 0; i < tseg.ChildSegments.Count; i ++)
                {
                    if (!TestSegment(tseg.ChildSegments[i], seg.Children[i]))
                    {
                        return false;
                    }
                }
                return true;
            }

            return false;
        }

        private static Boolean RunOneTestCase(SegmentationTestCase testCase)
        {
            var segList = PunctuationMarkHelper.Segmentation(testCase.TestText);
            if (segList.Count != testCase.Segments.Count)
                return false;
            for (Int32 i = 0; i < segList.Count; i ++)
            {
                if (!TestSegment(segList[i], testCase.Segments[i]))
                    return false;
            }
            return true;
        }

        [TestMethod]
        public void RFT_Acquire_all()
        {
            SegmentationTestCases testcases = LoadTestCases(@"segmentation_zhs_acquire_all.xml");

            testcases.TestCases.ForEach(
                tc =>
                {
                    if (!RunOneTestCase(tc))
                    {
                        Assert.Fail(tc.TestText);
                    }
                }
                );
        }

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
