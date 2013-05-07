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

            // Test sentences starting with at 2 words of Cap-Initial
            RunTest_Match(token,
    "A Symantec original questions ST0-134 ST0-134 test answers NO.9 According to Symantec, what is a botnet?",
    new String[] { "A Symantec", "ST0", "ST0", "NO", "According", "Symantec" }
    );

        }

        [TestMethod]
        public void Test_Hyphens()
        {
            var token = Loader.GetToken(
                "words_hyphened_test.xml",
                null,
                new String[] { "words_hyphened.reggrp" }
                );

            RunTest_Match(token,
    "Man-in-the-browser (MITB, MitB, MIB, MiB), a form of Internet threat related to man-in-the-middle (MITM), is a proxy Trojan horse[1] that infects a web browser by taking advantage of vulnerabilities in browser security to modify web pages, modify transaction content or insert additional transactions, all in a completely covert fashion invisible to both the user and host web application. A MitB attack will be successful irrespective of whether security mechanisms such as SSL/PKI and/or two or three-factor Authentication solutions are in place. A MitB attack may be countered by utilising out-of-band transaction verification, although SMS verification can be defeated by man-in-the-mobile (MitMo) malware infection on the mobile phone.",
    new String[] { "Man-in-the-browser", "man-in-the-middle", "three-factor", "out-of-band", "man-in-the-mobile" }
    );
        }

        [TestMethod]
        public void Test_WordList()
        {
            var token = Loader.GetToken(
                "word_list_test.xml",
                null,
                new String[] { "words_list.reggrp" }
                );

            RunTest_Match(token,
    "Man-in-the-browser (MITB, MitB, MIB, MiB), a form of Internet threat related to man-in-the-middle (MITM), is a proxy Trojan horse[1] that infects a web browser by taking advantage of vulnerabilities in browser security to modify web pages, modify transaction content or insert additional transactions, all in a completely covert fashion invisible to both the user and host web application. A MitB attack will be successful irrespective of whether security mechanisms such as SSL/PKI and/or two or three-factor Authentication solutions are in place. A MitB attack may be countered by utilising out-of-band transaction verification, although SMS verification can be defeated by man-in-the-mobile (MitMo) malware infection on the mobile phone.",
    new String[] { "MITB, MitB, MIB, MiB" }
    );
        }
    }
}
