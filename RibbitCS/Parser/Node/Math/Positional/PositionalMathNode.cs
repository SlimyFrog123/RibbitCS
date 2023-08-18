namespace RibbitCS.Parser.Node.Math.Positional;

public class PositionalMathNode : Node
{
    public Token NameToken { get; private set; }

    public PositionalMathNode(Token nameToken)
    {
        NameToken = nameToken;
    }
}