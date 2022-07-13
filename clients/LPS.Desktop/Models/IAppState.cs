#nullable enable
using System.Collections.ObjectModel;

namespace LPS.Desktop.Models;

public interface IAppState
{
    public ObservableCollection<ConnectionDetails> SavedConnectionDetailsList { get; set; }
}
