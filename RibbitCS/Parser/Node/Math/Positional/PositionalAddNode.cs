namespace RibbitCS.Parser.Node.Math.Positional;

public class PositionalAddNode : PositionalMathNode
{
    public Token AmountToken { get; private set; }
    
    public PositionalAddNode(Token nameToken, Token amountToken) : base(nameToken)
    {
        AmountToken = amountToken;
    }
}