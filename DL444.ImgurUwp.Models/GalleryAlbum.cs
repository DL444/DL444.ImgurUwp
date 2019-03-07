using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DL444.ImgurUwp.Models
{
    public class GalleryAlbum : IGalleryItem
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int DateTime { get; set; }

        public string Cover { get; set; }
        [JsonProperty(PropertyName = "cover_width")]
        public int CoverWidth { get; set; }
        [JsonProperty(PropertyName = "cover_height")]
        public int CoverHeight { get; set; }

        [JsonProperty(PropertyName = "account_url")]
        public string AccountUrl { get; set; }
        [JsonProperty(PropertyName = "account_id")]
        public string AccountId { get; set; }

        public string Privacy { get; set; }
        public string Layout { get; set; }
        public int Views { get; set; }
        public string Link { get; set; }

        public int Ups { get; set; }
        public int Downs { get; set; }
        public int Points { get; set; }
        public int? Score { get; set; }
        [JsonProperty(PropertyName = "is_album")]
        public bool IsAlbum { get; set; }

        public string Vote { get; set; }
        public bool Favorite { get; set; }
        public bool? Nsfw { get; set; }
        [JsonProperty(PropertyName = "comment_count")]
        public int CommentCount { get; set; }
        public string Topic { get; set; }
        [JsonProperty(PropertyName = "topic_id")]
        public int? TopicId { get; set; }
        [JsonProperty(PropertyName = "images_count")]
        public int ImageCount { get; set; }
        public List<Image> Images { get; set; }
        [JsonProperty(PropertyName = "in_most_viral")]
        public bool InMostViral { get; set; }
        [JsonProperty(PropertyName = "in_gallery")]
        public bool InGallery { get; set; }
        public List<Tag> Tags { get; set; }
    }
}
