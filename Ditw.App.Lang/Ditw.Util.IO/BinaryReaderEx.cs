using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Ditw.Util.IO
{
    public static class BinaryReaderEx
    {
        public static Byte[] ReadAllBytes(this BinaryReader reader, Int32 bufferSize = 4096)
        {
            Byte[] resultBytes = new Byte[bufferSize];
            Int32 resultIndex = 0;
            Byte[] tmpBytes = reader.ReadBytes(bufferSize);
            while (tmpBytes.Length > 0)
            {
                if (tmpBytes.Length > resultBytes.Length - resultIndex)
                {
                    // grow resultBytes size
                    Byte[] resultBytes2 = new Byte[resultBytes.Length * 2];
                    Array.Copy(resultBytes, resultBytes2, resultIndex);
                    resultBytes = resultBytes2;
                }

                Array.Copy(tmpBytes, 0, resultBytes, resultIndex, tmpBytes.Length);
                resultIndex += tmpBytes.Length;
                tmpBytes = reader.ReadBytes(bufferSize);
            }

            Byte[] r = new Byte[resultIndex];
            Array.Copy(resultBytes, r, resultIndex);

            return r;
        }
    }
}
