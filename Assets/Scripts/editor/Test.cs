using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
/*
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
            var dirPath = "D:/Program Files (x86)/Steam/steamapps/common/Europa Universalis IV/history/provinces/";
            var dirInfo = new DirectoryInfo(dirPath);

            foreach (var file in dirInfo.GetFiles())
            {
                try
                {
                    var sr2 = new StreamReader(dirPath + file.Name);
                    var sss = file.Name.Substring(0, file.Name.LastIndexOf(".")).Split(
                        new string[] {" - "}, StringSplitOptions.RemoveEmptyEntries);
                    var id = Convert.ToInt32(sss[0]);
                    var province_name = sss[1];
                    ProvinceMeta meta = new ProvinceMeta();
                meta.Id = id;
                meta.kLoadColor = dictionary[id];
                meta.name = province_name;
                var context = sr2.ReadToEnd();
                var pattern1 = "(?:^|\n)owner (.*)\n";
                var con_match = Regex.Match(context, pattern1);
                if (!con_match.Success)
                {
                    var pattern2 = "(?:^|\n)culture (.*)\n";
                    var cul_match = Regex.Match(context, pattern2);
                    if (!cul_match.Success)
                    {
                        meta.owner = "ter";
                        var json11 = JsonConvert.SerializeObject(meta,Formatting.Indented);
                        sr2.Close();
                        var outpath11 = Application.streamingAssetsPath + "/temp/provinces/" + id + " - " + province_name + ".json";
                        var writer11 = new StreamWriter(outpath11);
                        writer11.Write(json11);
                        writer11.Flush();
                        writer11.Close();
                        continue;
                    }
                    meta.owner = "non";
                    var json22 = JsonConvert.SerializeObject(meta,Formatting.Indented);
                    sr2.Close();
                    var outpath22 = Application.streamingAssetsPath + "/temp/provinces/" + id + " - " + province_name + ".json";
                    var writer22 = new StreamWriter(outpath22);
                    writer22.Write(json22);
                    writer22.Flush();
                    writer22.Close();

                    meta.owner = "non";
                    continue;
                }
                var sss2 = con_match.ToString().Split(new char[] {' ', '='}, StringSplitOptions.RemoveEmptyEntries);
                var tag = sss2[1];
                meta.owner = tag.Substring(0,3);
                var json = JsonConvert.SerializeObject(meta,Formatting.Indented);
                sr2.Close();
                var outpath = Application.streamingAssetsPath + "/temp/provinces/" + id + " - " + province_name + ".json";
                var writer = new StreamWriter(outpath);
                writer.Write(json);
                writer.Flush();
                writer.Close();

                }
                catch (FormatException)
                {
                    Debug.Log("出现问题啦" + file.Name);
                }


            }
        }
*/
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
                        Debug.Log("出错误的是：" + name);
                        unused_name.Add(name, 0);
                        continue;
                    }

                    var sss = match.ToString().Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                    byte r = Convert.ToByte(sss[3]);
                    byte g = Convert.ToByte(sss[4]);
                    byte b = Convert.ToByte(sss[5]);
                    Color32 color = new Color32();
                    color.r = r;
                    color.g = g;
                    color.b = b;
                    nameList.Add(name);
                    CountryMeta meta = new CountryMeta(name, color);
                    metaDic.Add(name, meta);
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

            Debug.Log("color国家总数为：" + nameList.Count);

            var dirPath2 = "D:/Program Files (x86)/Steam/steamapps/common/Europa Universalis IV/history/countries/";
            var directoryInfo_2 = new DirectoryInfo(dirPath2);
            foreach (var fileInfo in directoryInfo_2.GetFiles())
            {
                StreamReader reader = new StreamReader(dirPath2 + fileInfo.Name);
                var temp_name = fileInfo.Name.Replace(" - ", "-");
                var ss = temp_name.Split(new char[] {'-'}, StringSplitOptions.RemoveEmptyEntries);
                var tag = ss[0];
                var name = ss[1].Substring(0, ss[1].LastIndexOf("."));
                CountryMeta meta;
                if (!metaDic.TryGetValue(name, out meta))
                {
                    Debug.Log("找不到：" + name);
                }

                meta.tag = tag;
                String context = reader.ReadToEnd();
                //Debug.Log(name);
                var json = JsonConvert.SerializeObject(meta, Formatting.Indented);
                var outpath = Application.streamingAssetsPath + "/temp/countries/" + tag + " - "+name + ".json";
                var writer = new StreamWriter(outpath);
                writer.Write(json);
                writer.Flush();
                writer.Close();
            }
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