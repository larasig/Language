using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ditw.App.EvtX.Cfg.Test;
using Ditw.Util.Xml;
using Ditw.App.Lang.Tokenizer;
using Ditw.Test.Lang.Segmentation;
using System.Diagnostics;
using Ditw.App.MediaSource.DbUtil;
using System.Text.RegularExpressions;

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

        static void Test1()
        {
#if false
            List<string> l1 = new List<string>()
            {
                "hacker activist",
                "hacker",
                "hacker groups",
                "hacker group",
            };
            foreach(string l in StringConstant.ReorderWordList(l1))
            {
                Console.WriteLine(l);
            }
            Console.ReadLine();
#endif

            Regex regex = new Regex("(hacker)|(hacker group)");
            String text = "hacker group LulzSec";
            //regex.Options = RegexOptions.
#if false
            var matches = regex.Matches(text);
            for (Int32 i = 0; i < matches.Count; i ++)
            {
                var m = matches[i];
                Console.WriteLine("Match: " + text.Substring(m.Index, m.Length));
            }
#else
            var m = regex.Match(text);
            while (m.Success)
            {
                Console.WriteLine("Match: " + text.Substring(m.Index, m.Length));
                m = m.NextMatch();
            }
#endif
            Console.ReadLine();
        }

        static void DbTestCases(Int32 srcId, Action<String> TestCaseHandler)
        {
            foreach (var c in MySQLAgentNewsFeeds.ReadStringsFromSource("news_eng", srcId, "content"))
            {
                TestCaseHandler(c);
            }
        }

        const String regexMalware = @"<regextokens id=""malware"">
  <regexgroup name=""Digits-G"">
    <regex id=""integer""><![CDATA[\d+(,\d{3})*]]></regex>
    <regex id=""decimal""><![CDATA[\d*\.\d+]]></regex>
    <regex id=""number""><![CDATA[{{decimal}}|{{integer}}]]></regex>
    <regex id=""numberX""><![CDATA[{{number}}[%]]]></regex>
  </regexgroup>
  <regexgroup name=""Software"">
    <regex id=""OS-Software""><![CDATA[(NetWare|Windows Phone|Windows|Linux|iOS|BlackBerry|Symbian|Mac OS X|Mac OSX|Mac OS|OS X|Mac|Unix|UNIX|Android)]]></regex>
    <regex id=""App-Software""><![CDATA[(IE|Java)]]></regex>
    <regex id=""Version""><![CDATA[\d+(\.\d+)*]]></regex>
    <regex id=""Version-List""><![CDATA[{{Version}}(,\s*{{Version}})*]]></regex>
    <regex id=""Software""><![CDATA[{{OS-Software}}|{{App-Software}}]]></regex>
    <regex id=""Software-V""><![CDATA[({{OS-Software}}|{{App-Software}})\s+{{Version-List}}]]></regex>
    <regex id=""Software-List""><![CDATA[{{Software}}(\s*,\s*{{Software}})*\s*(and|,)\s*{{Software}}]]></regex>
    <regex id=""Software-List-Malware""><![CDATA[{{Software-List}}\s*malware]]></regex>
  </regexgroup>
