using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ditw.App.Lang.Pattern
{
    public interface IMatchFilter
    {
        MatchInfoColl Filter(MatchInfoColl mic);
    }

    public static class MatchFilters
    {
        private static IEnumerable<MatchInfo> _KeepShortest(IEnumerable<MatchInfo> ml)
        {
            List<MatchInfo> filteredList = new List<MatchInfo>(ml.Count());
            filteredList.AddRange(ml);
            foreach (var mi in ml)
            {
                foreach (var mif in filteredList)
                {
                    if (mif.RangeContains(mi) && !mif.RangeEqual(mi))
                    {
                        filteredList.Remove(mif);
                        break;
                    }
                }
            }
            return filteredList;
        }

        private static IEnumerable<MatchInfo> _ExcludeClauseSeparator(IEnumerable<MatchInfo> ml)
        {
            List<MatchInfo> filteredList = new List<MatchInfo>(ml.Count());

            foreach (var mi in ml)
            {
                if (!BuiltinPatterns.CLAUSE_SEPARATOR.IsMatch(mi.Text))
                {
                    filteredList.Add(mi);
                }
            }
            return filteredList;
        }

        public static readonly Func<IEnumerable<MatchInfo>, IEnumerable<MatchInfo>> KeepShortest = _KeepShortest;
        public static readonly Func<IEnumerable<MatchInfo>, IEnumerable<MatchInfo>> ExcludeClauseSeparator = _ExcludeClauseSeparator;
        //public static readonly MatchInfoFilter KeepShortest = new MatchInfoFilterKeepShortest();
    }

    public class MatchInfoColl
    {
        private IEnumerable<MatchInfo> _matchInfo;
        public IEnumerable<MatchInfo> Matches
        {
            get { return _matchInfo; }
        }

        private MatchInfoColl(IEnumerable<MatchInfo> mi)
        {
            _matchInfo = mi;
        }

        public static MatchInfoColl FromEnumerables(IEnumerable<MatchInfo> mi)
        {
            return new MatchInfoColl(mi);
        }

        public MatchInfoColl ApplyFilter(IMatchFilter filter)
        {
            return filter.Filter(this);
        }

    }
}
