namespace RibbitCS;

public class AssignNode : Node
{
    public Token NameToken { get; private set; }
    public Token ValueToken { get; private set; }

    public AssignNode(Token nameToken, Token valueToken)
    {
        NameToken = nameToken;
        ValueToken = valueToken;
    }
}