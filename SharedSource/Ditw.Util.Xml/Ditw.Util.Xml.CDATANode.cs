using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Ditw.Util.Xml
{
    public class CDATANode : IXmlSerializable
    {
        private Dictionary<String, String> _attribMapping =
            new Dictionary<String, String>();

        private String _value;

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader.HasAttributes)
            {
                while (reader.MoveToNextAttribute())
                {
                    _attribMapping.Add(reader.Name, reader.Value);
                }
                reader.MoveToContent();
            }

            _value = reader.ReadElementContentAsString();
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (String k in _attribMapping.Keys)
            {
                writer.WriteAttributeString(k, _attribMapping[k]);
            }

            writer.WriteCData(_value);
        }

        public String GetAttribute(String attribName)
        {
            if (_attribMapping.ContainsKey(attribName))
            {
                return _attribMapping[attribName];
            }
            return String.Empty;
        }

        public String this[String attribName]
        {
            get { return GetAttribute(attribName); }
        }

        public String CData
        {
            get { return _value; }
        }
    }
}
