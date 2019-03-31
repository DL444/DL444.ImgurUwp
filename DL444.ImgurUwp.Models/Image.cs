using Newtonsoft.Json;

namespace DL444.ImgurUwp.Models
{
    public class Image : IItem
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int DateTime { get; set; }
        public string Type { get; set; }
        public bool Animated { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Size { get; set; }
        public int Views { get; set; }
        public long Bandwidth { get; set; }
        public string DeleteHash { get; set; }
        public string Name { get; set; }
        public string Section { get; set; }
        public string Link { get; set; }
        public string Gifv { get; set; }
        public string Mp4 { get; set; }
        [JsonProperty(PropertyName = "mp4_size")]
        public int Mp4Size { get; set; }
        public bool Looping { get; set; }
        public bool? Favorite { get; set; }
        public bool? Nsfw { get; set; }
        public string Vote { get; set; }
        [JsonProperty(PropertyName = "in_gallery")]
        public bool InGallery { get; set; }
        [JsonProperty(PropertyName = "is_album")]
        public bool? IsAlbum { get; set; }
        [JsonProperty(PropertyName = "account_url")]
        public string AccountUrl { get; set; }

        public int? Points { get; set; }
    }
}
