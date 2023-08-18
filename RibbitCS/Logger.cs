namespace RibbitCS;

public class Logger
{
    private string Prefix { get; set; }
    private readonly bool _verbose;
    
    public Logger(object obj, bool verbose)
    {
        Prefix = "[" + obj.GetType().Name + "]";
        _verbose = verbose;
    }

    public Logger(Logger parent, object obj)
    {
        Prefix = parent.Prefix + "[" + obj.GetType().Name + "]";
        _verbose = parent._verbose;
    }

    public void Log(params object[] arguments)
    {
        string logString = string.Join(" ", arguments);
        
        Console.WriteLine(Prefix + ": " + logString);
    }
    
    public void LogLine(params object[] arguments)
    {
        string logString = string.Join("\n", arguments);
        
        Console.WriteLine(Prefix + ": " + logString);
    }

    public void LogVerbose(params object[] arguments)
    {
        if (_verbose)
            Log(arguments);
    }

    public void LogError(string message)
    {
        Console.Error.WriteLine(Prefix + ": " + message);
    }
}