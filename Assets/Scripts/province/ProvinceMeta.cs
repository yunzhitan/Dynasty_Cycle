using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class ProvinceMeta{

    [JsonProperty("id")]
    public int Id { get; set; }
    [JsonIgnore]
    public Color kMapColor { get; set; }
    [JsonProperty("owner")]
    public String owner { get; set; }
    [JsonProperty("name")]
    public String name { get; set; }
    [JsonProperty("color")]
    public Color32 kLoadColor { get; set; }
    [JsonIgnore]
    public Color kShowColor;

    public ProvinceMeta(int id, Color mapColor)
    {
        Id = id;
        kMapColor = mapColor;
        kShowColor = mapColor;
        kShowColor.a = 0;
    }

    public ProvinceMeta()
    {
    }

    public ProvinceMeta(int id, Color mapColor, string owner)
    {
        Id = id;
        kMapColor = mapColor;
        this.owner = owner;
    }

    public void InitColor()
    {
        var r = (float) kLoadColor.r / 255f;
        var g = (float) kLoadColor.g / 255f;
        var b = (float) kLoadColor.b / 255f;
        this.kMapColor = new Color(r, g, b);
    }
}
