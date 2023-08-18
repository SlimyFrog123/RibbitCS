namespace RibbitCS;

public class PositionalSubtractNode : PositionalMathNode
{
    public Token AmountToken { get; private set; }
    
    public PositionalSubtractNode(Token nameToken, Token amountToken) : base(nameToken)
    {
        AmountToken = amountToken;
    }
}