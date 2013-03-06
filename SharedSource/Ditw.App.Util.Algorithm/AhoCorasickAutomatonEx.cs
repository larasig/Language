using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ditw.App.Util.Algorithm
{
    public interface IAhoCorasickAutomatonable<TChar>
    {
        IList<TChar> AhoCorasickAutomatonSequence
        {
            get;
        }
    }

    public class AhoCorasickAutomaton<TChar>
    {
        private AhoCorasickAutomaton _internal;

        private const Char StartChar = '一'; //Convert.ToChar(0x4000);

        private Dictionary<TChar, Char> _charMapping;
        private Dictionary<String, IAhoCorasickAutomatonable<TChar>> _wordMapping;

        private IAhoCorasickAutomatonable<TChar>[] _dictWords;

        public AhoCorasickAutomaton(
            IAhoCorasickAutomatonable<TChar>[] tDictWords
            )
        {
            _dictWords = tDictWords;

            BuildMapping();
        }

        private static Char Increment(ref Char c)
        {
            Int32 i = Convert.ToInt32(c);
            c = Convert.ToChar(++i);
            return c;
        }

        private void BuildMapping()
        {
            //_internal = new AhoCorasickAutomaton();
            _charMapping = new Dictionary<TChar, Char>();
            _wordMapping = new Dictionary<String, IAhoCorasickAutomatonable<TChar>>();

            Char currChar = StartChar;

            for (Int32 i = 0; i < _dictWords.Length; i++)
            {
                //String wordMappingKey = String.Empty;
                Char[] keyChars = new Char[_dictWords[i].AhoCorasickAutomatonSequence.Count];
                Int32 keyCharIdx = 0;
                foreach (var c in _dictWords[i].AhoCorasickAutomatonSequence)
                {
                    if (!_charMapping.ContainsKey(c))
                    {
                        _charMapping[c] = Increment(ref currChar);
                    }
                    keyChars[keyCharIdx++] = _charMapping[c];
                }
                String wordMappingKey = new String(keyChars);
                if (!_wordMapping.ContainsKey(wordMappingKey))
                {
                    _wordMapping[wordMappingKey] = _dictWords[i];
                }
                else
                {
                    Debug.WriteLine("Duplicate key: " + wordMappingKey);
                }
            }

            _internal = new AhoCorasickAutomaton();
            _internal.Initialize(_wordMapping.Keys);

        }

        public IEnumerable<IAhoCorasickAutomatonable<TChar>> GetKeywordsPosition(
            IList<TChar> input,
            Boolean useLongest = true)
        {
            Char[] inputChars = new Char[input.Count];
            Int32 idx = 0;
            foreach (TChar tc in input)
            {
                inputChars[idx++] = _charMapping[tc];
            }
            String matchTarget = new String(inputChars);

            List<IAhoCorasickAutomatonable<TChar>> matchInfo =
                new List<IAhoCorasickAutomatonable<TChar>>();
            foreach (var m in _internal.GetKeywordsPosition(matchTarget))
            {
                matchInfo.Add(
                    _wordMapping[m.Content]);
            }

            return matchInfo;
        }

    }
}
