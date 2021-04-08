using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PoopmailGui
{
    public class OverviewWindow : Window
    {
        public OverviewWindow()
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
    }
}