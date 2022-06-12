using Avalonia.Data.Converters;

namespace LPS.Client.Converters;

public static class StringConverters
{
    /// <summary>
    /// A value converter that returns true if the input string is null or an whitespace string.
    /// </summary>
    public static readonly IValueConverter IsNullOrWhiteSpace = 
        new FuncValueConverter<string?, bool>(string.IsNullOrWhiteSpace);
    
    /// <summary>
    /// A value converter that returns true if the input string is null or an empty string.
    /// </summary>
    public static readonly IValueConverter IsNullOrEmpty =
        new FuncValueConverter<string?, bool>(string.IsNullOrEmpty);
}
