using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Ditw.Util.Xml
{
    public static class XmlUtil
    {
        public static String Serialize<T>(T o)
        {
            //XmlAttributes attribs = new XmlAttributes();
            //attribs.XmlText = new XmlTextAttribute(typeof(MyXmlCDataString));
            //XmlAttributeOverrides overrides = new XmlAttributeOverrides();
            //overrides.Add(typeof(dataGeneralMenu), "menu", attribs);

            XmlSerializer s = new XmlSerializer(typeof(T)); //, overrides);
            //StringBuilder builder = new StringBuilder();

            MemoryStream ms = new MemoryStream();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;
            using (XmlWriter xmlWriter = XmlWriter.Create(ms, settings))
            {
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add(String.Empty, String.Empty);
                s.Serialize(xmlWriter, o, ns);
            }
            Byte[] bytes = ms.ToArray();
            // discard the BOM part
            Int32 idx = settings.Encoding.GetPreamble().Length;
            return Encoding.UTF8.GetString(bytes, idx, bytes.Length - idx);
        }

        public static void Serialize<T>(T o, Stream stream)
        {
            String str = Serialize<T>(o);

            Byte[] bytes = Encoding.UTF8.GetBytes(str);
            stream.Write(bytes, 0, bytes.Length);
        }

        public static void Serialize<T>(T o, String fileName)
        {
            String str = Serialize<T>(o);

            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                Byte[] bytes = Encoding.UTF8.GetBytes(str);
                fs.Write(bytes, 0, bytes.Length);
            }
        }

        //public static T DeserializeFromFile<T>(Uri fileUri)
        //{
            
        //}

        public static T DeserializeFromFile<T>(String fileName)
        {
            using (FileStream fstream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                return DeserializeFromStream<T>(fstream);
            }
        }

        public static T DeserializeFromStream<T>(Stream stream)
        {
            XmlSerializer s = new XmlSerializer(typeof(T));
            return (T)s.Deserialize(stream);
        }

        public static T DeserializeString<T>(String content)
        {
            using (TextReader reader = new StringReader(content))
            {
                XmlSerializer s = new XmlSerializer(typeof(T));
                return (T)s.Deserialize(reader);
            }
        }

        public static Object DeserializeString(String content, Type t)
        {
            using (TextReader reader = new StringReader(content))
            {
                XmlSerializer s = new XmlSerializer(t);
                return s.Deserialize(reader);
            }
        }


        //public static T DeserializeString<T>(String content, Encoding encoding)
        //{
        //    using (TextReader reader =
        //        new StreamReader(content, encoding))
        //    {
        //        XmlSerializer s = new XmlSerializer(typeof(T));
        //        return (T)s.Deserialize(reader);
        //    }
        //}

    }
}
