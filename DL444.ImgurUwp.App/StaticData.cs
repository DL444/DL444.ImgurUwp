using Newtonsoft.Json;
using System;
using System.IO;
using Windows.UI.Xaml.Controls;

namespace DL444.ImgurUwp.App
{
    static class ApiKey
    {
        public static string ClientId { get; }
        public static string ClientSecret { get; }
        public static string MashapeKey { get; }
        public static string Host { get; }

        static ApiKey()
        {
            using (StreamReader reader = new StreamReader("Assets/ApiKey.json"))
            {
                string json = reader.ReadToEnd();
                dynamic jsonObj = JsonConvert.DeserializeObject(json);
                ClientId = jsonObj.clientId.ToString();
                ClientSecret = jsonObj.clientSecret.ToString();
                MashapeKey = jsonObj.mashapeKey.ToString();
                Host = jsonObj.host.ToString();
            }
        }
    }

    static class ApiClient
    {
        private static string _ownerAccount;

        public static string OwnerAccount
        {
            get => _ownerAccount;
            set
            {
                if(_ownerAccount == null)
                {
                    _ownerAccount = value;
                }
            }
        }
        public static ImgurUwp.ApiClient.ApiClient Client { get; private set; }
        public static void InitializeApiClient(string accessToken)
        {
            Client = new ImgurUwp.ApiClient.ApiClient(ApiKey.Host,
                ApiKey.ClientId, ApiKey.ClientSecret, accessToken, ApiKey.MashapeKey);
            _ownerAccount = null;
        }
    }

    static class Navigation
    {
        public static Frame ContentFrame { get; private set; }

        public static void Navigate(Type pageType, object parameter)
        {
            ContentFrame.Navigate(pageType, parameter);
        }

        public static void InitializeNavigationHelper(Frame contentFrame)
        {
            ContentFrame = contentFrame;
        }
    }
}
