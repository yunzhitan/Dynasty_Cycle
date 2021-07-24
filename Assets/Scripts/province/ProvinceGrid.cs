using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using province;
using Color = UnityEngine.Color;
using Debug = UnityEngine.Debug;

[ExecuteInEditMode]
public class ProvinceGrid : MonoBehaviour {

    public Texture2D kProvinceMap;
    public RenderTexture kProvinceColorMap;

    public int kProvinceCount;
    public Material kLut;

    private Dictionary<Color, int> kProvinceColorToIDMap = new Dictionary<Color, int>();
    private Dictionary<int, Color> kProvinceIdtoColorMap = new Dictionary<int, Color>();
    private Dictionary<BorderPair,HashSet<Point>> borderPointMap = new Dictionary<BorderPair,HashSet<Point>>();
    private Dictionary<int, List<Border>> borderMap = new Dictionary<int, List<Border>>();

    private ProvinceMeta[] kProvinceMeta;

    private Texture2D kProvincePolitimap;

    public enum Type
    {

    }

    private void OnEnable()
    {
        //searDefine();
    }

    void Awake()
    {
        search();
        InitBorder();
    }

    private void InitBorder()
    {
        StreamReader reader = new StreamReader(Application.streamingAssetsPath
                                               + "/province/border.txt");
        while (reader.Peek() >= 0)
        {
            var s = reader.ReadLine().Split(new []{':'},StringSplitOptions.RemoveEmptyEntries);
            if(s.Length == 0) continue;
            var id_pair = s[0].Split(new []{','},StringSplitOptions.RemoveEmptyEntries);
            if (id_pair.Length == 0) continue;
            var pa = Convert.ToInt32(id_pair[0]);
            var pb = Convert.ToInt32(id_pair[1]);
            BorderPair pair = new BorderPair(pa, pb);
            Border border = new Border(pair,new List<Point>());
            var point_str_list = s[1].Split(new char[]{' '},StringSplitOptions.RemoveEmptyEntries);
            foreach (var point_str in point_str_list)
            {
                var point_str_temp = point_str.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                if(point_str_temp.Length == 0) continue;
                var x = Convert.ToInt32(point_str_temp[0]);
                var z = Convert.ToInt32(point_str_temp[1]);
                var point = new Point(x, z);
                border.pointList.Add(point);
            }

            if (borderMap.ContainsKey(pa))
            {
                borderMap[pa].Add(border);
            }
            else
            {
                var borderList = new List<Border>();
                borderList.Add(border);
                borderMap.Add(pa,borderList);
            }
            if (borderMap.ContainsKey(pb))
            {
                borderMap[pb].Add(border);
            }
            else
            {
                var borderList = new List<Border>();
                borderList.Add(border);
                borderMap.Add(pb,borderList);
            }
        }
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
            kProvinceIdtoColorMap.TryGetValue(id, out col);
            kProvinceColorToIDMap.TryGetValue(col, out flag);
            Debug.Assert(flag == id);
            kProvinceMeta[id] = new ProvinceMeta(id,col);
        }
        
        var ttt = kProvinceMap.GetPixel(0, 0);
        Debug.Log("getPixel： " + ttt);
        
        fileStream.Close();
        kProvincePolitimap = new Texture2D(ConfigParam.BLOCKMAXWIDTH, ConfigParam.BLOCKMAXHEIGHT);
        Graphics.ConvertTexture(kProvinceMap, kProvincePolitimap);
        kProvinceCount = count;
        //DebugProvinceCol();
        ChangeProvinceColor();
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

