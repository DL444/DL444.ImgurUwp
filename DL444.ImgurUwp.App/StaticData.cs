﻿using Newtonsoft.Json;
using System;
using System.IO;
using Windows.UI.Xaml.Controls;
using UI = Microsoft.UI.Xaml.Controls;

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
        public static ImgurUwp.ApiClient.ApiClient Client { get; private set; }
        public static void InitializeApiClient(string accessToken)
        {
            Client = new ImgurUwp.ApiClient.ApiClient(ApiKey.Host,
                ApiKey.ClientId, ApiKey.ClientSecret, accessToken, ApiKey.MashapeKey);
        }
    }

    static class Navigation
    {
        public static Frame ContentFrame { get; private set; }

        public static void Navigate(Type pageType, object parameter/*, bool clearNavView = true*/)
        {
            ContentFrame.Navigate(pageType, parameter);
        }

        public static void InitializeNavigationHelper(Frame contentFrame)
        {
            ContentFrame = contentFrame;
        }
    }
}
