using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace LPS.Desktop.Models;

[DataContract]
public class AppState: IAppState
{
    [DataMember]
    public ObservableCollection<ConnectionDetails> SavedConnectionDetailsList { get; set; }

    public AppState()
    {
        SavedConnectionDetailsList = new ObservableCollection<ConnectionDetails>();
    }
}
