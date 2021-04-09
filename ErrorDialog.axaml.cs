using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace PoopmailGui
{
    public class ErrorDialog : Window
    {
        public ErrorDialog()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        public ErrorDialog(int width, int height, string str)
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            var block = this.FindControl<TextBlock>("Text");
            Height = block.Height = height;
            Width = block.Width = width;
            
            block.TextWrapping = TextWrapping.WrapWithOverflow;
            block.Text = str;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}