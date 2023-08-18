namespace RibbitCS;

public class InitializeNode : Node
{
    public Token TypeToken { get; private set; }
    public Token NameToken { get; private set; }

    public InitializeNode(Token typeToken, Token nameToken)
    {
        TypeToken = typeToken;
        NameToken = nameToken;
    }
}