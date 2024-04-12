using System.ComponentModel;
using VaraniumSharp.WinUI.SortModule;

namespace TestHelper.Sorting;

public class EmbeddedEntry : INotifyPropertyChanged
{
    #region Events

    public event PropertyChangedEventHandler? PropertyChanged;

    #endregion

    #region Properties

    /// <summary>
    /// Embedded Id
    /// </summary>
    [SortableProperty("Embedded Id", "Sort by Embedded Id")]
    public int EmbeddedId { get; set; }

    #endregion
}