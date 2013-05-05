using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ditw.App.Lang.Tokenizer;

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
    }
}
