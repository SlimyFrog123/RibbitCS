namespace RibbitCS;

class Ribbit
{
    public Logger Logger { get; private set; }

    public Ribbit(bool verboseMode)
    {
        Logger = new Logger(this, verboseMode);
    }
    
    private void EndWithError(string error)
    {
        Logger.LogError("Error: " + error);
        Environment.Exit(1);
    }
    
    public static void Main(string[] args)
    {
        bool verboseMode = args.Contains("-v") || args.Contains("--verbose");
        
        Ribbit ribbit = new Ribbit(verboseMode);
        Logger logger = ribbit.Logger;
        
        if (args.Length < 1)
            ribbit.EndWithError("You must specify the path of the script to run!");

        // Ensure that the script path is a valid path.
        string scriptPath = args[0];

        if (!File.Exists(scriptPath))
            ribbit.EndWithError("The given script does not exist.");

        // Get the compile type.
        List<CompileType> compileTypes = new List<CompileType>();
        
        if (args.Contains("--python") || args.Contains("-p"))
            compileTypes.Add(CompileType.Python);
        
        if (args.Contains("--ribbit") || args.Contains("-r"))
            compileTypes.Add(CompileType.Ribbit);

        if (compileTypes.Count > 0)
        {
            if (compileTypes.Count > 1)
                ribbit.EndWithError("You cannot compile use both Python and Ribbit compile methods at once.");
        }
        else
        {
            ribbit.EndWithError("You must specify a compile type; either Ribbit (--ribbit or -r) or Python " +
                                "(--python or -p).");
        }

        CompileType compileType = compileTypes[0];
        
        logger.Log(">", "Script path:", scriptPath);
        logger.Log(">", "Compile type:", compileType.ToString());

        if (verboseMode)
            logger.Log(">", "Running in verbose mode, all operations will be outputted to the console.");
        else
        {
            logger.Log(">", "Running in normal mode. To run in verbose mode for more details, use either" +
                            " --verbose or -v");
        }
        
        logger.LogVerbose("Reading script file...");
        string fileContents = File.ReadAllText(scriptPath);
        logger.LogVerbose("Finished reading script file.");
        
        logger.LogVerbose("Lexing...");
        Lexer lexer = new Lexer(logger, fileContents);
        List<Token> tokens = lexer.Lex();
        
        logger.LogVerbose("Finished lexing.");
        
        foreach (Token token in tokens)
        {
            string tokenString = token.Type.ToString();

            if (token.Value != null && token.Value.ToString() != "\0")
                tokenString += ": " + token.Value;
            
            logger.LogVerbose("Token: " + tokenString);
        }

        // Create a parser and attempt to parse the tokens.
        Parser parser = new Parser(logger, tokens);
        List<Node> nodes = parser.Parse();
    }
}