using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using PoopmailGui.Api;

namespace PoopmailGui
{
    public class CreateMailboxWindow : Window
    {
        private readonly string _accessToken;
        private readonly ApiClient _apiClient;
        private readonly string _refreshToken;

        private ListMailboxWindow _parentWindow;

        public CreateMailboxWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        public CreateMailboxWindow(ListMailboxWindow parentWindow, ApiClient apiClient, string refreshToken,
            string accessToken)
        {
            _parentWindow = parentWindow;
            _apiClient = apiClient;
            _refreshToken = refreshToken;
            _accessToken = accessToken;

            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            var comboBox = this.FindControl<ComboBox>("ComboDomains");
            List<string> list = _apiClient.GetDomains(_refreshToken, _accessToken).Obj;
            comboBox.Items = list;

            if (list.Count > 0)
                comboBox.SelectedIndex = 0;
        }

        private void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            var comboBox = this.FindControl<ComboBox>("ComboDomains");
            if (comboBox.SelectedItem == null)
                return;
            string domain = (string) comboBox.SelectedItem;
            var box = this.FindControl<TextBox>("BoxAddress");
            var result = _apiClient.CreateMailbox(_refreshToken, _accessToken, box.Text, domain);
            if (result.Obj == null)
            {
                box.Text = result.Response.Response;
                return;
            }

            //_parentWindow.Refresh();
            Close();
        }
    }
}