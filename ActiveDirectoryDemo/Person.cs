using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ActiveDirectoryDemo
{
    class Person : ElcomEntry
    {
        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [JsonProperty("shortNameEng")]
        public string Name { get; set; }

        [JsonProperty("department")]
        public string Department { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("mobile")]
        public string MobilePhone { get; set; }

        [JsonProperty("jobTitleRus")]
        public string JobTitle { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }
    }
}
