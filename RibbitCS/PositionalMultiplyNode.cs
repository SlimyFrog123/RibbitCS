namespace RibbitCS;

public class PositionalMultiplyNode : PositionalMathNode
{
    public Token AmountToken { get; private set; }
    
    public PositionalMultiplyNode(Token nameToken, Token amountToken) : base(nameToken)
    {
        AmountToken = amountToken;
    }
}