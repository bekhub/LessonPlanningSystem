namespace LPS.Client.Services;

public interface ILoggingService
{
    void Log(string message);
    string Logs { get; }
}
