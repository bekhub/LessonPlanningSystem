using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;

namespace LPS.Client.Helpers;

public class MessageBoxHelper
{
    public static async Task<ButtonResult> ShowConfirmationAsync(string title, string message)
    {
        return await ShowMessageBoxAsync(ButtonEnum.YesNo, Icon.Warning, title, message);
    }
        
    public static async Task<ButtonResult> ShowErrorAsync(string message)
    {
        return await ShowMessageBoxAsync(ButtonEnum.Ok, Icon.Error, "Error", message);
    }
    
    public static async Task<ButtonResult> ShowSuccessAsync(string message)
    {
        return await ShowMessageBoxAsync(ButtonEnum.Ok, Icon.Success, "Success", message);
    }

    public static async Task<ButtonResult> ShowMessageAsync(string title, string message)
    {
        return await ShowMessageBoxAsync(ButtonEnum.Ok, Icon.Info, title, message);
    }
        
    public static async Task<ButtonResult> ShowMessageBoxAsync(ButtonEnum buttons, Icon icon, string title, string message)
    {
        var msBoxStandardWindow = MessageBox.Avalonia.MessageBoxManager.GetMessageBoxStandardWindow(new MessageBoxStandardParams
        {
            ButtonDefinitions = buttons,
            ContentTitle = title,
            ContentMessage = message,
            ShowInCenter = true,
            Icon = icon,
            CanResize = true,
            WindowStartupLocation = Avalonia.Controls.WindowStartupLocation.CenterOwner
        });

        return await msBoxStandardWindow
            .ShowDialog((Avalonia.Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)!
                .Windows[0]);
    }
}
