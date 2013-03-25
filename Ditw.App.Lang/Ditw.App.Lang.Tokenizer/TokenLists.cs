using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Ditw.App.Lang.Tokenizer
{
    public class CharTokenList : ITokenList
    {
        private String _rawText;
        public CharTokenList(String rawText)
        {
            _rawText = rawText;
        }

        public string RawText
        {
            get { return _rawText; }
        }

        public IEnumerable<IToken> Tokens
        {
            get
            {
                for (Int32 i = 0; i < _rawText.Length; i++)
                {
                    yield return new CharToken(_rawText, i);
                }
            }
        }

        public void TraceTokens()
        {
            throw new NotImplementedException();
        }
    }

    public class WordTokenList : ITokenList
    {
        private List<IToken> _tokenList;
        private String _cachedRawText;

        public WordTokenList(IEnumerable<IToken> tokens)
        {
            _tokenList = new List<IToken>(tokens.Count());
            _tokenList.AddRange(tokens);
        }

        public string RawText
        {
            get
            {
                if (String.IsNullOrEmpty(_cachedRawText))
                {
                    StringBuilder builder = new StringBuilder();
                    _tokenList.ForEach(
                        t => builder.Append(t.ToString())
                        );
                    _cachedRawText = builder.ToString();
                }
                return _cachedRawText;
            }
        }

        public IEnumerable<IToken> Tokens
        {
            get { return _tokenList; }
        }

        public void TraceTokens()
        {
            Trace.WriteLine(RawText);
            foreach (var t in Tokens)
            {
                Trace.Write(t.FirstCharIndex);
                Trace.Write(t.ToString());
                //Trace.Write(t.FirstCharIndex + t.Length);
                Trace.Write(' ');
            }
        }
    }
}
