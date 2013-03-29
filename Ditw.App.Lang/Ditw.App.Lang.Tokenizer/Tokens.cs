using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ditw.App.Util.Algorithm;

namespace Ditw.App.Lang.Tokenizer
{
#if false
    public class CharToken : IToken
    {
        private Int32 _index;
        private String _srcText;

        public CharToken(String srcText, Int32 idx)
        {
            _index = idx;
            _srcText = srcText;
        }

        public string SrcText
        {
            get { return _srcText; }
        }

        public int FirstCharIndex
        {
            get { return _index; }
        }

        public int Length
        {
            get { return 1; }
        }

        public override String ToString()
        {
            return _srcText[_index].ToString();
        }
    }
#endif
    public class TokenBase : IToken
    {
        public TokenBase(String src, Int32 startIndex, Int32 length)
        {
            SrcText = src;
            FirstCharIndex = startIndex;
            Length = length;
        }

        public string SrcText
        {
            get;
            private set;
        }

        public Int32 FirstCharIndex
        {
            get;
            private set;
        }

        public Int32 Length
        {
            get;
            private set;
        }

        public override String ToString()
        {
            return SrcText.Substring(FirstCharIndex, Length);
        }

        public IList<char> CharList
        {
            get { return ToString().ToList(); }
        }
    }

    public class AtomToken : TokenBase
    {
        public AtomToken(String src, Int32 startIndex, Int32 length)
            : base(src, startIndex, length)
        {
        }
    }

    public class StringToken : IToken
    {
        private KeywordWithPositionInfo _wordPos;

        public StringToken(KeywordWithPositionInfo wordPos)
        {
            _wordPos = wordPos;
        }

        public StringToken(String srcText, Int32 idx, Int32 length)
        {
            _wordPos = new KeywordWithPositionInfo()
            {
                Source = srcText,
                FirstCharIndex = idx,
                LastCharIndex = idx + length - 1
            };
        }

        public override String ToString()
        {
            return _wordPos.Content;
        }

        public string SrcText
        {
            get { return _wordPos.Source; }
        }

        public int FirstCharIndex
        {
            get { return _wordPos.FirstCharIndex; }
        }

        public int Length
        {
            get { return _wordPos.LastCharIndex - _wordPos.FirstCharIndex + 1; }
        }


        public IList<Char> CharList
        {
            get { return _wordPos.Content.ToList(); }
        }
    }
}
