using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using PoopmailGui.Api;
using TextCopy;

namespace PoopmailGui
{
    public class ListMailboxWindow : Window
    {
        private readonly string _accessToken;
        private readonly ApiClient _apiClient;
        private readonly string _refreshToken;

        public ListMailboxWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        public ListMailboxWindow(ApiClient apiClient, string refreshToken, string accessToken)
        {
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

            var listBox = this.FindControl<ListBox>("ListMailboxes");
            listBox.SelectionMode = SelectionMode.AlwaysSelected | SelectionMode.Single | SelectionMode.Toggle;

            var contextMenu = new ContextMenu {Width = 150, Height = 100};
            var menuItems = new List<Control>();
            menuItems.Add(MakeItem("Open", (sender, args) => { ButtonOpen_OnClick(null, null); }));
            menuItems.Add(MakeItem("Delete", (sender, args) => { HandleDelete(); }));
            menuItems.Add(MakeItem("Copy address", (sender, args) =>
            {
                var listBox = this.FindControl<ListBox>("ListMailboxes");
                var selectedItems = listBox.SelectedItems;
                if (selectedItems.Count != 1) return;

                var item = (ListBoxItem) selectedItems[0];
                var content = item.Content;
                if (!(content is TextBlock)) return;

                string address = ((TextBlock) content).Text;
                try
                {
                    ClipboardService.SetText(address);
                }
                catch (Exception e)
                {
                    new ErrorDialog(600, 200, "Failed to set clipboard (Maybe install xsel?):\n\n" + e.Message).Show();
                }
            }));
            contextMenu.Items = menuItems;
            listBox.ContextMenu = contextMenu;

            Refresh();
        }

        private void HandleDelete()
        {
            var listBox = this.FindControl<ListBox>("ListMailboxes");
            var selectedItems = listBox.SelectedItems;
            if (selectedItems.Count != 1) return;

            var item = (ListBoxItem) selectedItems[0];
            var content = item.Content;
            if (!(content is TextBlock)) return;

            string address = ((TextBlock) content).Text;
            if(address.Contains(" ") || !address.Contains("@"))
                return;
            
            var result = _apiClient.DeleteMailbox(_refreshToken, _accessToken, address);
            if ((int) result.Obj >= 200 && (int) result.Obj < 300)
                ((TextBlock) content).Text = "Deleted";
            else
                ((TextBlock) content).Text = "Failed to delete: " + result.Obj + ": " + result.Response.Response;
        }

        public void Refresh()
        {
            var mailboxes = _apiClient.GetMailboxes(_refreshToken, _accessToken);
            if (mailboxes.Obj == null)
                return;
            if (mailboxes.Obj.Data == null)
                return;

            var list = mailboxes.Obj.Data
                .Select(mailbox => MakeText(mailbox.Address, (o, o1) =>
                    ButtonOpen_OnClick(null, null))).ToList();
            var listBox = this.FindControl<ListBox>("ListMailboxes");
            listBox.Items = list;
        }

        private void Button_OnClick(object? sender, RoutedEventArgs e)
        {
            var window = new CreateMailboxWindow(this, _apiClient, _refreshToken, _accessToken);
            window.Show();
        }

        private void ButtonOpen_OnClick(object? sender, RoutedEventArgs e)
        {
            var listBox = this.FindControl<ListBox>("ListMailboxes");
            var selectedItems = listBox.SelectedItems;
            if (selectedItems.Count != 1) return;

            var item = (ListBoxItem) selectedItems[0];
            var content = item.Content;
            if (!(content is TextBlock)) return;

            string txt = ((TextBlock) content).Text;
            if(txt.Contains(" ") || !txt.Contains("@"))
                return;
            
            var window = new ViewMailboxWindow(txt, _apiClient, _refreshToken, _accessToken);
            window.Show();
        }

        private ListBoxItem MakeText(string str, Action<object, object> action)
        {
            var item = new ListBoxItem();
            item.AddHandler(DoubleTappedEvent, action);
            var box = new TextBlock();
            box.Text = str;
            item.Content = box;
            return item;
        }

        private Control MakeItem(string hellooo, Action<object, object> action)
        {
            var txt = new TextBlock {Text = hellooo};
            txt.AddHandler(TappedEvent, action);
            return txt;
        }
    }
}