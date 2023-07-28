using System;
using System.Threading.Tasks;
using VaraniumSharp.WinUI.Interfaces.CustomPaneBase;

namespace VaraniumSharp.WinUI.Interfaces.BorderedPane;

/// <summary>
/// Context for the BorderedPane
/// </summary>
public interface IBorderedPaneContext : ICustomPaneContext
{
    #region Public Methods

    /// <summary>
    /// Set the title for the control
    /// </summary>
    Task SetTitleAsync();

    /// <summary>
    /// Set the functions to get and set the title for the control
    /// </summary>
    /// <param name="getCurrentTitle">Func to get the control`s current title</param>
    /// <param name="setCurrentTitle">Action to set the title for the control</param>
    void SetTitleFunctions(Func<string> getCurrentTitle, Action<string> setCurrentTitle);

    #endregion
}