/*
 * Created by SharpDevelop.
 * User: jiajiwu
 * Date: 1/3/2013
 * Time: 11:55 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Ditw.App.Util.Algorithm
{
	/// <summary>
	/// Description of DictionarySerializer.
	/// </summary>
	public class DictionarySerializer
	{
        private static String[] LoadDict(String fileName)
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                String content = reader.ReadToEnd();
                return content.Split(new char[] { '\r', '\n' });
            }
        }

        public static void SerializeDict(String txtPath, String binPath)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            AhoCorasickAutomaton acAuto = new AhoCorasickAutomaton();
            String[] dictEntries = LoadDict(txtPath);
            sw.Stop();
            Debug.WriteLine("LoadDict: " + sw.ElapsedMilliseconds);
            sw.Restart();
            acAuto.Initialize(dictEntries);
            sw.Stop();
            Debug.WriteLine("Initialize: " + sw.ElapsedMilliseconds);

            sw.Restart();
            BinarySerializer.Serialize(binPath, acAuto);
            sw.Stop();
            Debug.WriteLine("Serialize: " + sw.ElapsedMilliseconds);
        }

        public static AhoCorasickAutomaton DeserializeDict(String binPath)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            AhoCorasickAutomaton auto = BinarySerializer.Deserialize<AhoCorasickAutomaton>(binPath);
            sw.Stop();
            Debug.WriteLine("Deserialize: " + sw.ElapsedMilliseconds);
            return auto;
        }

	}
}
