using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DL444.ImgurUwp.Models
{
    public class Trophy
    {
        [JsonProperty(PropertyName = "name_clean")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string DisplayName { get; set; }

        public int Id { get; set; }
        public string Description { get; set; }
        public int DateTime { get; set; }

        public string Image { get; set; }

        [JsonProperty(PropertyName = "image_height")]
        public int ImageHeight { get; set; }
        [JsonProperty(PropertyName = "image_width")]
        public int ImageWidth { get; set; }

        // Describes the item where the trophy was earned.
        public string Data { get; set; }

        [JsonProperty(PropertyName = "data_link")]
        public string DataLink { get; set; }
    }
}
