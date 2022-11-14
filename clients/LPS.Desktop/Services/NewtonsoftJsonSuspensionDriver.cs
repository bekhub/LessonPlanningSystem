using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using Newtonsoft.Json;
using ReactiveUI;

namespace LPS.Desktop.Services;

public sealed class NewtonsoftJsonSuspensionDriver<TState> : ISuspensionDriver where TState: class, new()
{
    private readonly string _file;
    private readonly JsonSerializerSettings _settings = new() {
        TypeNameHandling = TypeNameHandling.All,

    };

    public NewtonsoftJsonSuspensionDriver(string file) => _file = file;

    public IObservable<Unit> InvalidateState()
    {
        if (File.Exists(_file))
            File.Delete(_file);
        return Observable.Return(Unit.Default);
    }

    public IObservable<object> LoadState()
    {
        var lines = File.ReadAllText(_file);
        var state = JsonConvert.DeserializeObject<TState>(lines, _settings);
        return Observable.Return(state ?? new TState());
    }

    public IObservable<Unit> SaveState(object state)
    {
        var lines = JsonConvert.SerializeObject(state, _settings);
        File.WriteAllText(_file, lines);
        return Observable.Return(Unit.Default);
    }
}
