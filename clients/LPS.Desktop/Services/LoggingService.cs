using System;
using System.Text;
using ReactiveUI;

namespace LPS.Desktop.Services;

public sealed class LoggingService : ReactiveObject, ILoggingService
{
    private readonly StringBuilder _log;

    public LoggingService()
    {
        _log = new StringBuilder();
    }

    public string Logs => _log.ToString();

    public void Log(string message)
    {
        _log.Insert(0, "[" + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt") + "] " + message + "\n");
        this.RaisePropertyChanged(nameof(Logs));
    }
}
