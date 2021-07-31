using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using country;
using Newtonsoft.Json;
using province;
using UnityEngine.UI;
using Color = UnityEngine.Color;
using Debug = UnityEngine.Debug;

[ExecuteInEditMode]
public class ProvinceGrid : MonoBehaviour {

    public Texture2D kProvinceMap;
    public RenderTexture kProvinceColorMap;

    public int kProvinceCount;
    public Material kLut;

    private Dictionary<Color, int> kProvinceColorToIDMap = new Dictionary<Color, int>();
    private Dictionary<string, CountryMeta> kCountryMetaMap = new Dictionary<string, CountryMeta>();

    private ProvinceMeta[] kProvinceMeta;

    private Texture2D kProvincePolitimap;


    void Awake()
    {
        InitCountries();
        InitProvinces();
        //search();
    }
    public void InitCountries()
    {
        var dirPath = Application.streamingAssetsPath + "/temp/countries/";
        var directoryInfo_ = new DirectoryInfo(dirPath);
        foreach (var fileInfo in directoryInfo_.GetFiles())
        {
            if (Path.GetExtension(fileInfo.Name) == ".meta") continue;
            try
            {
                StreamReader reader = new StreamReader(dirPath + fileInfo.Name);
                var temp_name = fileInfo.Name.Replace(" - ", "-");
                var ss = temp_name.Split(new char[] {'-'}, StringSplitOptions.RemoveEmptyEntries);
                var tag = ss[0];
                var context = reader.ReadToEnd();
                var meta = JsonConvert.DeserializeObject<CountryMeta>(context);
                meta.initColor();
                kCountryMetaMap.Add(tag,meta);
            }
            catch (JsonReaderException)
            {
                Debug.Log("出问题的是"+fileInfo.Name);
            }
        }
    }


