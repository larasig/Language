using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ditw.App.Util
{
    /// <summary>
    /// a property bag, not thread-safe!
    /// </summary>
    public class PropertyBag
    {
        private Dictionary<String, Object> _properties =
            new Dictionary<String, Object>();


        public String[] PropertyNames
        {
            get
            {
                return _properties.Keys.ToArray();
            }
        }

        public Object this[String property]
        {
            get { return _properties[property]; }
        }

        public void Update(String property, Object val)
        {
            _properties[property] = val;
        }

        public void Add(String property, Object val)
        {
            Add(property, val, false);
        }


        public void Add(String property, Object val, Boolean update)
        {
            if (!update && _properties.ContainsKey(property))
            {
                throw new ArgumentException("Property already defined.", property);
            }

            _properties[property] = val;
        }

        public Int32 Count
        {
            get { return _properties.Count; }
        }

        public void Remove(String property)
        {
            if (_properties.ContainsKey(property))
            {
                _properties.Remove(property);
            }
            else
            {
                throw new ArgumentException("Property not found.", property);
            }
        }

        public Boolean Contains(String propertyName)
        {
            return _properties.ContainsKey(propertyName);
        }
    }
}
