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
                mic.Matches
                    .Where(
                        m => CheckMatch(tokens, m)//m.CheckAgainstTokens(tokens)
                    )
                );
        }

        private Boolean CheckMatch(ITokenList tokenList, MatchInfo m)
        {
            if (m.SubMatches == null)
            {
                return CheckIndexes(tokenList, m.Index, m.Length);
            }

            foreach (var subMatch in m.SubMatches)
            {
                if (!CheckMatch(tokenList, subMatch))
                {
                    return false;
                }
            }

            return true;
        }

        private Boolean CheckIndexes(
            ITokenList tokenList,
            Int32 start,
            Int32 length
            )
        {
            Int32 next = start + length;
            Boolean startOk = false, endOk = false;
            foreach (var t in tokenList.Tokens)
            {
                if (t.FirstCharIndex == start
                    || t.FirstCharIndex + t.Length == start // relax restriction
                    )
                {
                    startOk = true;
                    //continue;
                }
                if (t.FirstCharIndex + t.Length == next
                    || t.FirstCharIndex == next // relax restriction
                    )
                {
                    endOk = true;
                    //continue;
                }
            }

            return startOk && endOk;
        }

    }

}
