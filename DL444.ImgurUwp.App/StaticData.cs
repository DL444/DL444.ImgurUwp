using Newtonsoft.Json;
using System.IO;

namespace DL444.ImgurUwp.App
{
    static class ApiKey
    {
        public static string ClientId { get; }
        public static string ClientSecret { get; }
        public static string MashapeKey { get; }

        static ApiKey()
        {
            using (StreamReader reader = new StreamReader("Assets/ApiKey.json"))
            {
                string json = reader.ReadToEnd();
                dynamic jsonObj = JsonConvert.DeserializeObject(json);
                ClientId = jsonObj.clientId.ToString();
                ClientSecret = jsonObj.clientSecret.ToString();
                MashapeKey = jsonObj.mashapeKey.ToString();
            }
        }
    }
}
