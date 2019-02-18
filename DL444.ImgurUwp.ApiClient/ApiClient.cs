using System;
using System.Net.Http;
using Newtonsoft.Json;

namespace DL444.ImgurUwp.ApiClient
{
    public class ApiClient
    {
        HttpClient client = new HttpClient();

        public string Host { get; }
        public string ClientId { get; }
        public string ClientSecret { get; }
        public string MashapeKey { get; }
        public string AccessToken { get; }
        public string Username { get; }


        static string GetDataToken(string jsonText)
        {
            Newtonsoft.Json.Linq.JObject obj = Newtonsoft.Json.Linq.JObject.Parse(jsonText);
            if(obj.ContainsKey("data"))
            {
                return (string)obj["data"];
            }
            else
            {
                return null;
            }
        }

        public ApiClient(string host, string clientId, string clientSecret, string username = "", string accessToken = "", string mashapeKey = "")
        {
            Host = host;
            ClientId = clientId;
            ClientSecret = clientSecret;
            MashapeKey = mashapeKey;
            AccessToken = accessToken;
            Username = username;

            client.BaseAddress = new Uri(Host);
        }
    }
}
