using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using PoopmailGui.Api;
using RestSharp;

namespace PoopmailGui
{
    public class ListMailboxWindow : Window
    {
        private ApiClient _apiClient;
        private string _refreshToken;
        private string _accessToken;

        public ListMailboxWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        public ListMailboxWindow(ApiClient apiClient, string refreshToken, string accessToken)
        {
            this._apiClient = apiClient;
            this._refreshToken = refreshToken;
            this._accessToken = accessToken;
            
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            var listBox = this.FindControl<ListBox>("ListMailboxes");
            listBox.SelectionMode = SelectionMode.Single;
            
            Refresh();
        }

        public void Refresh()
        {
            var mailboxes = _apiClient.GetMailboxes(_refreshToken, _accessToken);
            if (mailboxes.Obj == null)
                return;      
            if (mailboxes.Obj.Data == null)
                return;
            
            var listBox = this.FindControl<ListBox>("ListMailboxes");
            var list = mailboxes.Obj.Data.Select(mailbox => MakeText(mailbox.Address)).ToList();
            listBox.Items = list;
        }

        private void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            var window = new CreateMailboxWindow(this, _apiClient, _refreshToken, _accessToken);
            window.Show();
        }

        private ListBoxItem MakeText(string str)
        {
            var item = new ListBoxItem();
            var box = new TextBlock();
            box.Text = str;
            item.Content = box;
            return item;
        }

        private void ButtonOpen_OnClick(object? sender, RoutedEventArgs e)
        {
            var listBox = this.FindControl<ListBox>("ListMailboxes");
            var selectedItems = listBox.SelectedItems;
            if (selectedItems.Count != 1)
            {
                return;
            }
            
            var item = (ListBoxItem) selectedItems[0];
            var content = item.Content;
            if (!(content is TextBlock))
            {
                return;
            }

            string txt = ((TextBlock) content).Text;
            var window = new ViewMailboxWindow(txt, _apiClient, _refreshToken, _accessToken);
            window.Show();
        }
    }
}