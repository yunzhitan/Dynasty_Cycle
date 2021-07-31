using System;
using UnityEngine;
using Newtonsoft.Json;

namespace country
{
    public class CountryMeta
    {
        [JsonProperty("name")]
        public String name { get; set; } //国家名称
        
        [JsonProperty("color")]
        public Color32 load_color { get; set; } //国家颜色
        
        [JsonIgnore]
        public Color color { get; set; }
        
        [JsonProperty("tag")]
        public String tag { get; set; } //国家代码

        public CountryMeta()
        {
            tag = " ";
        }

        public void initColor()
        {
            var r = (float) load_color.r / 255f;
            var g = (float) load_color.g / 255f;
            var b = (float) load_color.b / 255f;
            this.color = new Color(r, g, b);

        }

        public CountryMeta(string name, Color32 loadColor, string tag)
        {
            this.name = name;
            this.load_color = loadColor;
            this.tag = tag;
        }

        public CountryMeta(string name, Color32 loadColor)
        {
            this.name = name;
            this.load_color = loadColor;
            tag = " ";
        }

        public override string ToString()
        {
            return $"{nameof(name)}: {name}, {nameof(load_color)}: {load_color}, {nameof(tag)}: {tag}";
        }
    }
    
    
}
