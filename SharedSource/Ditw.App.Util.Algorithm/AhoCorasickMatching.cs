using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ditw.App.Util.Algorithm
{
#if false
    public interface IExactMatchable<TChar>
    {
        String Content
        {
            get;
            set;
        }

        TChar this[Int32 i]
        {
            get;
        }

        Int32 CharCount
        {
            get;
        }
    }
#endif
    // See:
    //   http://en.wikipedia.org/wiki/Aho%E2%80%93Corasick_string_matching_algorithm
    [Serializable]
    public class AhoCorasickTrieNode
    //where TWord : IExactMatchable<TChar>
    {
        #region Serialization
        public Char CharFromParent
        {
            get;
            set;
        }

        public AhoCorasickTrieNode NextIfMiss
        {
            get;
            set;
        }

        public String Output
        {
            get;
            set;
        }

        public String Word
        {
            get;
            set;
        }
        #endregion

        public IEnumerable<String> Outputs
        {
            get
            {
                //if (Output == null)
                //{
                //    yield return null;
                //}
                //else
                //{
                String o = Output;
                AhoCorasickTrieNode n = this;
                while (!n.IsRoot)
                {
                    if (o != null)
                    {
                        yield return o;
                    }
                    o = n.NextIfMiss.Output;
                    n = n.NextIfMiss;
                }
                //}
            }
        }

        private Dictionary<Char, AhoCorasickTrieNode>
            _nextIfMatchMapping = new Dictionary<Char, AhoCorasickTrieNode>();

        internal AhoCorasickTrieNode CreateNextMatch(Char c,
            String w)
        {
            if (_nextIfMatchMapping.ContainsKey(c))
            {
                //Debug.WriteLine(String.Format("'{0}' '{1}' already present",
                //    c, w));
            }
            else
            {
                AhoCorasickTrieNode newNode =
                    new AhoCorasickTrieNode()
                    {
                        Word = w,
                        CharFromParent = c
                    };
                _nextIfMatchMapping[c] = newNode;
            }

            return _nextIfMatchMapping[c];
        }

        internal void PointMatchTo(Char c, AhoCorasickTrieNode match)
        {
            if (_nextIfMatchMapping.ContainsKey(c))
            {
                throw new Exception("'{0}' already pointed!");
            }
            _nextIfMatchMapping[c] = match;
        }

        internal Boolean IsRoot
        {
            get { return NextIfMiss == null; }
        }

        internal Boolean IsMiss(Char c)
        {
            return !_nextIfMatchMapping.ContainsKey(c);
        }

        public AhoCorasickTrieNode Move(Char nextChar)
        {
            return _nextIfMatchMapping.ContainsKey(nextChar) ?
                _nextIfMatchMapping[nextChar] : NextIfMiss;
        }


        private IEnumerable<AhoCorasickTrieNode> Children
        {
            get
            {
                foreach (Char c in _nextIfMatchMapping.Keys)
                {
                    yield return _nextIfMatchMapping[c];
                }
            }
        }

        #region for debugging
        public override string ToString()
        {
            return "[" + Word + "]";
        }

        public void DebugTraverse()
        {
            Debug.Write(this);
            Debug.Write(" Children: ");
            foreach (AhoCorasickTrieNode child in Children)
            {
                Debug.Write(" ");
                if (child != this)
                {
                    Debug.Write(child.CharFromParent + "->");
                }
                Debug.Write(child);
            }
            Debug.Write("; Miss: " + this.NextIfMiss);
            Debug.Write("; Outputs: ");
            foreach (String o in Outputs)
            {
                if (o != null)
                    Debug.Write(o + '|');
            }
            Debug.WriteLine(String.Empty);

            foreach (AhoCorasickTrieNode child in Children)
            {
                if (child != this)
                    child.DebugTraverse();
            }
        }
        #endregion
    }

    [Serializable]
    public class AhoCorasickAutomaton
    //where TWord : IExactMatchable<TChar>
    {

        private AhoCorasickTrieNode _root;
        public AhoCorasickTrieNode TrieRoot
        {
            get { return _root; }
        }

        private String[] _entriesRef;

        private HashSet<Char> _alphabet;

        public AhoCorasickAutomaton()
        {
        }

        private void ConstructionPhaseI()
        {
            _alphabet = new HashSet<Char>();

            // Phase I:
            foreach (String w in _entriesRef)
            {
                AhoCorasickTrieNode currentNode = _root;
                for (Int32 i = 0; i < w.Length; i++)
                {
                    currentNode.CreateNextMatch(w[i],
                        w.Substring(0, i + 1)
                        );
                    if (!_alphabet.Contains(w[i]))
                    {
                        _alphabet.Add(w[i]);
                    }
                    currentNode = currentNode.Move(w[i]);
                }
                currentNode.Output = w;
            }

            foreach (Char c in _alphabet)
            {
                if (_root.IsMiss(c))
                {
                    _root.PointMatchTo(c, _root);
                }
            }

        }

        private void ConstructionPhaseII()
        {
            Queue<AhoCorasickTrieNode> q = new Queue<AhoCorasickTrieNode>();

            foreach (Char c in _alphabet)
            {
                AhoCorasickTrieNode n = _root.Move(c);
                if (n != _root)
                {
                    n.NextIfMiss = _root;
                    q.Enqueue(n);
                }
            }

            while (q.Count != 0)
            {
                AhoCorasickTrieNode n = q.Dequeue();
                foreach (Char c in _alphabet)
                {
                    if (!n.IsMiss(c))
                    {
                        AhoCorasickTrieNode u = n.Move(c);
                        q.Enqueue(u);
                        AhoCorasickTrieNode v = n.NextIfMiss;
                        //if (!v.IsMiss(c))
                        //{
                        //    u.NextIfMiss = v.Move(c);
                        //}
                        //else
                        //{
                        //    u.NextIfMiss = v;
                        //}
                        while (v.IsMiss(c) && v != _root)
                        {
                            v = v.NextIfMiss;
                        }
                        u.NextIfMiss = v.Move(c);
                    }
                }
            }
        }

        public void Initialize(params IEnumerable<String>[] entries)
        {
            Int32 totalLength = entries.Sum(a => a.Count());
            _entriesRef = new String[totalLength];
            Int32 idx = 0;
            for (Int32 i = 0; i < entries.Length; i++)
            {
                Array.Copy(entries[i].ToArray(), 0, _entriesRef, idx,
                    entries[i].Count()
                    );
                idx += entries[i].Count();
            }
            _root = new AhoCorasickTrieNode()
            {
                Word = "_$ROOT$_"
            };

            ConstructionPhaseI();
            ConstructionPhaseII();
        }

        public IEnumerable<KeywordWithPositionInfo> GetKeywordsPosition(String input,
            Boolean useLongest = true)
        {
            AhoCorasickTrieNode n = TrieRoot;
            List<KeywordWithPositionInfo> posList = new List<KeywordWithPositionInfo>();
            for (Int32 i = 0; i < input.Length; i++)
            {
                while (!n.IsRoot && n.IsMiss(input[i]))
                {
                    n = n.NextIfMiss;
                }

                if (!n.IsMiss(input[i]))
                {
                    n = n.Move(input[i]);
                }
                else
                {
                    Debug.WriteLine(input[i] + " not found in dictionary!");
                    continue;
                }

                //Debug.WriteLine("|------------------ " + i.ToString() + " ---- " + n.ToString());
#if false
                if (useLongest)
                {
                    yield return n.Outputs.Max();
                }
                else
                {
                    foreach (String k in n.Outputs)
                    {
                        yield return k;
                    }
                }
#else
                foreach (String k in n.Outputs)
                {
                    posList.Add(new KeywordWithPositionInfo()
                        {
                            LastCharIndex = i,
                            FirstCharIndex = i - k.Length + 1,
                            Source = input
                        }
                    );
                }
#endif
            }

            if (useLongest)
            {
                List<KeywordWithPositionInfo> longestList = new List<KeywordWithPositionInfo>();
                var enuOrigin = posList.GetEnumerator();

                while (enuOrigin.MoveNext())
                {
                    KeywordWithPositionInfo posOrigin = enuOrigin.Current;
                    while (longestList.Count > 0)
                    {
                        KeywordWithPositionInfo lastPos =
                            longestList.Last();
                        if (posOrigin.Contains(lastPos))
                        {
                            longestList.RemoveAt(
                                longestList.Count - 1);
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (longestList.Count == 0 ||
                        !longestList.Last().Contains(posOrigin))
                    {
                        longestList.Add(posOrigin);
                    }
                }
                return longestList;
            }
            else
            {
                return posList;
            }
        }
    }
}
