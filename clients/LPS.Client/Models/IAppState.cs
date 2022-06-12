using System.Collections.ObjectModel;

namespace LPS.Client.Models;

public interface IAppState
{
    public ObservableCollection<ConnectionDetails> SavedConnectionDetailsList { get; set; }
}
