using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ditw.App.Lang.Tokenizer;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ditw.Test.Lang.RegexTokenUnitTest
{
    static class Loader
    {
        static String _constDir = @"C:\GIT\Language\Ditw.App.Lang\RegexDef\";
        static String[] _constFiles = new String[] {
            _constDir + "malware_ind.cnst",
            _constDir + "cyberattack_ind.cnst",
            _constDir + "nation.cnst",
            _constDir + "phrases.cnst",
            _constDir + "hacker_ind.cnst",
        };

        internal static RegexToken GetToken()
        {
            return RegexToken.FromXml(
                        _constDir + @"testRegex.xml",
                        _constFiles
                        );
        }

        internal static RegexToken GetToken(String xmlFile, String[] constFiles, String[] regexGroupFiles)
        {
            if (constFiles == null)
            {
                constFiles = new String[] { };
            }
            if (regexGroupFiles == null)
            {
                regexGroupFiles = new String[] { };
            }
            return RegexToken.FromXml(
                        _constDir + xmlFile,
                        constFiles.Select(s => _constDir + s).ToArray(),
                        regexGroupFiles.Select(s => _constDir + s).ToArray()
                        );
        }

        internal static void RunTest_Match(RegexToken regexToken, String txt,
            params String[] expectedPhrases)
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

            var notEmptySeg = bts.ChildSegments.Where(s => !String.IsNullOrEmpty(s.Text)).ToList();
            Assert.IsTrue(expectedPhrases.Length == notEmptySeg.Count());

            Int32 idx = 0;
            foreach (var s in notEmptySeg)
            {
                Assert.IsTrue(
                    expectedPhrases[idx++].Equals(s.Text, StringComparison.Ordinal)
                    );
            }
        }

    }
}
