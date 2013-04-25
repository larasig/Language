using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Ditw.App.Lang.Tokenizer
{
    public interface ITextSegment
    {
        String Source
        {
            get;
        }

        Int32 StartIndex
        {
            get;
        }

        Int32 Length
        {
            get;
        }

        Boolean Dividable
        {
            get;
        }

        String Text
        {
            get;
        }

        Boolean IsInternal
        {
            get;
        }

        IList<ITextSegment> Decompose();

        #region DEBUG
        void TraceSegment();
        #endregion
    }

    public class BasicTextSegment : ITextSegment
    {
        public static Boolean Contain(ITextSegment ts, Int32 index)
        {
            return ts.StartIndex <= index && index < ts.StartIndex + ts.Length;
        }

        public static Boolean Contain(ITextSegment ts1, ITextSegment ts2)
        {
            return Contain(ts1, ts2.StartIndex) && Contain(ts1, ts2.StartIndex + ts2.Length - 1);
        }

        public BasicTextSegment(
            String source,
            Int32 index,
            Int32 length)
        {
            Source = source;
            StartIndex = index;
            Length = length;
        }

        public String Source
        {
            get;
            internal set;
        }

        public Int32 StartIndex
        {
            get;
            private set;
        }

        public Int32 Length
        {
            get;
            private set;
        }

        public virtual bool Dividable
        {
            get { return true; }
        }

        public virtual IList<ITextSegment> Decompose()
        {
            return new List<ITextSegment>(1) { this };
        }


        public string Text
        {
            get { return Source.Substring(StartIndex, Length); }
        }


        public virtual void TraceSegment()
        {
            var children = Decompose();
            if (children.Count > 1)
            {
                if (children.Where(c => !c.IsInternal).Count() > 0)
                {
                    Trace.WriteLine(Text);
                    Trace.WriteLine("-----------------------------------");
                }
                foreach (var s in children)
                {
                    s.TraceSegment();
                }
            }
            else
            {
                if (!IsInternal)
                {
                    Trace.WriteLine(Text);
                }
            }
        }


        public virtual bool IsInternal
        {
            get { return true; }
        }
    }

    public class CompoundTextSegment : BasicTextSegment
    {
        public CompoundTextSegment(
            String source,
            Int32 index,
            Int32 length)
            : base(source, index, length)
        {
            _decomposeList = base.Decompose().ToList();
        }

        private List<ITextSegment> _decomposeList;

        internal void InsertSegments(params ITextSegment[] segments)
        {
            for (Int32 i = 0; i < segments.Length; i++)
            {
                ITextSegment containingSegment = null;
                Int32 segIndex = 0;
                foreach (var ts in _decomposeList)
                {
                    segIndex ++;
                    if (BasicTextSegment.Contain(ts, segments[i]))
                    {
                        containingSegment = ts;
                        break;
                    }
                }

                if (containingSegment == null)
                {
                    throw new Exception("no containing segment found!");
                }

                // pre
                if (containingSegment.StartIndex < segments[i].StartIndex)
                {
                    BasicTextSegment preSegment = new BasicTextSegment(
                        Source, containingSegment.StartIndex, segments[i].StartIndex - containingSegment.StartIndex);
                    _decomposeList.Insert(segIndex++, preSegment);
                }
                // curr
                _decomposeList.Insert(segIndex++, segments[i]);
                // post
                Int32 endIndex1 = containingSegment.StartIndex + containingSegment.Length;
                Int32 endIndex2 = segments[i].StartIndex + segments[i].Length;
                if (endIndex1 > endIndex2)
                {
                    BasicTextSegment preSegment = new BasicTextSegment(
                        Source, endIndex2,
                        endIndex1 - endIndex2);
                    _decomposeList.Insert(segIndex++, preSegment);
                }
                _decomposeList.Remove(containingSegment);
            }
        }

        public override IList<ITextSegment> Decompose()
        {
            return _decomposeList;
        }
    }
}
