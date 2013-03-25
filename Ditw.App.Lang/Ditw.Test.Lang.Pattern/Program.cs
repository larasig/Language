/*
 * Created by SharpDevelop.
 * User: jiajiwu
 * Date: 12/30/2012
 * Time: 9:38 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

using Ditw.App.Lang.Pattern;
using Ditw.App.Lang.StaticData;
using Ditw.App.MediaSource.DbUtil;
using Ditw.App.Util.Algorithm;
using Ditw.Util.Xml;
using Ditw.App.EvtX.Cfg.Test;
using Ditw.App.Lang.Tokenizer;

namespace Ditw.Test.Lang.Pattern
{
	class Program
	{				
		static PatternMatchCollection _matches = new PatternMatchCollection();

		static String _delimiter = "[、，；：的兼和任与“”]";
		static ExprBase _expr = new AndExpr(
			null,
			new RegexExpr(_delimiter),
			new NotExpr(new RegexExpr(_delimiter)),
			new RegexExpr("(新|全球|前|副|执行|行政|资深|原|高级)*(总裁|董事长|CEO|首席执行官|CFO)")
		);
		static ExprPattern _ep = new ExprPattern(ExprPatternID.CEO_1, _expr);

		private static void WriteMatches()
		{
			
		}
		
		private static void CaptureText()
		{
			MySQLAgentNewsFeeds.ReadNews(
            	//new DateTime(2012, 2, 29),
            	//new DateTime(2010, 8, 31),
            	null,
            	NewsNewHandler
            );
			
			#if ADD_CAPTURED_TEXT_DB
			List<DbCommand> cmds = new List<DbCommand>(_matches.MatchTexts.Count());
			#endif
			foreach (String t in _matches.MatchTextsInOrder)
            {
            	Trace.WriteLine("---------------------");
            	var ml = _matches.GetMatchList(t);
            	Trace.Write(ml.Count);
        		Trace.WriteLine("\t" + t);
			#if ADD_CAPTURED_TEXT_DB
        		cmds.Add(
        			MySQLAgentNewsFeeds.CreateInsertCapturedTextCommand(
        				_matches.GetMatchListData(t, _ep.Id)
        			)
        		);
			#endif
            }
			#if ADD_CAPTURED_TEXT_DB
			MySQLAgentNewsFeeds.InsertCapturedText(cmds.ToArray());
			#endif
		}
		
		private static void FilterCapturedText()
		{
			MySQLAgentNewsFeeds.ReadCapturedText(
            	//new DateTime(2012, 2, 29),
            	//new DateTime(2010, 8, 31),
            	null,
            	FilteredTextRowHandler
            );
		}
        static void FilteredTextRowHandler(IDataStore ds)
        {
        	Trace.WriteLine("-----------------------------");
        	Trace.WriteLine(String.Format("{0}\t{1}\t{2}",
        		ds["count"],
        		ds["capturedText"],
        		ds["contextText"])
        	);
        }

		public static void Main(string[] args)
		{
            ZhsTokenizer.PrepareDictionary(_dictPath);

            var tokenList = ZhsTokenizer.Instance.Tokenize(
                "他说，在全球范围停止演习和训练，意味着美国军队的参与度将降低，而与其他国家的关系将受损。"
                );
            tokenList.TraceTokens();
            Console.ReadLine();
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
            RFTestCases(@"zhs_polrel_all.xml", RFTestCase_ZHS_PolRel);

			
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}

        static void ShowMatch(IEnumerable<MatchInfo> matches)
        {
            if (matches == null)
                return;
            foreach (MatchInfo m in matches)
            {
                Trace.WriteLine("---------------------");
                Trace.WriteLine(m.SrcText);
                Trace.Write("  ");
                Trace.WriteLine(m.Text);
                if (m.SubMatches == null)
                    continue;
                foreach (MatchInfo mi in m.SubMatches)
                {
                    Trace.WriteLine("\t" + mi.Text);
                    //Trace.WriteLine("\t" + mi.SrcText.Substring(mi.Index, 10));
                }
            }
        }

        static void RFTestCase_ZHS_Aquire(EvtXTest testCase)
        {
            ShowMatch(BuiltInExpressions.BEING_ACQUIRED.Match(testCase.Sentence));
            ShowMatch(BuiltInExpressions.DUI_ACQUIRED.Match(testCase.Sentence));
        }

        static String _dictPath = @"C:\git\LANG\Language\Ditw.App.Lang\Ditw.App.Lang.Tokenizer\Dictionaries";
        static void RFTestCase_ZHS_PolRel(EvtXTest testCase)
        {
            //ZhsTokenizer.PrepareDictionary(_dictPath);

            TokenizerFilter tf = new TokenizerFilter(ZhsTokenizer.Instance);

            //ShowMatch(BuiltInExpressions.PAY_ATTENTION.Match(testCase.Sentence));
            //ShowMatch(
            //    MatchFilters.ExcludeClauseSeparator(
            //        BuiltInExpressions.RELATIONSHIP_WITH.Match(testCase.Sentence)
            //    ));
            ShowMatch(
                MatchInfoColl.FromEnumerables(
                    BuiltInExpressions.RELATIONSHIP_WITH.Match(
                        testCase.Sentence
                        
                        //"不管是中曾根康弘，还是小泉纯一郎，都在任时间较长，因此得以和美国构建了良好的关系。"
                        //"他说，在全球范围停止演习和训练，意味着美国军队的参与度将降低，而与其他国家的关系将受损。"
                        )
                ).ApplyFilter(tf)
                .Matches
                );
            //ShowMatch(BuiltInExpressions.ATTENTION_PAY.Match(testCase.Sentence));
        }


        static void RFTestCase_ENG_BizTrans(EvtXTest testCase)
        {
            //ShowMatch(BuiltInExpressions.PAY_ATTENTION.Match(testCase.Sentence));
            ShowMatch(BuiltInExpressions.PAY_ATTENTION_1.Match(testCase.Sentence));
            ShowMatch(BuiltInExpressions.ATTENTION_PAY.Match(testCase.Sentence));
        }

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
		
		internal static void Assert(Boolean e)
		{
			if (!e)
				throw new Exception("Assert fail!");
		}
		
		
		public static void TestRegexExpr_PatternWNW()
		{
			String t = @"图片来源：香港文汇报人民网2月29日电 据香港文汇报报道，因应近日传媒连番报道特首曾荫权涉及的“富豪款待”及“曾大屋”等事件，曾荫权致函全体公务员，强调在诚信问题上绝不会宽待自己，但明白到有关其外游及退休计划等连串报道，为他所领导并共事的公务员队伍带来不安，决定向公务员作出郑重交代，并直言这深刻的一课，让他学会日后处事必须更小心谨慎，继续在余下任期做好工作，尽心勉力服务香港，为全体市民谋取最佳福祉，并期望继续得到公务员队伍勤勉、专业的支持和协助";

			ExprBase expr;
			IEnumerable<MatchInfo> matches;

			#region 据 . 报道
			expr = new AndExpr(
				null,
				LitExpr.FromString("据"),
				LitExpr.FromString("报道")
			);
			matches = expr.Match(t);
			Assert(matches.Count() == 3);
			Assert(matches.First().Text.Equals(@"据香港文汇报报道", StringComparison.Ordinal));
			Assert(matches.Last().Text.Equals(@"据香港文汇报报道，因应近日传媒连番报道特首曾荫权涉及的“富豪款待”及“曾大屋”等事件，曾荫权致函全体公务员，强调在诚信问题上绝不会宽待自己，但明白到有关其外游及退休计划等连串报道", StringComparison.Ordinal));
			#endregion
			
			#region 据 !, 报道
			expr = CommonExprs.CreatePtnWNW(
				",;:，；：",
				new String[]
				{
					"据"
				},
				new String[]
				{
					"报道"
				}
			);
			matches = expr.Match(t);
			Assert(matches.Count() == 1);
			Assert(matches.First().Text.Equals("据香港文汇报报道", StringComparison.Ordinal));
			#endregion

			#region 据 !曾荫权 报道
			expr = CommonExprs.CreatePtnWNW(
				new String[]
				{
					"曾荫权"
				},
				new String[]
				{
					"据"
				},
				new String[]
				{
					"报道"
				}
			);
			matches = expr.Match(t);
			Assert(matches.Count() == 2);
			Assert(matches.First().Text.Equals("据香港文汇报报道", StringComparison.Ordinal));

			expr = CommonExprs.CreatePtnWNW(
				new String[]
				{
					"传媒"
				},
				new String[]
				{
					"据"
				},
				new String[]
				{
					"报道"
				}
			);
			matches = expr.Match(t);
			Assert(matches.Count() == 1);
			Assert(matches.First().Text.Equals("据香港文汇报报道", StringComparison.Ordinal));
			#endregion
			
			
		}
		
		internal static String CurrentPath
		{
			get
			{
				return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			}
		}
		
		public static void TestRegExprWithWordSet()
		{
			WordSet ws = WordSet.FromFile(CurrentPath + @"\..\..\Data_ZHS\nations_zhs.xml");
			ExprBase expr;
			IEnumerable<MatchInfo> matches;
			
			expr = CommonExprs.CreatePtnWNW(
				",;:，；：",
				ws.Words.ToArray(),
				new String[] { 
                	"东",
                	"南",
                	"西",
                	"北",
                	"中",
                	"东南",
                	"东北",
                	"西南",
                	"西北",
                },
				new String[] {
					"部城市"
				}
			);
			
			String t = "7月17日，在巴基斯坦东部城市拉合尔，一辆救护车抵达爆炸现场。";
			matches = expr.Match(t);
			Assert(matches.Count() == 1);
			//Assert(matches.First().Text.Equals("据香港文汇报报道", StringComparison.Ordinal));
		}
		
		public static void TestAndExpr()
		{
            MySQLAgentNewsFeeds.ReadNews(
            	new DateTime(2012, 2, 29),
            	NewsNewHandler
            );
		}
		
        static AndExpr _andExpr1 = new AndExpr(
			null,
			LitExpr.FromChar('除'),
			LitExpr.FromChar('外'),
			LitExpr.FromChar('还')
        );
		
		static void TestSentence(ExprBase expr, String text)
		{
            String[] sentences = text.Split(
                new String[] { "\n", "\r", "\t", "。", "？" },
                StringSplitOptions.RemoveEmptyEntries
                );
        	

            if (sentences != null)
            {
                foreach (String s in sentences)
                {
                	ShowMatch(expr.Match(s));
                }
            }
		}
		
		static void DictionaryTest(String[] testSentences)
		{
			String dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			String dictBinPath = dir +  @"\DictionaryZhs.bin";
			if (!File.Exists(dictBinPath))
			{
				DictionarySerializer.SerializeDict(
					dir + @"\Data_Zhs\DictionaryZhs.txt", dictBinPath);
				Console.WriteLine("Done with serialization!");
			}
			
			AhoCorasickAutomaton acAuto = DictionarySerializer.DeserializeDict(dictBinPath);

            //Int32 i = 0;
            foreach (String t in testSentences)
            {
                foreach (KeywordWithPositionInfo pos in acAuto.GetKeywordsPosition(t))
                {
                    Debug.WriteLine(pos);
                    //Assert.IsTrue(o[i++].Equals(s, StringComparison.Ordinal));
                }
            }

		}
		
        static void NewsNewHandler(String content)
        {
        	#if false
			WordSet ws = WordSet.FromFile(CurrentPath + @"\..\..\Data_ZHS\nations_zhs.xml");
			
			ExprBase expr;
			expr = CommonExprs.CreatePtnWNW(
				",;:，；：",
				ws.Words.ToArray(),
				new String[] { 
                	"东",
                	"南",
                	"西",
                	"北",
                	"中",
                	"东南",
                	"东北",
                	"西南",
                	"西北",
                },
				new String[] {
					"部城市"
				}
			);
			#endif
			
			//expr = new RegexExpr("([、，；的][^、，；的]*)(新|全球|前|副|执行)*总裁");
			//expr = new RegexExpr("((担任|出任|当选)[^、，；]*)(新|全球|前|副|执行)*总裁");
//			expr = CommonExprs.CreatePtnWNW(
//				",;:，；：",
//				ws.Words.ToArray(),
//				new String[] { 
//                	"地区",
//                }
//			);

			//String delimiter = "[、，；：的兼和任与“”（）()]";
			
			//_ep = new ExprPattern(ExprPatternID.CEO_1, _expr);
			_ep.Apply(content, _matches.AddTextInBetween);
			
        	//TestSentence(expr, content);
        	//Run(expr, content, ShowMatchAndInbetween);
        }

        static void Run(ExprBase expr, String text, Action<IEnumerable<MatchInfo>> handleMatch)
		{
            String[] sentences = text.Split(
                new String[] { "\n", "\r", "\t", "。", "？", "！" },
                StringSplitOptions.RemoveEmptyEntries
                );
        	

            if (sentences != null)
            {
                foreach (String s in sentences)
                {
                	handleMatch(expr.Match(s));
                }
            }
		}
        
        static AhoCorasickAutomaton _acAuto;
        static void PrepareDictionary()
        {
        	if (_acAuto != null)
        		return;
			String dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			String dictBinPath = dir +  @"\DictionaryZhs.bin";
			if (!File.Exists(dictBinPath))
			{
				DictionarySerializer.SerializeDict(
					dir + @"\Data_Zhs\DictionaryZhs.txt", dictBinPath);
				Console.WriteLine("Done with serialization!");
			}
			
			_acAuto = DictionarySerializer.DeserializeDict(dictBinPath);

        }

        static void ShowMatchAndInbetween(IEnumerable<MatchInfo> matches)
        {
        	if (matches == null)
        		return;
        	
        	//PrepareDictionary();
        	
            foreach(MatchInfo m in matches)
            {
//	            Trace.WriteLine("---------------------");
//            	Trace.WriteLine(m.SrcText);
//            	Trace.Write("  ");
//            	Trace.WriteLine(m.DebugText);
            	if (m.SubMatches != null)
            	{
//	            	foreach(MatchInfo mi in m.SubMatches)
//	            	{
//		            	Trace.WriteLine("\t\t" + mi.DebugText);
//	            	}
	            	
//	            	Trace.WriteLine("    -----------------");
	            	foreach(String s in m.TextsInBetween)
	            	{
//	            		Trace.WriteLine("\t" + s);
//	            		Trace.Write("\t");
//		            	foreach (KeywordWithPositionInfo pos in _acAuto.GetKeywordsPosition(s))
//		                {
//		                    Debug.Write(pos);
//		                }
//	            		Trace.WriteLine(String.Empty);
	            		MatchCollection.Add(s, m);
	            	}

//	            	Trace.WriteLine("    -----------------");
            	}
            }
            
        }

	}
}