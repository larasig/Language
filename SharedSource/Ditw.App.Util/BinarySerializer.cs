using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Ditw.App.Util
{
    public static class BinarySerializer
    {
        private static BinaryFormatter _formatter = new BinaryFormatter();

        public static void Serialize<T>(
            String filePath,
            T obj)
        {
            using (Stream stream = new FileStream(
                filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                _formatter.Serialize(stream, obj);
            }

        }

        public static T Deserialize<T>(String filePath)
        {
            using (Stream stream = new FileStream(
                filePath, FileMode.Open, FileAccess.Read))
            {
                return (T)_formatter.Deserialize(stream);
            }
        }
    }
}
