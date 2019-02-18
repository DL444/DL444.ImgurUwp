using System;

namespace DL444.ImgurUwp.Models
{
    public class UserToken
    {
        public string Username { get; set; }
        public string UserId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        /// <summary>
        /// The expire time of RefreshToken
        /// </summary>
        public DateTime ExpireTime { get; set; }
    }
}
