using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ditw.App.Lang.Tokenizer;
using System.Diagnostics;

namespace Ditw.Test.Lang.RegexTokenUnitTest
{
    [TestClass]
    public class NameTest_Initial
    {
        static void RunTest_Match(RegexToken regexToken, String txt,
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

        [TestMethod]
        public void Test_SentenceStart()
        {
            var token = Loader.GetToken();

            RunTest_Match(token,
    "First discovered in 2011, the Shylock banking Trojan affects virtually all versions of Windows from Windows 2000 onward, and has turned into one of the most advanced forms of financial fraud malware around.",
    "Shylock banking Trojan");

            RunTest_Match(token,
    "It employed malware that can wipe the contents of a computer's hard disk as well as drives connected to the infected computer.");

        }

        [TestMethod]
        public void Test_Initials()
        {
            var token = Loader.GetToken(
                "words_initial_test.xml",
                null,
                new String[] { "pos.reggrp", "words_initial.reggrp" }
                );

            RunTest_Match(token,
    "First discovered in 2011, I'm the Shylock banking Trojan affects virtually all versions of Windows from Windows 2000 onward, and has turned into one of the most advanced forms of financial fraud malware around.",
    new String[] { "I", "Shylock", "Trojan", "Windows", "Windows" }
    );

        }
    }
}
