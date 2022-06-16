#nullable enable
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using LPS.Client.Helpers;
using LPS.Client.Models;
using LPS.Utils.Extensions;
using MySqlConnector;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace LPS.Client.ViewModels;

public class ConnectionPageViewModel : RoutableViewModel
{
    [Reactive] public string? Server { get; set; }
    [Reactive] public string? Database { get; set; }
    [Reactive] public string? User { get; set; }
    [Reactive] public string? Password { get; set; }
    [Reactive] public string? MysqlVersion { get; set; }
    [Reactive] public ConnectionDetails? CurrentConnectionDetails { get; set; }
    [Reactive] public ConnectionDetails? SelectedConnectionDetails { get; set; }
    public ObservableCollection<ConnectionDetails> SavedConnectionDetailsList { get; }

    public ReactiveCommand<Unit, Unit> ConnectToDatabase { get; }
    public ReactiveCommand<Unit, Unit> RemoveConnectionDetails { get; }

    public ConnectionPageViewModel(RouterViewModel routerViewModel) : this(null, routerViewModel) { }

    public ConnectionPageViewModel(IAppState? appState, RouterViewModel routerViewModel) 
        : base(routerViewModel, "connection")
    {
        appState ??= Locator.Current.GetService<IAppState>()!;
        SavedConnectionDetailsList = appState.SavedConnectionDetailsList;
        var allFieldsAreFilled = this.WhenAnyValue(
                x => x.Server, x => x.Database, 
                x => x.User, x => x.Password, 
                (server, database, user, password) =>
                    !(server.IsNullOrWhiteSpace() || database.IsNullOrWhiteSpace() || user.IsNullOrWhiteSpace() ||
                      password.IsNullOrWhiteSpace()));
        ConnectToDatabase = ReactiveCommand.CreateFromTask(ConnectToDatabaseImpl, allFieldsAreFilled);
        var canRemove = this.WhenAny(x => x.SelectedConnectionDetails, 
            details => details.Value != null);
        RemoveConnectionDetails = ReactiveCommand.Create(RemoveConnectionDetailsImpl, canRemove);
        this.WhenActivated(disposable => {
            RouterViewModel.IsGoBackEnabled = false;
            this.WhenAny(x => x.CurrentConnectionDetails, 
                    details => details.Value != null)
                .Subscribe(x => RouterViewModel.IsGoNextEnabled = x).DisposeWith(disposable);
        });
    }

    public override void OnGoNext()
    {
        RouterViewModel.ConfigurationDetails.ConnectionDetails = CurrentConnectionDetails! with {};
        base.OnGoNext();
    }

    private async Task ConnectToDatabaseImpl()
    {
        var connectionDetails = CreateConnectionDetails();
        var connection = new MySqlConnection(connectionDetails.GetConnectionString());
        try {
            await connection.OpenAsync();
            const string stm = "SELECT VERSION()";
            var cmd = new MySqlCommand(stm, connection);
            var version = await cmd.ExecuteScalarAsync();
            MysqlVersion = version!.ToString();
            connectionDetails.MysqlVersion = version.ToString();
            await connection.CloseAsync();
            await MessageBoxHelper.ShowSuccessAsync("Successfully connected to database");
            if (SavedConnectionDetailsList.FirstOrDefault(x => x == connectionDetails) == null)
                SavedConnectionDetailsList.Add(connectionDetails);
            CurrentConnectionDetails = connectionDetails;
        } catch (Exception ex) {
            await MessageBoxHelper.ShowErrorAsync(ex.Message);
            CurrentConnectionDetails = null;
        }
    }

    public void ConnectionDetailsSelectionChanged(ConnectionDetails? details)
    {
        Server = details?.Server;
        Database = details?.Database;
        User = details?.User;
        Password = details?.Password;
        MysqlVersion = details?.MysqlVersion;
        CurrentConnectionDetails = details;
    }

    private void RemoveConnectionDetailsImpl()
    {
        if (SelectedConnectionDetails == null) return;
        SavedConnectionDetailsList.Remove(SelectedConnectionDetails);
    }
    
    private ConnectionDetails CreateConnectionDetails()
    {
        return new ConnectionDetails(Server!, Database!, User!, Password!, MysqlVersion!);
    }
}
