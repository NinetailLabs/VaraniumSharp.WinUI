using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace VaraniumSharp.WinUI.Interfaces.Pickers
{
    /// <summary>
    /// Wrapper for Pickers
    /// </summary>
    public interface IPickerWrapper
    {
        #region Public Methods

        /// <summary>
        /// Pick a folder
        /// </summary>
        /// <returns>Folder picked by the user. If no folder is picked null is returned</returns>
        Task<StorageFolder?> PickFolderAsync();

        /// <summary>
        /// Pick a single file with a specific type
        /// </summary>
        /// <param name="fileTypes">Dictionary containing file type name and extensions</param>
        /// <param name="suggestedFilename">Suggested name for the file. Pass null or empty to leave empty</param>
        /// <returns>StorageFile from the pick. If no file is picked null is returned</returns>
        Task<StorageFile?> PickSaveFileAsync(KeyValuePair<string, List<string>> fileTypes, string? suggestedFilename);

        #endregion
    }
}