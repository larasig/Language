using System.Xml;
using System;
using System.IO;
using System.Collections.Generic;

namespace Ditw.Util.Xml
{
    public static class XmlReaderExt
    {
        public static IDictionary<String, String> GetAttributeCollection(this XmlReader reader)
        {
            IDictionary<String, String> dict = new Dictionary<String, String>();
            while (reader.MoveToNextAttribute())
            {
                dict.Add(reader.Name, reader.Value);
            }
            return dict;
        }

#if false
        public static Boolean TryReadStartElement(this XmlReader reader, String elementName)
        {
            try
            {
                reader.ReadStartElement(elementName);
                return true;
            }
            catch (XmlException)
            {
                return false;
            }
        }

        public static Boolean TryReadStartElement(this XmlReader reader, String elementName, String ns)
        {
            try
            {
                reader.ReadElementContentAs(
                return true;
            }
            catch (XmlException)
            {
                return false;
            }
        }
#endif
    }

    public class XmlReaderEx
    {
        public Action<XmlReader, Object> ReadPostProcessing;

        public void Read(String content, Object userObj)
        {
            using (XmlReader reader = XmlReader.Create(new StringReader(content)))
            {
                while (reader.Read())
                {
                    ReadPostProcessing(reader, userObj);
                }
            }
        }

    }
}
