using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using Ditw.App.Util;
using Ditw.App.Util.Algorithm;
//using Ditw.Util.Algorithm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Ditw.Util.IO;
using System.Runtime.Serialization;

namespace Ditw.Test.App.Util.Algorithm
{
    [TestClass]
    public class AhoCorasickMatchingTest
    {
        private String[] LoadDict(String fileName)
        {
            String path = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location
                );

            using (StreamReader reader = new StreamReader(
                path + "\\" + fileName))
            {
                String content = reader.ReadToEnd();
                return content.Split(new char[] { '\r', '\n' });
            }
        }

        private static readonly String[] TestTargets = new String[]
            {
	"日本防卫大臣田中直纪",
	"以色列国防部长巴拉克",
	"德国总理默克尔",
	"德国学者泽林",
	"加拿大总理斯蒂芬•哈珀(Stephen Harper)",
	"俄罗斯总理普京",
	"美国副总统拜登",
	"中国国务院副总理王岐山",
	"法国总统萨尔科齐",
	"日本首相鸠山由纪夫",
	"菲律宾总统阿基诺三世",
	"韩国总统李明博",
	"巴基斯坦外交部长希娜·拉巴尼·哈尔",
	"俄罗斯国防部长阿纳托利·谢尔久科夫",
	"俄外长拉夫罗夫",
	"日本首相野田佳彦",
	"日本首相宫泽真一",
	"俄罗斯联邦政府总理弗拉基米尔·弗拉基米罗维奇·普京",
	"朝鲜领导人金正日",
	"美国国务卿希拉里",
            };

        private void SerializeDict()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            AhoCorasickAutomaton acAuto = new AhoCorasickAutomaton();
            String[] dictEntries = LoadDict("DictionaryZhs.txt");
            sw.Stop();
            Debug.WriteLine("LoadDict: " + sw.ElapsedMilliseconds);
            sw.Restart();
            acAuto.Initialize(dictEntries);
            sw.Stop();
            Debug.WriteLine("Initialize: " + sw.ElapsedMilliseconds);

            String filePath =
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +
                    @"\zhsDict.bin";
            sw.Restart();
            BinarySerializer.Serialize(filePath, acAuto);
            sw.Stop();
            Debug.WriteLine("Serialize: " + sw.ElapsedMilliseconds);
        }

        private AhoCorasickAutomaton DeserializeDict()
        {
            String filePath =
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +
                    @"\zhsDict.bin";
            Stopwatch sw = new Stopwatch();
            sw.Start();
            AhoCorasickAutomaton auto = BinarySerializer.Deserialize<AhoCorasickAutomaton>(filePath);
            sw.Stop();
            Debug.WriteLine("Deserialize: " + sw.ElapsedMilliseconds);
            return auto;
        }

        [TestMethod]
        public void ChineseDictionarySerializationTest()
        {
            SerializeDict();
            DeserializeDict();
        }

        [TestMethod]
        public void ChineseDictionaryTest()
        {
#if false
            AhoCorasickAutomaton acAuto = new AhoCorasickAutomaton();
            String[] dictEntries = LoadDict("DictionaryZhs.txt");
            acAuto.Initialize(dictEntries);
#else
            AhoCorasickAutomaton acAuto = DeserializeDict();
#endif
            String[] testSentences = new String[] {
                "在伊朗首都德黑兰，遭炸弹袭击的汽车旁留下的血迹。",
                "当天在哥南部卡克塔省枪杀了4名被其扣押的军方人质",
            };

            //Int32 i = 0;
            foreach (String t in testSentences)
            {
                foreach (KeywordWithPositionInfo pos in acAuto.GetKeywordsPosition(t))
                {
                    Debug.WriteLine(pos);
                    //Assert.IsTrue(o[i++].Equals(s, StringComparison.Ordinal));
                }
            }
        }

    }
}
