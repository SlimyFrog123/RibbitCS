﻿namespace RibbitCS.Parser.Node.Math;

public class DecrementNode : MathNode
{
    public Token NameToken { get; private set; }

    public DecrementNode(Token nameToken)
    {
        NameToken = nameToken;
    }
}