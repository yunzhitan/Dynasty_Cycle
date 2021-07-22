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
        public Color32 color { get; set; } //国家颜色

        public CountryMeta()
        {

        }

        public CountryMeta(string name, Color32 color)
        {
            this.name = name;
            this.color = color;
        }
        
        public override String ToString()
        {
            return name +" " + color.ToString();
        }
    }
    
    
}
