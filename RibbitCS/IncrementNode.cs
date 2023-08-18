namespace RibbitCS;

public class IncrementNode : MathNode
{
    public Token NameToken { get; private set; }

    public IncrementNode(Token nameToken)
    {
        NameToken = nameToken;
    }
}