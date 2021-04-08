using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using PoopmailGui.Api;

namespace PoopmailGui
{
    public class CreateMailboxWindow : Window
    {

        private ListMailboxWindow _parentWindow;
        private ApiClient _apiClient;
        private string _refreshToken;
        private string _accessToken;
        
        public CreateMailboxWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        public CreateMailboxWindow(ListMailboxWindow parentWindow, ApiClient apiClient, string refreshToken, string accessToken)
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
        }

        private void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            var box = this.FindControl<TextBox>("BoxAddress");
            var result = _apiClient.CreateMailbox(_refreshToken, _accessToken, box.Text);
            if (result.Obj == null)
            {
                box.Text = result.Response.Response;
                return;
            }
            
            _parentWindow.Refresh();
            Close();
        }
    }
}