                if (!kProvinceIdtoColorMap.ContainsKey(id))
                {
                    kProvinceIdtoColorMap.Add(id, color);
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
            
            var Pdata = GetProvinceData(id);
            if (Pdata != null)
            {
                var col = kProvinceMap.GetPixel((int)pos.x, (int)pos.z);
                var tarcol = Color.white;
                if (Pdata.kShowColor.a > 0.5)
                {
                    tarcol = Color.black;
                    tarcol.a = 0;
                }
                else
                {
                    tarcol = Color.red;
                    tarcol.a = 1;
                }
                Pdata.kShowColor = tarcol;
                ChangeTargetProvinceColor(col, tarcol);

                var border_col = Color.blue;
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
    
    private void AddBorderPoint(Color ca, Color cb, int i, int j)
    {
        int pa = 0, pb = 0;
        kProvinceColorToIDMap.TryGetValue(ca, out pa);
        kProvinceColorToIDMap.TryGetValue(cb, out pb);
        BorderPair pair = new BorderPair(pa, pb);

        if (borderPointMap.ContainsKey(pair))
        {
            borderPointMap[pair].Add(new Point(i, j));
        }
        else
        {
            var pointSet = new HashSet<Point>();
            pointSet.Add(new Point(i, j));
            borderPointMap.Add(pair, pointSet);
        }
    }

    void SearchBorder()
    {
        var width = kProvinceMap.width;
        var height = kProvinceMap.height;
        LC_Helper.Loop(width, (i) =>
        {
            LC_Helper.Loop(height, (j) =>
            {
                var col = kProvinceMap.GetPixel(i, j);
                
                if (i > 0 && i < width - 1 && j > 0 && j < height - 1)
                {
                    var col_left = kProvinceMap.GetPixel(i - 1, j);
                    var col_right = kProvinceMap.GetPixel(i + 1, j);
                    var col_low = kProvinceMap.GetPixel(i, j - 1);
                    var col_high = kProvinceMap.GetPixel(i, j + 1);
                    if(col_left != col) AddBorderPoint(col,col_left,i,j);
                    if(col_right != col) AddBorderPoint(col,col_right,i,j);
                    if(col_low != col) AddBorderPoint(col,col_low,i,j);
                    if(col_high != col) AddBorderPoint(col,col_high,i,j);
                } 
                else if (i == 0)
                {
                    if (j == 0 || j == height - 1) ;
                    else
                    {
                        var col_right = kProvinceMap.GetPixel(i + 1, j);
                        var col_low = kProvinceMap.GetPixel(i, j - 1);
                        var col_high = kProvinceMap.GetPixel(i, j + 1);
                        if (col_right != col) AddBorderPoint(col, col_right, i, j);
                        if (col_low != col) AddBorderPoint(col, col_low, i, j);
                        if (col_high != col) AddBorderPoint(col, col_high, i, j);
                    }
                }
                else if (i == width - 1)
                {
                    if (j == 0 || j == height - 1) ;
                    else
                    {
                        var col_left = kProvinceMap.GetPixel(i - 1, j);
                        var col_low = kProvinceMap.GetPixel(i, j - 1);
                        var col_high = kProvinceMap.GetPixel(i, j + 1);
                        if (col_left != col) AddBorderPoint(col, col_left, i, j);
                        if (col_low != col) AddBorderPoint(col, col_low, i, j);
                        if (col_high != col) AddBorderPoint(col, col_high, i, j);

                    }
                }
                else if (j == 0)
                {
                    if (i == 0 || i == width - 1) ;
                    else
                    {
                        var col_right = kProvinceMap.GetPixel(i + 1, j);
                        var col_left = kProvinceMap.GetPixel(i-1, j );
                        var col_high = kProvinceMap.GetPixel(i, j + 1);
                        if (col_right != col) AddBorderPoint(col, col_right, i, j);
                        if (col_left != col) AddBorderPoint(col, col_left, i, j);
                        if (col_high != col) AddBorderPoint(col, col_high, i, j);
                    }
                }
                else if (j == height - 1)
                {
                    if (i == 0 || i == width - 1) ;
                    else
                    {
                        var col_left = kProvinceMap.GetPixel(i - 1, j);
                        var col_low = kProvinceMap.GetPixel(i, j - 1);
                        var col_right = kProvinceMap.GetPixel(i + 1, j);
                        if (col_left != col) AddBorderPoint(col, col_left, i, j);
                        if (col_low != col) AddBorderPoint(col, col_low, i, j);
                        if (col_right != col) AddBorderPoint(col, col_right, i, j);
                    }
                }
                
            });

        });
        var dict = new Dictionary<BorderTeck,int>();

        StreamWriter writer = new StreamWriter(Application.streamingAssetsPath
         + "/province/border.txt");
        StringBuilder builder = new StringBuilder();
        foreach (var pair in borderPointMap)
        {
            var id = pair.Key;
            var teck = new BorderTeck(id.provinceA,id.provinceB);
            if (dict.ContainsKey(teck))
                continue;
            dict.Add(teck,1);
            var pointList = pair.Value;

            builder.Append(String.Format("{0}:", teck));
            foreach (var point in pointList)
            {
                builder.Append(point);
            }

            builder.Append("\n\n");
        }
        writer.Write(builder.ToString());
        writer.Close();
    }

}
