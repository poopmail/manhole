using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using PoopmailGui.Api;

namespace PoopmailGui
{
    public class LoginWindow : Window
    {
        private ApiClient _apiClient;

        public LoginWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void ButtonLogin_OnClick(object? sender, RoutedEventArgs e)
        {
            var block = this.FindControl<TextBlock>("BlockError");
            var toggle = this.FindControl<ToggleSwitch>("ToggleStaging");
            var user = this.FindControl<TextBox>("BoxLoginName").Text;
            var pass = this.FindControl<TextBox>("BoxLoginPass").Text;

            _apiClient = new ApiClient(toggle.IsChecked.GetValueOrDefault(false));

            var refreshTokenResult = _apiClient.GetRefreshToken(user, pass);
            if (refreshTokenResult.Obj == null)
            {
                block.Text = "Failed to get refresh token: " + refreshTokenResult.Response.StatusCode + ": " +
                             refreshTokenResult.Response.Response;
                return;
            }
            
            var accessTokenResult = _apiClient.GetAccessToken(refreshTokenResult.Obj);
            if (accessTokenResult.Obj == null)
            {
                block.Text = "Failed to get access token: " + accessTokenResult.Response.StatusCode + ": " +
                             accessTokenResult.Response.Response;
                return;
            }

            var window = new ListMailboxWindow(_apiClient, refreshTokenResult.Obj, accessTokenResult.Obj.Token);
            window.Show();

            if (Application.Current.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) return;
            desktop.MainWindow.Close();
            desktop.MainWindow = window;
        }
    }
}