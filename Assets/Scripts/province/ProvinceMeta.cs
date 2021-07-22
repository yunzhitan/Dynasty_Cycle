using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class ProvinceMeta{

    [JsonProperty("id")]
    public int id { get; set; }
    [JsonProperty("color")]
    public Color kMapColor { get; set; }
    public Color kShowColor;

    public ProvinceMeta(int id, Color mapColor)
    {
        id = id;
        kMapColor = mapColor;
        kShowColor = mapColor;
        kShowColor.a = 0;
    }
}
