using Newtonsoft.Json;
using System.Collections.Generic;

namespace DL444.ImgurUwp.Models
{
    public class Comment
    {
        public int Id { get; set; }
        [JsonProperty(PropertyName = "image_id")]
        public string ImageId { get; set; }
        [JsonProperty(PropertyName = "comment")]
        public string Content { get; set; }
        public string Author { get; set; }
        public bool OnAlbum { get; set; }
        public string AlbumCover { get; set; }
        public int Ups { get; set; }
        public int Downs { get; set; }
        public int Points { get; set; }
        public int DateTime { get; set; }
        public int ParentId { get; set; }
        public bool Deleted { get; set; }
        public string Vote { get; set; }
        public List<Comment> Children { get; set; }
    }
}
