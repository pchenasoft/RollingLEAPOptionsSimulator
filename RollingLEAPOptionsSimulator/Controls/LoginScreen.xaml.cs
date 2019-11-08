// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginScreen.xaml.cs" company="KriaSoft LLC">
//   Copyright © 2013 Konstantin Tarkus, KriaSoft LLC. See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace RollingLEAPOptionsSimulator.Controls
{
    using RollingLEAPOptionsSimulator;
    using System.Windows;
    using System.Windows.Input;

    using RollingLEAPOptionsSimulator.Utility;
    using System;

    /// <summary>
    /// Interaction logic for LoginScreen.xaml
    /// </summary>
    public partial class LoginScreen : Window
    {
        private readonly AmeritradeClient client;

        public LoginScreen()
        {
            this.InitializeComponent();

            this.AppKey.Text = Settings.GetProtected(Settings.AppKeyKey);
            this.RefreshToken.Text = Settings.GetProtected(Settings.RefreshTokenKey);        
            this.RememberUserName.IsChecked = Settings.Get(Settings.RememberKey, defaultValue: true);
            this.ErrorMessage.Visibility = Visibility.Collapsed;

            if (!string.IsNullOrWhiteSpace(this.AppKey.Text))
            {
                this.RefreshToken.Focus();
            }

        }

        public LoginScreen(AmeritradeClient client)
            : this()
        {
            this.client = client;          
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.AppKey.Text) || string.IsNullOrWhiteSpace(this.RefreshToken.Text))
            {
                this.ErrorMessage.Visibility = Visibility.Visible;
                return;
            }

            Settings.SetProtected(Settings.AppKeyKey, this.RememberUserName.IsChecked.Value ? this.AppKey.Text : string.Empty);
            Settings.SetProtected(Settings.RefreshTokenKey, this.RememberUserName.IsChecked.Value ? this.RefreshToken.Text : string.Empty);           
            Settings.Set(Settings.RememberKey, this.RememberUserName.IsChecked.Value);

            this.RefreshToken.IsEnabled = false;
            this.AppKey.IsEnabled = false;
            this.LoginButton.IsEnabled = false;

            var result = await this.client.LogIn(this.AppKey.Text, this.RefreshToken.Text);

            if (!result)
            {
                this.AppKey.IsEnabled = true;
                this.RefreshToken.IsEnabled = true;           
                this.LoginButton.IsEnabled = true;
                this.ErrorMessage.Visibility = Visibility.Visible;
                return;
            }

            this.DialogResult = result;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }       
    }
}
