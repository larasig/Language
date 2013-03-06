using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Ditw.App.Lang.Pattern;
using Ditw.App.MediaSource.DbUtil;

namespace Ditw.App.Lang.ConsoleTest
{
    class Program
    {
            //static Regex regex = ExprBuilder.GetRegex("从", "回到");
            //static Regex regex = ExprBuilder.GetRegex("除", "外", "还");
            //static Regex regex = ExprBuilder.GetRegex("在", "时");
            #if false
            static Regex regex1 = ExprBuilder.GetRegex("部", "城市");
            static Regex regex2 = ExprBuilder.GetRegexNoClauseSeparators(
            	new Char[] { ',', ';', '，', '；' },
            	"部", "城市");
            static Regex regex1 = ExprBuilder.GetRegex("从", "出发");
            static Regex regex2 = ExprBuilder.GetRegexNoClauseSeparators(
            	new Char[] { ',', ';', '，', '；' },
            	"从", "出发");
            static Regex regex1 = ExprBuilder.GetRegex("从", "开始");
            static Regex regex2 = ExprBuilder.GetRegexNoClauseSeparators(
            	new Char[] { ',', ';', '，', '；' },
            	"从", "开始");
            #endif
            #if false
            static Regex regex1 = ExprBuilder.GetRegex("据", "报道");
            static Regex regex2 = ExprBuilder.GetRegexNoClauseSeparators(
            	new String[] { ",", ";", "，", "；" },
            	"据", "报道");
//            static Regex regex3 = ExprBuilder.GetRegex(
//            	new String[] { ",", ";", "，", "；" },
//            	"据", "报道");
            static Regex regex3 = new Regex("(据)[^,;，；]*(报道)");
            #endif
            //Regex regex = ExprBuilder.GetRegex("抵", "后");
            //Regex regex = ExprBuilder.GetRegex("不");
            //Regex regex = ExprBuilder.GetRegex("不仅");
            //Regex regex = ExprBuilder.GetRegex("据", "报道");
            static Regex regex1 = ExprBuilder.GetRegex("前", "议员");
            static Regex regex2 = ExprBuilder.GetRegexNoClauseSeparators(
            	new String[] { ",", ";", "，", "；", "民前" },
            	"前", "议员");
//            static Regex regex3 = ExprBuilder.GetRegexNoClauseSeparators(
//            	new String[] { ",", ";", "，", "；", "民前" },
//            	"前", "议员");
            static Regex regex3 = new Regex(
				@"(?<g1>前).*(?<g2>议员)"//"(前)(?>!.*民主).*(议员)"
			);
        static void Main(string[] args)
        {
            //const String sep = "{649875FE-A398-49B4-BA74-01F3A5FFF6F7}";


            Main1();
            
            Console.ReadLine();
        }
        static void Main1()
        {
            //const String sep = "{649875FE-A398-49B4-BA74-01F3A5FFF6F7}";


            MySQLAgentNewsFeeds.ReadNews(
            	new DateTime(2012, 2, 29),
            	NewsNewHandler
            );
            
        }
        static void Main2()
        {
            //const String sep = "{649875FE-A398-49B4-BA74-01F3A5FFF6F7}";


            NewsHandler(
            	//"据《新闻晨报<!--keyword--> (微博)<!--/keyword-->》报道"
            	"前国家民党议员"
            );
            
        }
        
        static AndExpr _andExpr = new AndExpr(null,
//                         LitExpr.FromStrings(
//                         	new String[] { "国家", "民党" }
//                         ),
                         LitExpr.FromString("担任"),
                         LitExpr.FromString("职务")
                        );

        static AndExpr _dirExpr = new AndExpr(null,
                         LitExpr.FromStrings(
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
                            }
                         ),
                         LitExpr.FromString("部城市")
                        );

        static AndExpr _titleExpr = new AndExpr(null,
                         LitExpr.FromStrings(
                         	new String[] { 
                            	"董事长",
                            	"主任",
                            	"经理",
                            	"被告人",
                            	"市长",
                            	"助理",
                            	"市长助理",
                            	"总裁",
                            }
                         ),
                         LitExpr.FromString("，")
                        );
        
        static void NewsNewHandler(String content)
        {
        	Console.WriteLine(_cnt++);
        	
            String[] sentences = content.Split(
                new String[] { "\n", "\r", "\t", "。", "？" },
                StringSplitOptions.RemoveEmptyEntries
                );
        	

            if (sentences != null)
            {
                foreach (String s in sentences)
                {
                    ShowMatch(TestExpr.NationNationRelExpr.Match(s));
                }
            }
        }

        
        static void ShowMatch(IEnumerable<MatchInfo> matches)
        {
        	if (matches == null)
        		return;
            foreach(MatchInfo m in matches)
            {
	            Trace.WriteLine("---------------------");
            	Trace.WriteLine(m.SrcText);
            	Trace.Write("  ");
            	Trace.WriteLine(m.Text);
            	foreach(MatchInfo mi in m.SubMatches)
            	{
	            	Trace.WriteLine("\t" + mi.Text);
	            	//Trace.WriteLine("\t" + mi.SrcText.Substring(mi.Index, 10));
            	}
            }
        }
        
        static void Main3()
        {
            //const String sep = "{649875FE-A398-49B4-BA74-01F3A5FFF6F7}";


            String t = @"图片来源：香港文汇报人民网2月29日电 据香港文汇报报道，因应近日传媒连番报道特首曾荫权涉及的“富豪款待”及“曾大屋”等事件，曾荫权致函全体公务员，强调在诚信问题上绝不会宽待自己，但明白到有关其外游及退休计划等连串报道，为他所领导并共事的公务员队伍带来不安，决定向公务员作出郑重交代，并直言这深刻的一课，让他学会日后处事必须更小心谨慎，继续在余下任期做好工作，尽心勉力服务香港，为全体市民谋取最佳福祉，并期望继续得到公务员队伍勤勉、专业的支持和协助";
//            LitExpr le = LitExpr.FromStrings(
//            	new String[] {
//	            	"国家",
//	            	"家民",
//	            	"党家"
//            	}
//            );
			var mlist = _andExpr.Match(t);

			ShowMatch(mlist);
        }
        
        static Int32 _cnt = 0;
        
        static void NewsHandler(String content)
        {
        	Console.WriteLine(_cnt++);
        	
            String[] sentences = content.Split(
                new String[] { "\n", "\r", "\t", "。", "？" },
                StringSplitOptions.RemoveEmptyEntries
                );

            if (sentences != null)
            {
                foreach (String s in sentences)
                {
                    Match m = regex1.Match(s);
                    if (m.Success)
                    {
                    	Trace.Write("----");
	                    Trace.WriteLine(s);
                        Trace.WriteLine("\t" + m.ToString());
                    }
                    m = regex2.Match(s);
                    if (m.Success)
                    {
                    	Trace.Write("====");
	                    Trace.WriteLine(s);
                        Trace.WriteLine("\t" + m.ToString());
                    }
                    m = regex3.Match(s);
                    if (m.Success)
                    {
                    	Trace.Write("####");
	                    Trace.WriteLine(s);
                        Trace.WriteLine("\t" + m.ToString());
                    }
                    #if false
                    #endif
                }
            }
        }
    }
}
