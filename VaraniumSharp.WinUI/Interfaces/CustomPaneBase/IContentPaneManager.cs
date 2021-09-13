using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using VaraniumSharp.WinUI.Interfaces.HorizontalPane;

namespace VaraniumSharp.WinUI.Interfaces.CustomPaneBase
{
    /// <summary>
    /// Assists with the serialization and deserialization of the controls
    /// </summary>
    public interface IContentPaneManager
    {
        #region Properties

        /// <summary>
        /// The base pane that is used to display content
        /// </summary>
        IHorizontalLayoutPane BasePane { get; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Delete a layout file
        /// </summary>
        /// <param name="layoutId">Id of the layout to remove</param>
        Task DeleteLayoutAsync(Guid layoutId);

        /// <summary>
        /// Request that the current layout is saved
        /// </summary>
        Task SaveLayoutAsync();

        /// <summary>
        /// Request that the setting page is displayed
        /// </summary>
        Task ShowSettingPageAsync();

        /// <summary>
        /// Request update of the <see cref="BasePane"/>'s size
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        void UpdateBasePaneSize(object sender, SizeChangedEventArgs e);

        /// <summary>
        /// Request that the displayed content is updated
        /// </summary>
        /// <param name="tabName">The name of the content to display</param>
        Task UpdateContentAsync(string tabName);

        #endregion
    }
}