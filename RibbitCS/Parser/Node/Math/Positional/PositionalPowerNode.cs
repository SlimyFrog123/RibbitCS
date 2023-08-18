﻿namespace RibbitCS.Parser.Node.Math.Positional;

public class PositionalPowerNode : PositionalMathNode
{
    public Token AmountToken { get; private set; }
    
    public PositionalPowerNode(Token nameToken, Token amountToken) : base(nameToken)
    {
        AmountToken = amountToken;
    }
}