</regextokens>";

        const String malwareExtract_old = @"<regextokens id=""malwareEx"">
  <regexgroup name=""keywords"">
    <regex id=""name1""><![CDATA[[A-Z]\w*(\s+[A-Z]\w*)*]]></regex>
    <regex id=""name2""><![CDATA[""[^""]*""]]></regex>
    <regex id=""name3""><![CDATA['[^']*']]></regex>
    <regex id=""name""><![CDATA[{{name1}}|{{name2}}|{{name3}}]]></regex>
    <regex id=""called""><![CDATA[(known as|called|named|codenamed|dubbed)]]></regex>
    <regex id=""keywords""><![CDATA[(hacker group|malware|group|hacker)]]></regex>
    <regex id=""named"" isInternal=""0""><![CDATA[{{called}}(\s+\w+){0,1}\s+{{name}}]]></regex>
    <regex id=""pattern"" isInternal=""0""><![CDATA[{{keywords}}.*{{named}}]]></regex>
    <regex id=""botnet"" isInternal=""0""><![CDATA[{{name}}\s+botnet]]></regex>
  </regexgroup>
</regextokens>";

        const String malwareExtract = @"<regextokens id=""malwareEx"">
  <regexgroup name=""keywords"">
    <regex id=""name1""><![CDATA[[A-Z]\w*(\s+[A-Z]\w*)*]]></regex>
    <regex id=""name2""><![CDATA[""[^""]*""]]></regex>
    <regex id=""name3""><![CDATA[“[^“]*”]]></regex>
    <regex id=""name"" isInternal=""0""><![CDATA[{{name1}}|{{name2}}|{{name3}}]]></regex>
    <regex id=""called""><![CDATA[(known as|called|named|codenamed|dubbed)]]></regex>
    <regex id=""keywords"" isInternal=""0""><![CDATA[(hacker group|malware|group|hacker)]]></regex>
    <regex id=""named"" isInternal=""0""><![CDATA[{{called}}(\s+\w+){0,1}\s+{{name}}]]></regex>
    <regex id=""pattern"" isInternal=""0""><![CDATA[{{keywords}}.*{{named}}]]></regex>
  </regexgroup>
</regextokens>";

        const String regextest = @"<regextokens id=""n"">
  <regexgroup name=""Digits-G"">
    <regex id=""decimal""><![CDATA[\d*\.\d+]]></regex>
    <regex id=""integer""><![CDATA[\d+]]></regex>
    <regex id=""number""><![CDATA[{{decimal}}|{{integer}}]]></regex>
    <regex id=""numberX""><![CDATA[{{number}}[%]]]></regex>
  </regexgroup>
  <regexgroup name=""Digits-ZHS"">
    <regex id=""zhs-quantity""><![CDATA[(艘|吨)]]></regex>
    <regex id=""zhs-number-suffix""><![CDATA[(万|亿)]]></regex>
    <regex id=""zhs-currency""><![CDATA[(元|美元|日元|欧元|美分|港元)]]></regex>
    <regex id=""numberX-zhs""><![CDATA[{{number}}{{zhs-number-suffix}}]]></regex>
    <regex id=""numberXU-zhs""><![CDATA[{{number}}{{zhs-quantity}}]]></regex>
    <regex id=""numberXM-zhs""><![CDATA[({{numberX-zhs}}|{{number}}){{zhs-currency}}]]></regex>
  </regexgroup>
  <regexgroup name=""DateTime-ZHS"">
    <regex id=""date""><![CDATA[\d+日]]></regex>
    <regex id=""month""><![CDATA[\d+月]]></regex>
    <regex id=""year""><![CDATA[\d+年]]></regex>
    <regex><![CDATA[{{year}}{{month}}]]></regex>
    <regex><![CDATA[{{month}}{{date}}]]></regex>
    <regex><![CDATA[{{year}}{{month}}{{date}}]]></regex>
  </regexgroup>
</regextokens>";

        static String _constDir = @"C:\GIT\Language\Ditw.App.Lang\RegexDef\";
        static String[] _constFiles = new String[] {
            _constDir + "malware_ind.cnst",
            _constDir + "cyberattack_ind.cnst",
            _constDir + "nation.cnst",
            _constDir + "phrases.cnst",
            _constDir + "hacker_ind.cnst",
        };

        static SegmentationTestCases _testCases;
        static RegexToken _regexToken;
        static void Main(string[] args)
        {

            //Test1();
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

            //RFTestCases(@"zhs_acquire_all.xml", RFTestCase_Test1); //RFTestCase_Segmentation); //RFTestCase_ZHS_PolRel);
            //RFTestCases(@"malware_threat_product.xml", RFTestCase_Malware1);
            //RFTestCases(@"malware_threat_sentences.xml", RFTestCase_Malware2);//RFTestCase_Malware1);
            //RFTestCases(@"malware_threat_product.xml", RFTestCase_Malware2);//RFTestCase_Malware1);

            _regexToken = RegexToken.FromXml(
                _constDir + @"testRegex.xml",
                _constFiles
                );
            DbTestCases(200100,
                //s => Trace.Write("-----------------------------------\n" + s)
                s =>
                {
                    Trace.Write("-----------------------------------\n" + s);
                    Trace.WriteLine("-----------------------------------\n");
                    //RunTest(s, malwareExtract);
                    Test_TextPreprocessor(s);
                }
                );

            //XmlUtil.Serialize(_testCases, _pathToRFTestCaseFiles + @"\segmentation_test.xml");
			
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
        }

        static void Test_TextPreprocessor(String text)
        {
            foreach (var s in TextPreProcessor.GetSentences(text))
            {
                //Trace.WriteLine(s.Text);
                //RunTest(s.Text, Tokens);
                RunTest_Match(s.Text);
            }
        }

        static void RFTestCase_Malware1(EvtXTest testCase)
        {
            RFTestCase_Regex(testCase, regexMalware);
        }

        static void RFTestCase_Malware2(EvtXTest testCase)
        {
            //testCase.Sentence = @"'Kneber' Botnet Attacks PCs Worldwide: FAQ";
            RFTestCase_Regex(testCase, malwareExtract);
        }

        static void RFTestCase_Test1(EvtXTest testCase)
        {
            RFTestCase_Regex(testCase, regextest);
        }

        static void RFTestCase_Regex(EvtXTest testCase, String regex)
        {
#if true
            RunTest(testCase.Sentence, regex);
#else
            String tmp = testCase.Sentence; //"而且，花旗银行还开出7.7亿美元收购仁川炼油公司的价格，比中化集团的5.6亿美元高出2.2亿美元，超出了中化集团的承受能力，最终导致了并购失败。";
            RegexToken rt = new RegexToken();
            rt.Xml = XmlUtil.DeserializeString<RegexTokenXml>(regex);
            BasicTextSegment bts = new BasicTextSegment(
                tmp, 0, tmp.Length);
            var t = rt.MatchText(bts);
            Trace.WriteLine(t.Text);
            Trace.WriteLine("-----------------------------------");
            t.TraceSegment();
            Trace.WriteLine(String.Empty);
            //Console.ReadLine();
#endif
        }

        static void RunTest(String txt, String regex)
        {
            RegexToken rt = new RegexToken();
            rt.Xml = XmlUtil.DeserializeString<RegexTokenXml>(regex);
            BasicTextSegment bts = new BasicTextSegment(
                txt, 0, txt.Length);
            var t = rt.MatchText(bts);
            //Trace.WriteLine(t.Text);
            //Trace.WriteLine("-----------------------------------");
            t.TraceSegment();
            Trace.WriteLine(String.Empty);
        }

        static void RunTest_Match(String txt)
        {
            BasicTextSegment bts = new BasicTextSegment(
                txt, 0, txt.Length);
            _regexToken.FindChildMatch(bts);
            //Trace.WriteLine(t.Text);
            //Trace.WriteLine("-----------------------------------");
            bts.TraceChildSegments();
            Trace.WriteLine(String.Empty);
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
