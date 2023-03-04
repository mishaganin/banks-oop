using Banks.Console.Interfaces;

namespace Banks.Console;

public class AbsParser : IParser
{
    private IParser? _nextParser;

    public AbsParser() => _nextParser = null;

    public IParser SetNextParser(IParser parser)
    {
        _nextParser = parser;
        return parser;
    }

    public virtual string Execute(string command)
    {
        if (_nextParser != null)
            return _nextParser.Execute(command);
        return string.Empty;
    }
}