    private void InitProvinces()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        kProvinceMeta = new ProvinceMeta[ConfigParam.MAXProvinceCount];    
        var dirPath = Application.streamingAssetsPath + "/temp/provinces/";
        var directoryInfo_ = new DirectoryInfo(dirPath);
        int count = 0;
        foreach (var fileInfo in directoryInfo_.GetFiles())
        {
            if (Path.GetExtension(fileInfo.Name) == ".meta") continue;
            count++;
            try
            {
                StreamReader reader = new StreamReader(dirPath + fileInfo.Name);
                var context = reader.ReadToEnd();
                var meta = JsonConvert.DeserializeObject<ProvinceMeta>(context);
                meta.InitColor();
                reader.Close();
                if (!kProvinceColorToIDMap.ContainsKey(meta.kMapColor))
                {
                    kProvinceColorToIDMap.Add(meta.kMapColor, meta.Id);
                }

                if (kCountryMetaMap.ContainsKey(meta.owner))
                {
                    meta.kShowColor = kCountryMetaMap[meta.owner].color;
                    ChangeTargetProvinceColor(meta.kMapColor,meta.kShowColor);
                }
                kProvinceMeta[meta.Id] = meta;
            }
            catch (JsonReaderException)
            {
                Debug.Log("出问题的是"+fileInfo.Name);
            }
        }
        kProvincePolitimap = new Texture2D(ConfigParam.BLOCKMAXWIDTH, ConfigParam.BLOCKMAXHEIGHT);
        Graphics.ConvertTexture(kProvinceMap, kProvincePolitimap);
        kProvinceCount = count;
        //DebugProvinceCol();
        stopwatch.Stop();
        Debug.Log("INITPROVINCE所用时间为" + stopwatch.ElapsedMilliseconds);
    }
    
    private void search()
    {
        
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        SearchColor();

        if(kProvinceMap ==null)
        {
            return;
        }

        var width = kProvinceMap.width;
        var height = kProvinceMap.height;

        FileStream fileStream = new FileStream(Application.streamingAssetsPath + "/province/provinceId.txt",FileMode.Open);
        StreamReader reader = new StreamReader(fileStream);

        kProvinceMeta = new ProvinceMeta[ConfigParam.MAXProvinceCount];

        string s;
        int flag = 0;
        int count = 0;
        Color col;
        while (reader.Peek() >= 0)
        {
            count++;
            s = reader.ReadLine();
            var id = Convert.ToInt32(s);
            /*
            kProvinceColorToIDMap.TryGetValue(col, out flag);
            Debug.Assert(flag == id);
            kProvinceMeta[id] = new ProvinceMeta(id,col);
            */
        }
        
        
        fileStream.Close();
        kProvincePolitimap = new Texture2D(ConfigParam.BLOCKMAXWIDTH, ConfigParam.BLOCKMAXHEIGHT);
        Graphics.ConvertTexture(kProvinceMap, kProvincePolitimap);
        kProvinceCount = count;
        //DebugProvinceCol();
        stopwatch.Stop();
        
        Debug.Log("search函数的执行时间为：" + stopwatch.ElapsedMilliseconds);
    }

    private void SearchColor()
    {
        var path = Application.streamingAssetsPath + "/province/province_color.txt";
        var sr = new StreamReader(path);
        String[] sss = new string[4];
        try
        {
            while (sr.Peek() >= 0)
            {
                sss = sr.ReadLine().Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                if (sss.Length == 0) continue;
                var id = Convert.ToInt32(sss[0]);
                var r = (float) Convert.ToByte(sss[1]) / 255f;
                var g = (float) Convert.ToByte(sss[2]) / 255f;
                var b = (float) Convert.ToByte(sss[3]) / 255f;

                Color color = new Color(r, g, b, 1);

                if (!kProvinceColorToIDMap.ContainsKey(color))
                {
                    kProvinceColorToIDMap.Add(color, id);
                }
            }
        }
        catch (FormatException)
        {
            Debug.Log("出问题的是：" + sss);
        }

        sr.Close();
    }
    public int GetProvinceID( int x, int z)
    {
        x = x % kProvinceMap.width;
        z = z % kProvinceMap.height;

        var col = kProvinceMap.GetPixel(x, z);
        int ID = -1;

        if( kProvinceColorToIDMap.TryGetValue(col, out ID))
        {

        }

        return ID;
    }

    public int GetProvinceID( Color col)
    {
        int ID = -1;
        kProvinceColorToIDMap.TryGetValue(col, out ID);
        return ID;
    }

    public ProvinceMeta GetProvinceData( int x, int z)
    {
        return GetProvinceData(GetProvinceID(x, z));
    }

    public ProvinceMeta GetProvinceData( int ID)
    {
        if( ID <0 || ID >= kProvinceMeta.Length)
        {
            return null;
        }
        return kProvinceMeta[ID];

    }

    public ProvinceMeta GetProvinceData( Color col)
    {
        return GetProvinceData(GetProvinceID(col));
    }

    public void ChangeProvinceColor()
    {
        var width = kProvinceMap.width;
        var height = kProvinceMap.height;

        LC_Helper.Loop(width, (i) =>
        {
            LC_Helper.Loop(height, (j) =>
            {
                var col = kProvinceMap.GetPixel(i, j);
                var pdata = GetProvinceData(col);

                if(pdata != null)
                {
                    kProvincePolitimap.SetPixel(i, j, pdata.kShowColor);
                }

            });

        });

        kProvincePolitimap.Apply();

        Graphics.Blit(kProvincePolitimap, kProvinceColorMap);
    }

    public void ChangeTargetProvinceColor(Color kBaseColor, Color kTargeColor)
    {
        kLut.SetColor("_BaseColor", kBaseColor);
        kLut.SetColor("_TarColor", kTargeColor);
        kLut.SetTexture("_MainTex", kProvinceColorMap);

        Graphics.Blit(kProvinceColorMap, kProvinceColorMap, kLut);
    }

    public void DebugProvinceCol()
    {
        LC_Helper.Loop(kProvinceMeta.Length, (i) =>
        {
            var data = kProvinceMeta[i];

            if(data.Id %2 == 0)
            {
                data.kShowColor = Color.white;
                data.kShowColor.a = 1;
            }
            else
            {
                data.kShowColor = Color.black;
                data.kShowColor.a = 0;

            }



        });

    }

    public void RayTest()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, int.MaxValue, 1 << LayerMask.NameToLayer("Terrain")))
        {
            Debug.Log("检测到物体" + hit.point);
            //Pickat(hit.point);
            PickEx(hit.point);
        }


    }

    public void FixedUpdate()
    {
        if (Input.GetMouseButtonUp(0))
        {
            RayTest();
        }
    }
    
        public void PickEx(Vector3 pos)
        { 
            var id = GetProvinceID((int) pos.x, (int) pos.z);
            Debug.Log("选择到省份" + id);
            /*
            var Pdata = GetProvinceData(id);
            if (Pdata != null)
            {
                var col = kProvinceMap.GetPixel((int)pos.x, (int)pos.z);
                var tarcol = Color.white;
                if (Pdata.kShowColor.a > 0.5)
                {
                    tarcol = Color.yellow;
                    tarcol.a = 0;
                }
                else
                {
                    tarcol = Color.red;
                    tarcol.a = 1;
                }
                Pdata.kShowColor = tarcol;
                ChangeTargetProvinceColor(col, tarcol);
                /*
                var border_col = Color.blue;
                border_col.a = 1;
                List<Border> borders;
                borderMap.TryGetValue(id, out borders);
                foreach (var border in borders)
                {
                    foreach (var point in border.pointList)
                    {
                        kProvincePolitimap.SetPixel(point.x,point.y,border_col);
                    }
                }
                kProvincePolitimap.Apply();
                
                Graphics.Blit(kProvincePolitimap, kProvinceColorMap);
                }
            */
            
    }


    public void Pickat(Vector3 pos)
    {
        Debug.Log("选择到省份" + GetProvinceID((int)pos.x, (int)pos.z));

        var Pdata = GetProvinceData((int)pos.x, (int)pos.z);
        if(Pdata != null)
        {
            if(Pdata.kShowColor.a > 0.5)
            {
                Pdata.kShowColor = Color.black;
                Pdata.kShowColor.a = 0;
            }
            else
            {
                Pdata.kShowColor = Color.blue;
                Pdata.kShowColor.a = 1;
            }

            ChangeProvinceColor();
        }
    }
    
    public static System.Text.Encoding GetType(string FILE_NAME)
    {
        System.IO.FileStream fs = new System.IO.FileStream(FILE_NAME, System.IO.FileMode.Open,
            System.IO.FileAccess.Read);
        System.Text.Encoding r = GetType(fs);
        fs.Close();
        return r;
    }

    /// 通过给定的文件流，判断文件的编码类型
    /// <param name="fs">文件流</param>
    /// <returns>文件的编码类型</returns>
    public static System.Text.Encoding GetType(System.IO.FileStream fs)
    {
        byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
        byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
        byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM
        System.Text.Encoding reVal = System.Text.Encoding.Default;

        System.IO.BinaryReader r = new System.IO.BinaryReader(fs, System.Text.Encoding.Default);
        int i;
        int.TryParse(fs.Length.ToString(), out i);
        byte[] ss = r.ReadBytes(i);
        if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
        {
            reVal = System.Text.Encoding.UTF8;
        }
        else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
        {
            reVal = System.Text.Encoding.BigEndianUnicode;
        }
        else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
        {
            reVal = System.Text.Encoding.Unicode;
        }
        r.Close();
        return reVal;
    }
    private static bool IsUTF8Bytes(byte[] data)
    {
        int charByteCounter = 1;  //计算当前正分析的字符应还有的字节数
        byte curByte; //当前分析的字节.
        for (int i = 0; i < data.Length; i++)
        {
            curByte = data[i];
            if (charByteCounter == 1)
            {
                if (curByte >= 0x80)
                {
                    //判断当前
                    while (((curByte <<= 1) & 0x80) != 0)
                    {
                        charByteCounter++;
                    }
                    //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X　
                    if (charByteCounter == 1 || charByteCounter > 6)
                    {
                        return false;
                    }
                }
            }
            else
            {
                //若是UTF-8 此时第一位必须为1
                if ((curByte & 0xC0) != 0x80)
                {
                    return false;
                }
                charByteCounter--;
            }
        }
        return true;
    }
    
}
