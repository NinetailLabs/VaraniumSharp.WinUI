using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.UI.Text;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;

namespace VaraniumSharp.WinUI.ExtensionMethods;

/// <summary>
/// Extension method for the <see cref="RichTextBlock"/>
/// </summary>
public static class RichTextBlockExtensionMethods
{
    #region Public Methods

    /// <summary>
    /// Show text formatted with token in the <see cref="RichTextBlock"/> as formatted text.
    /// Currently, supports bold, italic and new paragraphs.
    /// </summary>
    /// <param name="textBlock">The text block in which the text should be displayed</param>
    /// <param name="textToShow">The delimited text to display in the text box</param>
    public static void ShowRichText(this RichTextBlock textBlock, string textToShow)
    {
        ShowRichText(textBlock, textToShow, new());
    }

    /// <summary>
    /// Show text formatted with token in the <see cref="RichTextBlock"/> as formatted text.
    /// Currently, supports bold, italic and new paragraphs.
    /// <remarks>
    /// The code is based on this StackOverflow answer https://stackoverflow.com/a/77020059/2017251
    /// </remarks>
    /// </summary>
    /// <param name="textBlock">The text block in which the text should be displayed</param>
    /// <param name="textToShow">The delimited text to display in the text box</param>
    /// <param name="tokenHelper">Contains the tokens and regex to extract them</param>
    public static void ShowRichText(this RichTextBlock textBlock, string textToShow, TokenHelper tokenHelper)
    {
        var fontWeight = FontWeights.Normal;
        var fontStyle = FontStyle.Normal;

        Paragraph paragraph = new();

        foreach (var token in TokenizeString(textToShow, tokenHelper.TokenRegex))
        {
            if (token == tokenHelper.BoldStartToken || token == tokenHelper.BoldEndToken)
            {
                fontWeight = token == tokenHelper.BoldStartToken 
                    ? FontWeights.Bold 
                    : FontWeights.Normal;
                continue;
            }

            if (token == tokenHelper.ItalicStartToken || token == tokenHelper.ItalicEndToken)
            {
                fontStyle = token == tokenHelper.ItalicStartToken 
                    ? FontStyle.Italic 
                    : FontStyle.Normal;
                continue;
            }

            if (token == tokenHelper.NewLineToken)
            {
                textBlock.Blocks.Add(paragraph);
                paragraph = new();
                continue;
            }

            Run run = new()
            {
                Text = token,
                FontWeight = fontWeight,
                FontStyle = fontStyle,
            };

            paragraph.Inlines.Add(run);
        }

        textBlock.Blocks.Add(paragraph);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Tokenize the string
    /// </summary>
    /// <param name="input">The string to tokenize</param>
    /// <param name="tokenRegex">Regex to use for extracting the tokens</param>
    /// <returns>List of tokens</returns>
    private static List<string> TokenizeString(string input, string tokenRegex)
    {
        List<string> tokens = [];
        var regex = new Regex(tokenRegex);
        var matches = regex.Matches(input);
        var lastIndex = 0;

        foreach (var match in matches.Cast<Match>())
        {
            if (match.Index > lastIndex)
            {
                tokens.Add(input[lastIndex..match.Index]);
            }

            tokens.Add(match.Value);
            lastIndex = match.Index + match.Length;
        }

        if (lastIndex < input.Length)
        {
            tokens.Add(input[lastIndex..]);
        }

        return tokens;
    }

    #endregion
}