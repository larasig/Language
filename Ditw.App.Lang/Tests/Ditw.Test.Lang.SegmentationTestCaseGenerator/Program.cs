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
            foreach (var c in MySQLAgentNewsFeeds.ReadStringsFromSource(srcId, "content"))
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

        const String Tokens = @"<regextokens id=""tokens"">
  <constants>
    <c id=""c_nations"" casesensitive=""0"" allowplural=""0"">
      <e>Abkhazia</e>
      <e>Afghanistan</e>
      <e>Albania</e>
      <e>Algeria</e>
      <e>Andorra</e>
      <e>Angola</e>
      <e>Antigua and Barbuda</e>
      <e>Argentina</e>
      <e>Armenia</e>
      <e>Australia</e>
      <e>Austria</e>
      <e>Azerbaijan</e>
      <e>Bahamas</e>
      <e>Bahrain</e>
      <e>Bangladesh</e>
      <e>Barbados</e>
      <e>Belarus</e>
      <e>Belgium</e>
      <e>Belize</e>
      <e>Benin</e>
      <e>Bhutan</e>
      <e>Bolivia</e>
      <e>Bosnia and Herzegovina</e>
      <e>Botswana</e>
      <e>Brazil</e>
      <e>Britain</e>
      <e>Brunei</e>
      <e>Bulgaria</e>
      <e>Burkina Faso</e>
      <e>Burma</e>
      <e>Burundi</e>
      <e>Cambodia</e>
      <e>Cameroon</e>
      <e>Canada</e>
      <e>Cape Verde</e>
      <e>Central African Republic</e>
      <e>Chad</e>
      <e>Chile</e>
      <e>China</e>
      <e>Colombia</e>
      <e>Comoros</e>
      <e>Congo</e>
      <e>Congo-Kinshasa</e>
      <e>Congo-Brazzaville</e>
      <e>Cook Islands</e>
      <e>Costa Rica</e>
      <e>Côte d'Ivoire</e>
      <e>Croatia</e>
      <e>Cuba</e>
      <e>Cyprus</e>
      <e>Czech Republic</e>
      <e>Denmark</e>
      <e>Djibouti</e>
      <e>Dominica</e>
      <e>Dominican Republic</e>
      <e>East Timor</e>
      <e>Ecuador</e>
      <e>Egypt</e>
      <e>El Salvador</e>
      <e>Equatorial Guinea</e>
      <e>Eritrea</e>
      <e>Estonia</e>
      <e>Ethiopia</e>
      <e>Fiji</e>
      <e>Finland</e>
      <e>France</e>
      <e>Gabon</e>
      <e>Gambia</e>
      <e>Georgia</e>
      <e>Germany</e>
      <e>Ghana</e>
      <e>Greece</e>
      <e>Grenada</e>
      <e>Guatemala</e>
      <e>Guinea</e>
      <e>Guinea-Bissau</e>
      <e>Guyana</e>
      <e>Haiti</e>
      <e>Honduras</e>
      <e>Hungary</e>
      <e>Iceland</e>
      <e>India</e>
      <e>Indonesia</e>
      <e>Iran</e>
      <e>Iraq</e>
      <e>Ireland</e>
      <e>Israel</e>
      <e>Italy</e>
      <e>Jamaica</e>
      <e>Japan</e>
      <e>Jordan</e>
      <e>Kazakhstan</e>
      <e>Kenya</e>
      <e>Kiribati</e>
      <e>North Korea</e>
      <e>South Korea</e>
      <e>Kosovo</e>
      <e>Kuwait</e>
      <e>Kyrgyzstan</e>
      <e>Laos</e>
      <e>Latvia</e>
      <e>Lebanon</e>
      <e>Lesotho</e>
      <e>Liberia</e>
      <e>Libya</e>
      <e>Liechtenstein</e>
      <e>Lithuania</e>
      <e>Luxembourg</e>
      <e>Macedonia</e>
      <e>Madagascar</e>
      <e>Malawi</e>
      <e>Malaysia</e>
      <e>Maldives</e>
      <e>Mali</e>
      <e>Malta</e>
      <e>Marshall Islands</e>
      <e>Mauritania</e>
      <e>Mauritius</e>
      <e>Mexico</e>
      <e>Micronesia</e>
      <e>Moldova</e>
      <e>Monaco</e>
      <e>Mongolia</e>
      <e>Montenegro</e>
      <e>Morocco</e>
      <e>Mozambique</e>
      <e>Myanmar</e>
      <e>Nagorno-Karabakh</e>
      <e>Namibia</e>
      <e>Nauru</e>
      <e>Nepal</e>
      <e>Netherlands</e>
      <e>New Zealand</e>
      <e>Nicaragua</e>
      <e>Niger</e>
      <e>Nigeria</e>
      <e>Norway</e>
      <e>Oman</e>
      <e>Pakistan</e>
      <e>Palau</e>
      <e>Palestine</e>
      <e>Panama</e>
      <e>Papua New Guinea</e>
      <e>Paraguay</e>
      <e>Peru</e>
      <e>Philippines</e>
      <e>Poland</e>
      <e>Portugal</e>
      <e>Qatar</e>
      <e>Romania</e>
      <e>Russia</e>
      <e>Rwanda</e>
      <e>Sahrawi Arab Democratic Republic</e>
      <e>Saint Kitts and Nevis</e>
      <e>Saint Lucia</e>
      <e>Saint Vincent and the Grenadines</e>
      <e>Samoa</e>
      <e>San Marino</e>
      <e>São Tomé and Príncipe</e>
      <e>Saudi Arabia</e>
      <e>Senegal</e>
      <e>Serbia</e>
      <e>Seychelles</e>
      <e>Sierra Leone</e>
      <e>Singapore</e>
      <e>Slovakia</e>
      <e>Slovenia</e>
      <e>Solomon Islands</e>
      <e>Somalia</e>
      <e>Somaliland</e>
      <e>South Africa</e>
      <e>South Ossetia</e>
      <e>South Sudan</e>
      <e>Spain</e>
      <e>Sri Lanka</e>
      <e>Sudan</e>
      <e>Suriname</e>
      <e>Swaziland</e>
      <e>Sweden</e>
      <e>Switzerland</e>
      <e>Syria</e>
      <e>Taiwan</e>
      <e>Tajikistan</e>
      <e>Tanzania</e>
      <e>Thailand</e>
      <e>Togo</e>
      <e>Tonga</e>
      <e>Transnistria</e>
      <e>Trinidad and Tobago</e>
      <e>Tunisia</e>
      <e>Turkey</e>
      <e>Turkmenistan</e>
      <e>Tuvalu</e>
      <e>Uganda</e>
      <e>Ukraine</e>
      <e>United Arab Emirates</e>
      <e>UAE</e>
      <e>United Kingdom</e>
      <e>UK</e>
      <e>United States</e>
      <e>US</e>
      <e>U.S.</e>
      <e>USA</e>
      <e>Uruguay</e>
      <e>Uzbekistan</e>
      <e>Vanuatu</e>
      <e>Vatican City</e>
      <e>Venezuela</e>
      <e>Vietnam</e>
      <e>Yemen</e>
      <e>Zambia</e>
      <e>Zimbabwe</e>

      <e>D.R.C.</e>
      <e>P.R.C.</e>

      <e>EU</e>
    </c>
    <c id=""c_cyberattack"" casesensitive=""0"" allowplural=""1"">
      <e>botnet attack</e>
      <e>botnet-driven attack</e>
      <e>cyber attack</e>
      <e>cyber-attack</e>
      <e>cyberattack</e>
      <e>cyber intrusion</e>
      <e>crypto attack</e>
      <e>trojan attack</e>
      <e>online attack</e>
      <e>malware attack</e>

      <e>DoS attack</e>
      <e>DoS Attack</e>
      <e>DDoS assault</e>
      <e>DDoS Assault</e>
      <e>DDoS campaign</e>
      <e>DDoS Campaign</e>
      <e>DDoS attack</e>
      <e>DDoS Attack</e>
      <e>SQL injection attack</e>
      <e>denial of service attack</e>
      <e>denial-of-service attack</e>
      <e>distributed denial-of-service attack</e>
      <e>distributed denial of service attack</e>
      <e>phishing attack</e>
      <e>smurf attack</e>
      <e>spam attack</e>
      <e>virus attack</e>
      <e>computer virus attack</e>
      <e>viral post attack</e>
      <e>worm attack</e>
      <e>zero-day attack</e>
      <e>zero day attack</e>
      <e>zero-hour attack</e>
      <e>zero hour attack</e>
      <e>day zero attack</e>
      <e>zero-day exploit</e>
      <e>zero day exploit</e>
      <e>zero-hour exploit</e>
      <e>zero hour exploit</e>
      <e>day zero exploit</e>

      <e>hacker attack</e>

      <e>remoteadmin attack</e>
      <e>malware-based attack</e>
      <e>malware/virus attack</e>
      <e>rogue attack</e>
      <e>remote admin attack</e>
      <e>dos attack</e>
      <e>spam-storm attack</e>
      <e>spam-driven attack</e>
      <e>malware-driven attack</e>
      <e>malware-like attack</e>
      <e>malware-helped attack</e>
      <e>malware-related attack</e>
    </c>
    <c id=""c_called"" casesensitive=""0"" allowplural=""0"">
      <e>known as</e>
      <e>called</e>
      <e>dubbed</e>
      <e>calling itself</e>
    </c>
    <c id=""c_hacker"" casesensitive=""0"" allowplural=""1"">
      <e>hacker</e>
      <e>cybercrook</e>
      <e>cyber-crook</e>
      <e>cybercriminal</e>
      <e>cyber-criminal</e>
      <e>cyberextortionist</e>
      <e>cybergroup</e>
      <e>hacktivist</e>
      <e>hactivist</e>

      <e>cyber crook</e>
      <e>cyber criminal</e>
      <e>cyber fiends</e>
      <e>cyber snoopers</e>
      <e>hacker group</e>
      <e>hackers group</e>
      <e>hackers' group</e>
      <e>hacker collective</e>
      <e>hacking group</e>
      <e>hacking collective</e>
      <e>hacktivist group</e>
      <e>hacktivism group</e>
      <e>hactivist group</e>
      <e>hacking activist group</e>
      <e>hacker activist group</e>
      <e>computer crook</e>
    </c>
  </constants>
  <regexgroup name=""keywords"">
    <regex id=""attack_ind""><![CDATA[{{c_cyberattack}}]]></regex>
    <regex id=""hacker_ind""><![CDATA[{{c_hacker}}]]></regex>
    <regex id=""name1""><![CDATA[[A-Z]\w*(\s+[A-Z]\w*)*]]></regex>
    <regex id=""name2""><![CDATA[""[^""]*""]]></regex>
    <regex id=""name3""><![CDATA[“[^“]*”]]></regex>
    <regex id=""name"" isInternal=""0""><![CDATA[{{hacker_ind}}|{{name2}}|{{name3}}]]></regex>
    <regex id=""called_name"" isInternal=""0""><![CDATA[{{c_called}}(\s+the)?\s+({{name}}|{{name1}})]]></regex>
  </regexgroup>
</regextokens>";

        static SegmentationTestCases _testCases;
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
                RunTest(s.Text, Tokens);
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
