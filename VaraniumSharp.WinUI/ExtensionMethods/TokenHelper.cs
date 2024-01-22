namespace VaraniumSharp.WinUI.ExtensionMethods;

/// <summary>
/// Contains the tokens and regex to extract the tokens from the text
/// </summary>
public class TokenHelper
{
    #region Constructor

    /// <summary>
    /// Default Constructor that sets the tokens up in the format of {b}
    /// </summary>
    public TokenHelper()
    {
        TokenRegex = @"(\{[^{}]*\})";
        BoldStartToken = "{b}";
        BoldEndToken = "{/b}";
        ItalicStartToken = "{i}";
        ItalicEndToken = "{/i}";
        NewLineToken = "{br}";
    }

    #endregion

    #region Properties

    /// <summary>
    /// The token used to delimit the end of bold text
    /// </summary>
    public string BoldEndToken { get; init; }

    /// <summary>
    /// The token used to delimit the start of bold text
    /// </summary>
    public string BoldStartToken { get; init; }

    /// <summary>
    /// The token used to delimit the end of italic text
    /// </summary>
    public string ItalicEndToken { get; init; }

    /// <summary>
    /// The token used to delimit the start of italic text
    /// </summary>
    public string ItalicStartToken { get; init; }

    /// <summary>
    /// Token to delimit a new line
    /// </summary>
    public string NewLineToken { get; init; }

    /// <summary>
    /// Regex string used to extract the tokens from the string
    /// </summary>
    public string TokenRegex { get; init; }

    #endregion
}