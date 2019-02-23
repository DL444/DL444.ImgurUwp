using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using DL444.ImgurUwp.Models;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DL444.ImgurUwp.App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }

        private async void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            UserToken userToken;
            try
            {
                userToken = await OAuthClient.Authenticate();
            }
            catch (OAuthFailedException)
            {
                return;
            }

            var credentialVault = new Windows.Security.Credentials.PasswordVault();
            credentialVault.Add(new Windows.Security.Credentials.PasswordCredential("Imgur", "Username", userToken.Username));
            credentialVault.Add(new Windows.Security.Credentials.PasswordCredential("Imgur", "UserId", userToken.UserId));
            credentialVault.Add(new Windows.Security.Credentials.PasswordCredential("Imgur", "AccessToken", userToken.AccessToken));
            credentialVault.Add(new Windows.Security.Credentials.PasswordCredential("Imgur", "RefreshToken", userToken.RefreshToken));
            ApiClient.InitializeApiClient(userToken.AccessToken);
            (Window.Current.Content as Frame).Navigate(typeof(MainPage));
        }
    }
}
