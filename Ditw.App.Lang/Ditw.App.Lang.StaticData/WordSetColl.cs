using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ditw.App.Lang.StaticData
{
    public class WordSetColl
    {
        private Dictionary<String, WordSet> _wordSetMapping;
        private String _configPath;

        public WordSetColl(String path, Int32 size)
        {
            _configPath = path;
            _wordSetMapping = new Dictionary<String, WordSet>(size);
        }

        public WordSetColl(String path)
            : this(path, 128)
        {
        }

        public void Add(String name, WordSet set)
        {
            if (_wordSetMapping.ContainsKey(name))
            {
                throw new ArgumentException("Word set (of the same name) exists!", name);
            }

            _wordSetMapping.Add(name, set);
        }

        public WordSet this[String name]
        {
            get { return _wordSetMapping[name]; }
        }

        public Boolean Contains(String name)
        {
            return _wordSetMapping.ContainsKey(name);
        }
    }
}
