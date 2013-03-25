using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ditw.App.Util.Algorithm;

namespace Ditw.App.Lang.Tokenizer
{
    public interface ITokenizer
    {
        ITokenList Tokenize(String inputText);

        ITokenList Tokenize(ITokenList inputTokenList);
        //public  GetTokens
    }

}
