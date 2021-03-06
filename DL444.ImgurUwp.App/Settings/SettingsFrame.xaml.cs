﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DL444.ImgurUwp.App.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsFrame : Page
    {
        public SettingsFrame()
        {
            this.InitializeComponent();
        }

        List<SettingsPageModel> Pages { get; } = new List<SettingsPageModel>()
        {
            new SettingsPageModel() { Title = "Personalize", Description = "Avatar, profile cover", ContentPageType = typeof(PersonalizeSettingsPage) },
            new SettingsPageModel() { Title = "General", Description = "Options, appearance", ContentPageType = typeof(GeneralSettingsPage) },
            new SettingsPageModel() { Title = "Account", Description = "Privacy, notifications, content", ContentPageType = typeof(AccountSettingsPage) },
        };

        private void PageList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var model = (sender as ListView).SelectedItem as SettingsPageModel;
            if(model.ContentPageType != null)
            {
                ContentFrame.Navigate(model.ContentPageType);
            }
        }
    }
}
