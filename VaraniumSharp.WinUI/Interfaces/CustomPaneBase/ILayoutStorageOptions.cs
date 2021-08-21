namespace VaraniumSharp.WinUI.Interfaces.CustomPaneBase
{
    /// <summary>
    /// Assist with getting the path where layout details are saved
    /// </summary>
    public interface ILayoutStorageOptions
    {
        #region Public Methods

        /// <summary>
        /// Get the path where layout files are stored
        /// </summary>
        /// <param name="filename">Name of the file</param>
        /// <returns>Filepath where layout files are stored</returns>
        public string GetJsonPath(string filename);

        #endregion
    }
}