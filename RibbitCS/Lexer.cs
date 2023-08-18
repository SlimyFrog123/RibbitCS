using System.Text.RegularExpressions;

namespace RibbitCS;

public class Lexer
{
    public string InputText { get; private set; }
    public int Position { get; private set; }
    public string CurrentChar { get; private set; }
    public State LexerState { get; private set; }

    public string NextChar => GetNextChar(0);
    
    private Logger _logger;
    
    // Regular expressions.
    private Regex _numberStartRegex;
    private Regex _numberRestRegex;
    private Regex _identifierStartRegex;
    private Regex _identifierRestRegex;
    private Regex _skipRegex;

    public Lexer(Logger parentLogger, string inputText)
    {
        InputText = inputText;
        Position = -1;
        CurrentChar = "";
        LexerState = State.Stop;

        _logger = new Logger(parentLogger, this);
        
        // Initialize regular expressions.
        _logger.LogVerbose("Initializing regular expressions...");

        _numberStartRegex = new Regex("[0-9]");
        _numberRestRegex = new Regex("[0-9_.]");
        _identifierStartRegex = new Regex("[a-zA-Z_]");
        _identifierRestRegex = new Regex("[a-zA-Z0-9_]");
        _skipRegex = new Regex("[\n\t\r ]");
        
        _logger.LogVerbose("Finished initializing regular expressions.");
    }
    
    private void EndWithError(string error)
    {
        _logger.LogError("Error: " + error);
        Environment.Exit(1);
    }

    private void Advance()
    {
        Position++;

        if (Position < InputText.Length)
        {
            CurrentChar = InputText[Position].ToString();
            LexerState = State.Go;
        }
        else
        {
            CurrentChar = "\0";
            LexerState = State.Stop;
        }
    }

    private void Advance(int times)
    {
        for (int i = 0; i < times; i++)
            Advance();
    }

    private string GetNextChar(int offset)
    {
        int position = Position + offset + 1;

        if (position < InputText.Length)
            return InputText[position].ToString();
        
        return "\0";
    }

