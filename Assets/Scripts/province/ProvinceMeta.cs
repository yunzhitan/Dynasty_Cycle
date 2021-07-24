using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class ProvinceMeta{

    [JsonProperty("id")]
    public int Id { get; set; }
    [JsonProperty("color")]
    public Color kMapColor { get; set; }
    public Color kShowColor;

    public ProvinceMeta(int id, Color mapColor)
    {
        Id = id;
        kMapColor = mapColor;
        kShowColor = mapColor;
        kShowColor.a = 0;
    }
}
