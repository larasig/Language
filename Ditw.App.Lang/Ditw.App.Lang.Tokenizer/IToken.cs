using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ditw.App.Lang.Tokenizer
{
    public interface IToken
    {
        String SrcText
        {
            get;
        }

        Int32 FirstCharIndex
        {
            get;
        }

        Int32 Length
        {
            get;
        }

        String ToString();

        IList<Char> CharList
        {
            get;
        }
    }

    public interface ITokenList
    {
        String RawText
        {
            get;
        }

        IEnumerable<IToken> Tokens
        {
            get;
        }

        #region Trace
        void TraceTokens();
        #endregion
    }
}
