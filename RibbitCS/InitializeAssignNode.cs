namespace RibbitCS;

public class InitializeAssignNode : Node
{
    public Token TypeToken { get; private set; }
    public Token NameToken { get; private set; }
    public Token ValueToken { get; private set; }

    public InitializeAssignNode(Token typeToken, Token nameToken, Token valueToken)
    {
        TypeToken = typeToken;
        NameToken = nameToken;
        ValueToken = valueToken;
    }
}