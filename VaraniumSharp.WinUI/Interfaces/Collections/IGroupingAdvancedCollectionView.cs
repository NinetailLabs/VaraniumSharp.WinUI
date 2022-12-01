using System;
using System.Collections;
using System.ComponentModel;
using CommunityToolkit.WinUI.UI;

namespace VaraniumSharp.WinUI.Interfaces.Collections;

/// <summary>
/// A collection view implementation that supports filtering, sorting and grouping
/// </summary>
public interface IGroupingAdvancedCollectionView : IAdvancedCollectionView
{
    #region Events

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    event PropertyChangedEventHandler? PropertyChanged;

    #endregion

    #region Properties

    /// <summary>
    /// Function that is used to group the collection items.
    /// Set to null to remove the grouping.
    /// </summary>
    Func<object, object>? Group { get; set; }

    /// <summary>
    /// Gets or sets the source
    /// </summary>
    IList? Source { get; set; }

    #endregion
}