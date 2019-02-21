using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DL444.ImgurUwp.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Bio { get; set; }
        public double Reputation { get; set; }
        [JsonProperty(PropertyName = "created")]
        public int CreatedTime { get; set; }

        [JsonProperty(PropertyName = "reputation_name")]
        public string ReputationName { get; set; }
        [JsonProperty(PropertyName = "is_blocked")]
        public bool IsBlocked { get; set; }
        public string Avatar { get; set; }
        [JsonProperty(PropertyName = "avatar_name")]
        public string AvatarName { get; set; }
        public string Cover { get; set; }
        [JsonProperty(PropertyName = "cover_name")]
        public string CoverName { get; set; }

        public bool UserFollow { get; set; }
        // This field is nested like this: "user_follow": { "status": False }
    }
}
