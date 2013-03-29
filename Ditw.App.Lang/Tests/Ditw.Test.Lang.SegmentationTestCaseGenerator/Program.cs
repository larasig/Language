﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ditw.App.EvtX.Cfg.Test;
using Ditw.Util.Xml;
using Ditw.App.Lang.Tokenizer;
using Ditw.Test.Lang.Segmentation;

namespace Ditw.Test.Lang.SegmentationTestCaseGenerator
{
    class Program
    {
        const String _pathToRFTestCaseFiles = @"C:\git\LANG\Language\Ditw.App.Lang\TestText";
        static void RFTestCases(String fileName, Action<EvtXTest> TestCaseHandler)
        {
            String testCaseFile = _pathToRFTestCaseFiles + "\\" + fileName;

            EvtXTests testSuite = XmlUtil.DeserializeFromFile<EvtXTests>(testCaseFile);
            foreach (var t in testSuite.Tests)
            {
                TestCaseHandler(t);
            }
        }

        static SegmentationTestCases _testCases;
        static void Main(string[] args)
        {
            //BuiltInExpressions.LIST.Match("以色列、叙利亚和其他中东国家也受到了一定影响");
			//TestAndExpr();
			//TestRegexExpr_PatternWNW();
			//TestRegExprWithWordSet();
			//CaptureText();
            //DictionaryTest(new String[] {
            //    "日军在南京疯狂大屠杀一事，战后中日两国均已出版了大量揭露这方面真相的书籍",
            //    "投诉举报中心还将跟踪了解国际食品药品安全重大事件，学习境外食品药品投诉举报工作先进经验，并与有关国家和地区的相关机构或组织建立密切合作关系，拓宽我国食品药品投诉举报工作的合作领域",
            //    }
            //);

            _testCases = new SegmentationTestCases();
            _testCases.TestCases = new List<SegmentationTestCase>();
            RFTestCases(@"zhs_acquire_all.xml", RFTestCase_Segmentation); //RFTestCase_ZHS_PolRel);

            XmlUtil.Serialize(_testCases, _pathToRFTestCaseFiles + @"\segmentation_test.xml");
			
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
        }

        static void RFTestCase_Segmentation(EvtXTest testCase)
        {
            //PunctuationMarkHelper.TraceSegmentation(testCase.Sentence);
            var segList = PunctuationMarkHelper.Segmentation(testCase.Sentence);
            SegmentationTestCase ts = new SegmentationTestCase()
            {
                TestText = testCase.Sentence,
                Segments = new List<Segment>(segList.Count)
            };
            foreach (var tseg in segList)
            {
                ts.Segments.Add(Segment.FromTextSegment(tseg));
            }

            _testCases.TestCases.Add(ts);
        }

    }
}