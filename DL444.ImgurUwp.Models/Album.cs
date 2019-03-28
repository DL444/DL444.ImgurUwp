using Newtonsoft.Json;
using System.Collections.Generic;

namespace DL444.ImgurUwp.Models
{
    public class Album : IItem
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int DateTime { get; set; }
        public string Cover { get; set; }
        [JsonProperty(PropertyName = "cover_width")]
        public int? CoverWidth { get; set; }
        [JsonProperty(PropertyName = "cover_height")]
        public int? CoverHeight { get; set; }
        [JsonProperty(PropertyName = "account_url")]
        public string AccountUrl { get; set; }
        [JsonProperty(PropertyName = "account_id")]
        public int AccountId { get; set; }
        public string Privacy { get; set; }
        public string Layout { get; set; }
        public int Views { get; set; }
        public string Link { get; set; }
        public bool Favorite { get; set; }
        public bool? Nsfw { get; set; }
        public string Section { get; set; }
        public int Order { get; set; }
        public string DeleteHash { get; set; }
        [JsonProperty(PropertyName = "images_count")]
        public int ImageCount { get; set; }
        public List<Image> Images { get; set; }
        [JsonProperty(PropertyName = "in_gallery")]
        public bool InGallery { get; set; }
        [JsonProperty(PropertyName = "is_album")]
        public bool? IsAlbum { get; set; }

        public int? Points { get; set; }
    }
}
