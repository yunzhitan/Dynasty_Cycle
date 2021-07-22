using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine.TestTools;
using country;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace editor
{
    public class Test
    {
        [Test]
        public void TestSimplePasses()
        {
            StreamReader reader = new StreamReader(Application.streamingAssetsPath
                                                   + "/temp/Yaka.json");
            String jsonstr = reader.ReadToEnd();
            CountryMeta meta = JsonConvert.DeserializeObject<CountryMeta>(jsonstr);
            Debug.Log(meta);
            // Use the Assert class to test conditions.
        }

        [Test]
        public void WriteProvince()
        {
            var dictionary = new Dictionary<int, Color32>();
            var colorFile = Application.streamingAssetsPath + "/province/province_color.txt";
            var sr = new StreamReader(colorFile);

            while (sr.Peek() >= 0)
            {
                var s = sr.ReadLine().Split(new char[]{' '},StringSplitOptions.RemoveEmptyEntries);
                if(s.Length == 0) continue;
                int id = Convert.ToInt32(s[0]);
                byte r = Convert.ToByte(s[1]);
                byte g = Convert.ToByte(s[2]);
                byte b = Convert.ToByte(s[3]);
                var color = new Color32();
                color.r = r;
                color.g = g;
                color.b = b;
                dictionary.Add(id,color);
            }
            sr.Close();
            var dirPath = "F:/Europa Universalis IV/history/provinces/";
            var dirInfo = new DirectoryInfo(dirPath);

            foreach (var file in dirInfo.GetFiles())
            {
                var sr2 = new StreamReader(dirPath + file.Name);
                var sss = file.Name.Substring(0, file.Name.LastIndexOf(".")).Split(
                    new char[] {' ', '-'}, StringSplitOptions.RemoveEmptyEntries);
                var id = Convert.ToInt32(sss[0]);
                var province_name = sss[1];
                Debug.Log(id + " " + province_name);
                var context = sr2.ReadToEnd();
                var pattern1 = "(?:^|\n)culture(.*)\n";
                var pattern2 = "(?:^|\n)religion (.*)\n";
                var pattern3 = "(?:^|\n)controller (.*)\n";

                var cul_match = Regex.Match(context, pattern1);
                var rel_match = Regex.Match(context, pattern2);
                var con_match = Regex.Match(context, pattern3);
                
                sr2.Close();
            }
        }

        [Test]
        public void WriteCountryToJson()
        {
            
            var nameList = new List<string>();
            var metaDic = new Dictionary<string, CountryMeta>();
            var unused_name = new Dictionary<string, int>();
            var dirPath = Application.streamingAssetsPath + "/countries/";
            var directoryInfo = new DirectoryInfo(dirPath);
            
            foreach (var file in directoryInfo.GetFiles())
            {
                try
                {
                    if (Path.GetExtension(file.Name) == ".meta") continue;
                    StreamReader reader = new StreamReader(dirPath + file.Name);
                    String name = file.Name.Substring(0, file.Name.LastIndexOf("."));
                    String context = reader.ReadToEnd();
                    String pattern = "(?:^|\n)color(.*)\n";
                    Match match = Regex.Match(context, pattern);
                    if (!match.Success)
                    {
                        Debug.Log("出错误的是："+name);
                        unused_name.Add(name,0);
                        continue;
                    }
                    var sss = match.ToString().Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                    byte r = Convert.ToByte(sss[3]);  byte g = Convert.ToByte(sss[4]);  byte b = Convert.ToByte(sss[5]);
                    Color32 color = new Color32();
                    color.r = r;  color.g = g;  color.b = b;
                    nameList.Add(name);
                    CountryMeta meta = new CountryMeta(name,color);
                    metaDic.Add(name,meta);
                }
                catch (IndexOutOfRangeException)
                {
                    Debug.Log("出错误的文件是：" + file.Name);
                }
                catch (System.FormatException)
                {
                    Debug.Log("出错误的文件是：" + file.Name);
                }
            }
            Debug.Log(nameList.Count);
            
            var dirPath2 = "F:/steam/steamapps/common/Europa Universalis IV/history/countries/";
            var directoryInfo_2 = new DirectoryInfo(dirPath2);

            foreach (var fileInfo in directoryInfo_2.GetFiles())
            {
                StreamReader reader = new StreamReader(dirPath2 + fileInfo.Name);
                var temp_name = fileInfo.Name.Replace(" - ","-");
                var ss = temp_name.Split(new char[]{'-'},StringSplitOptions.RemoveEmptyEntries);
                var tag = ss[0];
                var name = ss[1].Substring(0, ss[1].LastIndexOf("."));
                CountryMeta temp;
                if (!metaDic.TryGetValue(name, out temp))
                {
                    Debug.Log("找不到：" + name);
                }
                String context = reader.ReadToEnd();
                //Debug.Log(name);
            }
            
            /*
            var json = JsonConvert.SerializeObject(meta, Formatting.Indented);
            var outpath = Application.streamingAssetsPath + "/temp/" + name + ".json";
            var writer = new StreamWriter(outpath);
            JsonWriter jsonWriter = new JsonTextWriter(writer);
            writer.Write(json);
            writer.Flush();
            */
        }

        // A UnityTest behaves like a coroutine in PlayMode
        // and allows you to yield null to skip a frame in EditMode
        [UnityTest]
        public IEnumerator TestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // yield to skip a frame
            yield return null;
        }
    }
}