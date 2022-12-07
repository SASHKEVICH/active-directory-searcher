using Newtonsoft.Json;
using System;
using System.Collections.Generic;
namespace ActiveDirectoryDemo
{
    class Unit : ElcomEntry
    {
        [JsonProperty("unitName")]
        public string Name { get; set; }

        [JsonProperty("entries")]
        public List<ElcomEntry> Entries { get; set; }

        public Unit()
        {
            Entries = new List<ElcomEntry>();
        }
    }
}
