using Avalonia.Markup.Xaml;
using LPS.Client.ViewModels;

namespace LPS.Client.Views;

public partial class ConfigurationPageView : BaseControl<ConfigurationPageViewModel>
{
    public ConfigurationPageView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

