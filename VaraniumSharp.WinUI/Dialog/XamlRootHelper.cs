using Microsoft.UI.Xaml;
using VaraniumSharp.Attributes;
using VaraniumSharp.Enumerations;

namespace VaraniumSharp.WinUI.Dialog;

/// <summary>
/// Helper to get easy access to the XamlRoot for the main window
/// </summary>
[AutomaticContainerRegistration(typeof(XamlRoot), ServiceReuse.Singleton)]
public class XamlRootHelper
{
    #region Properties

    /// <summary>
    /// XamlRoot instance
    /// </summary>
    public XamlRoot? XamlRoot { get; private set; }

    #endregion

    #region Private Methods

    /// <summary>
    /// Set the XamlRoot
    /// </summary>
    /// <param name="root">XamlRoot value to set</param>
    internal void SetXamlRoot(XamlRoot root)
    {
        XamlRoot = root;
    }

    #endregion
}