using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ditw.App.Lang.Pattern;

namespace Ditw.App.Lang.Tokenizer
{
    public class TokenizerFilter : IMatchFilter
    {
        private ITokenizer _tokenizer;
        public TokenizerFilter(ITokenizer tokenizer)
        {
            _tokenizer = tokenizer;
        }

        public MatchInfoColl Filter(MatchInfoColl mic)
        {
            if (mic.Matches.Count() <= 0)
                return mic;
            String inputText = mic.Matches.First().SrcText;
            var tokens = _tokenizer.Tokenize(inputText);

            return MatchInfoColl.FromEnumerables(
                mic.Matches.Where(
                    m => m.CheckAgainstTokens(tokens)
                    )
                );
        }
    }

}
