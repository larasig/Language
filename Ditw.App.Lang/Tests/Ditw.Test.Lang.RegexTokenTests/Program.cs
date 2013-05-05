using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ditw.App.Lang.Tokenizer;
using Ditw.Util.Xml;
using System.Diagnostics;

namespace Ditw.Test.Lang.RegexTokenTests
{
    class Program
    {
        static String _constDir = @"C:\GIT\Language\Ditw.App.Lang\RegexDef\";
        static String[] _constFiles = new String[] {
            _constDir + "malware_ind.cnst",
            _constDir + "cyberattack_ind.cnst",
            _constDir + "nation.cnst",
            _constDir + "phrases.cnst",
            _constDir + "hacker_ind.cnst",
        };

        static void RunTest_Match(RegexToken regexToken, String txt)
        {
            BasicTextSegment bts = new BasicTextSegment(
                txt, 0, txt.Length);
            regexToken.FindChildMatch(bts);
            //Trace.WriteLine(t.Text);
            //Trace.WriteLine("-----------------------------------");
            Trace.WriteLine(txt);
            Trace.WriteLine("-----------------------------------");
            bts.TraceChildSegments();
            Trace.WriteLine("-----------------------------------");
        }

        static void Main(string[] args)
        {
            RegexToken rt = RegexToken.FromXml(
                _constDir + @"testRegex.xml",
                _constFiles
                );

            RunTest_Match(rt,
                "First discovered in 2011, the Shylock banking Trojan affects virtually all versions of Windows from Windows 2000 onward, and has turned into one of the most advanced forms of financial fraud malware around."
                );
            RunTest_Match(rt,
                "It employed malware that can wipe the contents of a computer's hard disk as well as drives connected to the infected computer."
                );
        }
    }
}
