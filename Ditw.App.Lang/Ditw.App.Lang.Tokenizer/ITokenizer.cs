using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ditw.App.Util.Algorithm;

namespace Ditw.App.Lang.Tokenizer
{
    public interface ITokenizer
    {
        IEnumerable<KeywordWithPositionInfo> Tokenize(String inputText);

        //public  GetTokens
    }

}
