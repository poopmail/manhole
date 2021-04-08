using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PoopmailGui.Api;

namespace PoopmailGui
{
    public class ViewMailboxWindow : Window
    {
        private readonly string _accessToken;
        private readonly ApiClient _apiClient;
        private readonly string _mailbox;
        private readonly string _refreshToken;
        private readonly Dictionary<string, string> mailDict = new();

        public ViewMailboxWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        public ViewMailboxWindow(string mailbox, ApiClient apiClient, string refreshToken, string accessToken)
        {
            _mailbox = mailbox;
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

            this.Title = "Mailbox <" + _mailbox + ">";

            var listBox = this.FindControl<ListBox>("ListMails");
            var list = new List<ListBoxItem>();
            var messages = _apiClient.GetMessages(_refreshToken, _accessToken, _mailbox);
            if (messages != null && messages.Obj != null && messages.Obj.Data != null)
                foreach (var message in messages.Obj.Data)
                {
                    string content = message.Content.Plain.Equals("") ? message.Content.Html : message.Content.Plain;
                    
                    mailDict.Add(message.Subject, content + "\n\n---\n\nBy " + message.From);
                    list.Add(MakeText(message.Subject));
                }

            listBox.Items = list;
        }

        private void ListMails_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            var listBox = this.FindControl<ListBox>("ListMails");
            var selectedItems = listBox.SelectedItems;
            if (selectedItems.Count != 1) return;

            var item = (ListBoxItem) selectedItems[0];
            var content = item.Content;
            if (!(content is TextBlock)) return;

            string txt = ((TextBlock) content).Text;
            var block = this.FindControl<TextBlock>("TextMail");
            block.Text = mailDict.GetValueOrDefault(txt, "?");
        }

        private ListBoxItem MakeText(string str)
        {
            var item = new ListBoxItem();
            var box = new TextBlock();
            box.Text = str;
            item.Content = box;
            return item;
        }
    }
}