    public List<Token> Lex()
    {
        List<Token> tokens = new List<Token>();

        Advance();

        while (LexerState == State.Go)
        {
            
            if (_numberStartRegex.IsMatch(CurrentChar))
            {
                // Parse a number (integer or float).
                string numberString = "";
                int decimalCount = 0;

                while (_numberRestRegex.IsMatch(CurrentChar))
                {
                    if (CurrentChar == ".")
                    {
                        decimalCount++;

                        if (decimalCount > 1)
                            EndWithError("You can only have a single decimal point in a floating point number.");
                        else
                            numberString += CurrentChar;
                    }
                    else if (CurrentChar != "_")
                        numberString += CurrentChar;
                    
                    Advance();
                }

                Token token;

                // Parse the number into its respective type (integer or float), and create its token.
                if (decimalCount == 1)
                    token = new Token(TokenType.Float, float.Parse(numberString));
                else
                    token = new Token(TokenType.Integer, int.Parse(numberString));
                
                // Add the token to the list of tokens.
                tokens.Add(token);
                
                continue;
            }
            else if (_identifierStartRegex.IsMatch(CurrentChar))
            {
                // Parse the identifier.
                string identifierString = "";

                while (_identifierRestRegex.IsMatch(CurrentChar))
                {
                    identifierString += CurrentChar;
                    
                    Advance();
                }

                // Create the token and add it to the list of tokens.
                Token token;
                
                if (new[] { "true", "false" }.Contains(identifierString))
                    token = new Token(TokenType.Boolean, identifierString == "true"); // Boolean.
                else
                    token = new Token(TokenType.Identifier, identifierString); // Plain identifier.
                
                tokens.Add(token);
                
                continue;
            }
            else if (CurrentChar == "\"")
            {
                // Parse a string literal.
                string stringContents = "";
                string lastChar = "\0";
                bool closed = false;
                
                Advance(); // Move past the opening double quote.

                while (LexerState == State.Go)
                {
                    if (CurrentChar == "\"")
                    {
                        if (lastChar == "\\")
                            stringContents += CurrentChar;
                        else
                        {
                            Advance(); // Move past the closing double quote.
                            closed = true;
                            break;
                        }
                    }
                    else
                    {
                        stringContents += CurrentChar;
                    }
                    
                    Advance();
                }

                if (closed)
                {
                    // Create the token and add it to the list of tokens.
                    Token token = new Token(TokenType.String, stringContents);
                    tokens.Add(token);
                }
                else
                    EndWithError("Unclosed string literal!");
                
                continue;
            }
            else if (CurrentChar == "'")
            {
                // Parse a char literal.
                Advance(); // Move past the opening single quote.
                
                _logger.LogVerbose("Character: ", CurrentChar);
                
                // Check for escape sequences.
                if (CurrentChar == "\\")
                {
                    Advance(); // Move past the backslash.
                    _logger.LogVerbose("CHARACTER: ", CurrentChar);

                    char escapedChar = '\0';

                    switch (CurrentChar)
                    {
                        case "'":
                            escapedChar = '\'';
                            break;
                        case "\"":
                            escapedChar = '\"';
                            break;
                        case "\\":
                            escapedChar = '\\';
                            break;
                        case "0":
                            escapedChar = '\0';
                            break;
                        case "a":
                            escapedChar = '\a';
                            break;
                        case "b":
                            escapedChar = '\b';
                            break;
                        case "f":
                            escapedChar = '\f';
                            break;
                        case "n":
                            escapedChar = '\n';
                            break;
                        case "r":
                            escapedChar = '\r';
                            break;
                        case "t":
                            escapedChar = '\t';
                            break;
                        case "v":
                            escapedChar = '\v';
                            break;
                        default:
                            EndWithError("Invalid escape sequence in character literal.");
                            break;
                    }
                    
                    Advance(); // Move past the escape sequence character.

                    if (CurrentChar != "'")
                        EndWithError("Character literal must only contain a single character.");

                    Token token = new Token(TokenType.Char, escapedChar);
                    tokens.Add(token);
                    
                    Advance();
                }
                else if (NextChar == "'")
                    EndWithError("Character literal must only contain a single character.");
                else
                {
                    char charLiteral = CurrentChar[0];
                    
                    Advance(); // Move past the character.
                    
                    if (CurrentChar != "'")
                        EndWithError("Character literal must only contain a single character.");

                    Token token = new Token(TokenType.Char, charLiteral);
                    tokens.Add(token);
                    
                    Advance(); // Move past the closing single quote.
                }
                
                continue;
            }
            else if (!_skipRegex.IsMatch(CurrentChar))
            {
                // Non-skip character (not space, tab, carriage return, or newline).

                if (CurrentChar == "+")
                {
                    // Addition (add, increment, positional add).
                    if (NextChar == "+")
                    {
                        // Increment.
                        Advance(1); // Advance to the next plus sign.

                        Token token = new Token(TokenType.Increment);
                        tokens.Add(token);
                        
                        continue;
                    }
                    else if (NextChar == "=")
                    {
                        // Positional add.
                        Advance(1); // Advance to the equals sign.

                        Token token = new Token(TokenType.PositionalAdd);
                        tokens.Add(token);
                    }
                    else
                    {
                        // We're gonna go with just addition, just as a catchall, screw errors. :)
                        Token token = new Token(TokenType.Add);
                        tokens.Add(token);
                    }
                }
                else if (CurrentChar == "-")
                {
                    // Subtraction (subtract, decrement, positional subtract).
                    if (NextChar == "-")
                    {
                        // Decrement.
                        Advance(1); // Advance to the next minus sign.

                        Token token = new Token(TokenType.Decrement);
                        tokens.Add(token);
                        
                        continue;
                    }
                    else if (NextChar == "=")
                    {
                        // Positional subtract.
                        Advance(1); // Advance to the equals sign.

                        Token token = new Token(TokenType.PositionalSubtract);
                        tokens.Add(token);
                    }
                    else
                    {
                        // Subtract.
                        Token token = new Token(TokenType.Subtract);
                        tokens.Add(token);
                    }
                }
                else if (CurrentChar == "*")
                {
                    // Multiplication (multiply, positional multiply).
                    if (NextChar == "=")
                    {
                        // Positional multiply.
                        Advance(1); // Advance to the equals sign.
                        
                        Token token = new Token(TokenType.PositionalMultiply);
                        tokens.Add(token);
                    }
                    else
                    {
                        // Multiply.
                        Token token = new Token(TokenType.Multiply);
                        tokens.Add(token);
                    }
                }
                else if (CurrentChar == "/")
                {
                    // Division (divide, positional divide).
                    if (NextChar == "=")
                    {
                        // Positional divide.
                        Advance(1); // Advance to the equals sign.

                        Token token = new Token(TokenType.PositionalDivide);
                        tokens.Add(token);
                    }
                    else
                    {
                        // Divide.
                        Token token = new Token(TokenType.Divide);
                        tokens.Add(token);
                    }
                }
                else if (CurrentChar == "%")
                {
                    // Remainder (remainder, positional remainder).
                    if (NextChar == "=")
                    {
                        // Positional remainder.
                        Advance(1); // Advance to the equals sign.

                        Token token = new Token(TokenType.PositionalRemainder);
                        tokens.Add(token);
                    }
                    else
                    {
                        // Remainder.
                        Token token = new Token(TokenType.Remainder);
                        tokens.Add(token);
                    }
                }
                else if (CurrentChar == "^")
                {
                    // Power (power, positional power).
                    if (NextChar == "=")
                    {
                        // Positional power.
                        Advance(1); // Advance to the equals sign.

                        Token token = new Token(TokenType.PositionalPower);
                        tokens.Add(token);
                    }
                    else
                    {
                        // Power.
                        Token token = new Token(TokenType.Power);
                        tokens.Add(token);
                    }
                }
                else if (CurrentChar == ".")
                {
                    // Period.
                    Token token = new Token(TokenType.Period, '.');
                    tokens.Add(token);
                }
                else if (CurrentChar == ",")
                {
                    // Comma.
                    Token token = new Token(TokenType.Comma, ',');
                    tokens.Add(token);
                }
                else if (CurrentChar == "(")
                {
                    // Opening parenthesis.
                    Token token = new Token(TokenType.OpeningParenthesis, '(');
                    tokens.Add(token);
                }
                else if (CurrentChar == ")")
                {
                    // Closing parenthesis.
                    Token token = new Token(TokenType.ClosingParenthesis, ')');
                    tokens.Add(token);
                }
                else if (CurrentChar == "[")
                {
                    // Opening bracket.
                    Token token = new Token(TokenType.OpeningBracket, '[');
                    tokens.Add(token);
                }
                else if (CurrentChar == "]")
                {
                    // Closing bracket.
                    Token token = new Token(TokenType.ClosingBracket, ']');
                    tokens.Add(token);
                }
                else if (CurrentChar == "{")
                {
                    // Opening brace.
                    Token token = new Token(TokenType.OpeningBrace, '{');
                    tokens.Add(token);
                }
                else if (CurrentChar == "}")
                {
                    // Closing brace.
                    Token token = new Token(TokenType.ClosingBrace, '}');
                    tokens.Add(token);
                }
                else if (CurrentChar == "<")
                {
                    // Less than.
                    Token token = new Token(TokenType.LessThan, '<');
                    tokens.Add(token);
                }
                else if (CurrentChar == ">")
                {
                    // Greater than.
                    Token token = new Token(TokenType.GreaterThan, '>');
                    tokens.Add(token);
                }
                else if (CurrentChar == "=")
                {
                    if (NextChar == "=")
                    {
                        // Equal (equality check).
                        Advance(1); // Advance to the second equals sign.

                        Token token = new Token(TokenType.Equal);
                        tokens.Add(token);
                    }
                    else
                    {
                        // Assign.
                        Token token = new Token(TokenType.Assign);
                        tokens.Add(token);
                    }
                }
                else if (CurrentChar == "!" && NextChar == "=")
                {
                    // Not equal (non-equality check).
                    Advance(); // Advance past the equals sign.
                    
                    Token token = new Token(TokenType.NotEqual);
                    tokens.Add(token);
                }
                else if (CurrentChar == "&" && NextChar == "&")
                {
                    // And.
                    Advance(); // Advance past the second ampersand.

                    Token token = new Token(TokenType.And);
                    tokens.Add(token);
                }
                else if (CurrentChar == "|" && NextChar == "|")
                {
                    // Or.
                    Advance(); // Advance past the second pipe.

                    Token token = new Token(TokenType.Or);
                    tokens.Add(token);
                }
                else
                {
                    // Some other character. Just add it.
                    Token token = new Token(TokenType.Character, CurrentChar[0]);
                    tokens.Add(token);
                }
            }
            
            Advance();
        }

        Token eofToken = new Token(TokenType.EndOfFile);
        tokens.Add(eofToken);

        return tokens;
    }
}