namespace RibbitCS;

public class Token
{
    public TokenType Type { get; private set; }
    public object? Value { get; private set; }

    public Token(TokenType type, object? value = null)
    {
        this.Type = type;
        this.Value = value;
    }
}