using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static DL444.ImgurUwp.ApiClient.DisplayParams;
using DL444.ImgurUwp.Models;

namespace DL444.ImgurUwp.ApiClient
{
    public partial class ApiClient
    {
        HttpClient client = new HttpClient();

        public string Host { get; }
        public string ClientId { get; }
        public string ClientSecret { get; }
        public string MashapeKey { get; }
        public string AccessToken { get; }
        public string Username { get; }

        static (bool success, int status, string dataToken) GetDataToken(string jsonText)
        {
            Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(jsonText);
            int statusCode = obj["status"].ToObject<int>();

            if ((bool)obj["success"])
            {
                return (true, statusCode, obj["data"].ToString());
            }
            else
            {
                return (false, statusCode, obj["data"].ToString());
            }
        }

        public ApiClient(string host, string clientId, string clientSecret, string accessToken, string username = "", string mashapeKey = "")
        {
            if(string.IsNullOrEmpty(host))
            {
                throw new ArgumentNullException(nameof(host));
            }
            if(string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentNullException(nameof(clientId));
            }
            if(string.IsNullOrEmpty(clientSecret))
            {
                throw new ArgumentNullException(nameof(clientSecret));
            }
            if(string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            Host = host;
            ClientId = clientId;
            ClientSecret = clientSecret;
            MashapeKey = mashapeKey;
            AccessToken = accessToken;
            Username = username;

            client.BaseAddress = new Uri($"https://{host}");
            if(!string.IsNullOrEmpty(mashapeKey))
            {
                client.DefaultRequestHeaders.Add("X-RapidAPI-Key", mashapeKey);
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            }
        }
    }

    public class ApiRequestException : Exception
    {
        public int Status { get; set; }

        public ApiRequestException() : base() { }

        public ApiRequestException(string message) : base(message) { }

        public ApiRequestException(string message, Exception innerException) : base(message, innerException) { }
    }

    static class Convert
    {
        static DateTime baseTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static DateTime ToDateTime(int epoch)
        {
            return baseTime.AddSeconds(epoch);
        }
    }
}
