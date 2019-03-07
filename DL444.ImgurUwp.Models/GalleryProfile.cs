using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DL444.ImgurUwp.Models
{
    public class GalleryProfile
    {
        [JsonProperty(PropertyName = "total_gallery_comments")]
        public int GalleryCommentCount { get; set; }

        [JsonProperty(PropertyName = "total_gallery_favorites")]
        public int GalleryFavoriteCount { get; set; }

        [JsonProperty(PropertyName = "total_gallery_submissions")]
        public int GallerySubmissionCount { get; set; }

        public List<Trophy> Trophies { get; set; }
    }
}
