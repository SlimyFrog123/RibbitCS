namespace RibbitCS.Parser;

public class Parser
{
    private Logger _logger;
    private List<Token> _tokens;
    private Token? _currentToken;
    private int _position;
    private State _state;

    public Parser(Logger parentLogger, List<Token> tokens)
    {
        _logger = new Logger(parentLogger, this);
        _tokens = tokens;

        _currentToken = null;
        _position = -1;
        _state = State.Stop;
    }

    private void Advance()
    {
        _position++;

        if (_position < _tokens.Count)
        {
            _currentToken = _tokens[_position];
            _state = State.Go;
        }
        else
        {
            _currentToken = null;
            _state = State.Stop;
        }
    }
}