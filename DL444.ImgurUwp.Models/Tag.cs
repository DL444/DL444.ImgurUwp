using Newtonsoft.Json;
using System.Collections.Generic;

namespace DL444.ImgurUwp.Models
{
    public class Tag
    {
        public string Name { get; set; }
        public int Followers { get; set; }
        [JsonProperty(PropertyName = "total_items")]
        public int TotalItems { get; set; }
        public bool Following { get; set; }
        public List<IGalleryItem> Items { get; set; }

        [JsonProperty(PropertyName = "display_name")]
        public string DisplayName { get; set; }
        public string Accent { get; set; }
    }
}
