namespace RibbitCS;

public class PositionalDivideNode : PositionalMathNode
{
    public Token AmountToken { get; private set; }
    
    public PositionalDivideNode(Token nameToken, Token amountToken) : base(nameToken)
    {
        AmountToken = amountToken;
    }
}