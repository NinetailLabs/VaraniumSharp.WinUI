using System;
using System.IO;
using VaraniumSharp.Attributes;
using VaraniumSharp.Enumerations;
using VaraniumSharp.WinUI.Interfaces.CustomPaneBase;

namespace VaraniumSharp.WinUI.CustomPaneBase
{
    /// <summary>
    /// Assist with getting the path where layout details are saved
    /// </summary>
    [AutomaticContainerRegistration(typeof(ILayoutStorageOptions), ServiceReuse.Singleton)]
    public class LayoutStorageOptions : ILayoutStorageOptions
    {
        #region Public Methods

        /// <inheritdoc/>
        public string GetJsonPath(string filename)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var fullPath = Path.Combine(path, "NineTailLabs", "VaraniumSharp", "Layout");
            Directory.CreateDirectory(fullPath);
            var file = Path.Combine(fullPath, filename);
            return file;
        }

        #endregion
    }
}