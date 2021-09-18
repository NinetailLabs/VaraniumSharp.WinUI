using Microsoft.UI.Xaml;
using VaraniumSharp.Attributes;
using VaraniumSharp.Enumerations;
using VaraniumSharp.WinUI.Interfaces.Pickers;

namespace VaraniumSharp.WinUI.Pickers
{
    /// <summary>
    /// Assist with setting the main <see cref="Window"/> that Pickers will be associated with
    /// </summary>
    [AutomaticContainerRegistration(typeof(IOwnerWindow), ServiceReuse.Singleton)]
    public class OwnerWindow : IOwnerWindow
    {
        #region Properties

        /// <inheritdoc />
        public Window? ParentWindow { get; set; }

        #endregion
    }
}