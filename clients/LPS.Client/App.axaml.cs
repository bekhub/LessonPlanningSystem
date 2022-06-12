using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using LPS.Client.Models;
using LPS.Client.Services;
using LPS.Client.ViewModels;
using LPS.Client.Views;
using ReactiveUI;
using Splat;

namespace LPS.Client;

public class App : Avalonia.Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Locator.CurrentMutable.RegisterLazySingleton(() => new ConventionalViewLocator(), typeof(IViewLocator));
        var suspension = new AutoSuspendHelper(ApplicationLifetime!);
        RxApp.SuspensionHost.CreateNewAppState = () => new AppState();
        RxApp.SuspensionHost.SetupDefaultSuspendResume(new NewtonsoftJsonSuspensionDriver<AppState>("appstate.json"));
        suspension.OnFrameworkInitializationCompleted();
        var state = RxApp.SuspensionHost.GetAppState<AppState>();
        Locator.CurrentMutable.RegisterLazySingleton<IAppState>(() => state);
            
        switch (ApplicationLifetime) {
            case IClassicDesktopStyleApplicationLifetime desktop:
                desktop.MainWindow = new MainWindow { DataContext = new MainViewModel() };
                break;
            case ISingleViewApplicationLifetime singleViewPlatform:
                singleViewPlatform.MainView = new MainView { DataContext = new MainViewModel() };
                break;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
