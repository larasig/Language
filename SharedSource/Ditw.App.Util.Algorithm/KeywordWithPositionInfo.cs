using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ditw.App.Util.Algorithm
{
    public struct KeywordWithPositionInfo
    {
        public Int32 FirstCharIndex;
        public Int32 LastCharIndex;
        public String Source;

        public String Content
        {
            get
            {
                return Source.Substring(FirstCharIndex,
                    LastCharIndex - FirstCharIndex + 1);
            }
        }

        public static Boolean IsOverlapped(
            KeywordWithPositionInfo pos1,
            KeywordWithPositionInfo pos2)
        {
            return (pos1.FirstCharIndex >= pos2.FirstCharIndex &&
                pos1.FirstCharIndex <= pos2.LastCharIndex) ||
                (pos1.LastCharIndex >= pos2.FirstCharIndex &&
                pos1.LastCharIndex <= pos2.LastCharIndex);
        }

        public Boolean Contains(KeywordWithPositionInfo pos2)
        {
            return (pos2.FirstCharIndex >= FirstCharIndex &&
                pos2.LastCharIndex <= LastCharIndex);
        }

        #region debug
        public override string ToString()
        {
            return String.Format("{0}({1},{2})", Content, FirstCharIndex, LastCharIndex);
        }
        #endregion
    }
}
