using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ditw.App.Util.Algorithm;
using System.IO;
using System.Diagnostics;

namespace Ditw.App.Lang.Tokenizer
{
    public class ZhsTokenizer : ITokenizer
    {
        private static String _dictionaryPath;
        private static AhoCorasickAutomaton _acAuto;

        private static ZhsTokenizer _tokenizer;
        private static Object _lock = new Object();

        public static ZhsTokenizer Instance
        {
            get { return _tokenizer; }
        }

        public static void PrepareDictionary(String dictPath)
        {
            if (_acAuto != null)
                return;

            lock (_lock)
            {
                if (_acAuto != null)
                    return;

                _dictionaryPath = dictPath;

                String dictBinPath = _dictionaryPath + @"\DictionaryZhs.bin";
                if (!File.Exists(dictBinPath))
                {
                    DictionarySerializer.SerializeDict(
                        _dictionaryPath + @"\DictionaryZhs.txt", dictBinPath);
                    Console.WriteLine("Done with serialization!");
                }

                _acAuto = DictionarySerializer.DeserializeDict(dictBinPath);
                _tokenizer = new ZhsTokenizer();
            }
            
        }

        public IEnumerable<KeywordWithPositionInfo> Tokenize(String inputText)
        {
            if (_acAuto == null)
            {
                Trace.WriteLine("Dictionary not ready!");
                return null;
            }

            return _acAuto.GetKeywordsPosition(inputText);
        }
    }

}
