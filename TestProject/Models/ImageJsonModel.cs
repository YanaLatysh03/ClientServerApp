using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestProject.Models
{
    public class ImageJsonModel
    {
        [JsonProperty("Id")]
        public Guid Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Path")]
        public string Path { get; set; }

        public ImageJsonModel()
        {
        }

        public ImageJsonModel(string name, string path, Guid id)
        {
            Name = name;
            Path = path;
            Id = id;
        }
    }
}
