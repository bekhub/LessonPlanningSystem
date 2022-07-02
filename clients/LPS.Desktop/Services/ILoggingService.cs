namespace LPS.Desktop.Services;

public interface ILoggingService
{
    void Log(string message);
    string Logs { get; }
}
