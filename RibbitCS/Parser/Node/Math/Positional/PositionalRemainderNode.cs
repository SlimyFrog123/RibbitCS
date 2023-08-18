namespace RibbitCS.Parser.Node.Math.Positional;

public class PositionalRemainderNode : PositionalMathNode
{
    public Token AmountToken { get; private set; }
    
    public PositionalRemainderNode(Token nameToken, Token amountToken) : base(nameToken)
    {
        AmountToken = amountToken;
    }
}