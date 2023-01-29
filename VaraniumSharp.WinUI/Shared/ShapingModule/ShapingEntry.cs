using System;
using System.ComponentModel;
using VaraniumSharp.WinUI.DragAndDrop;

namespace VaraniumSharp.WinUI.Shared.ShapingModule
{
    /// <summary>
    /// Data about properties used for shaping
    /// </summary>
    public class ShapingEntry : IStringDragItem, INotifyPropertyChanged
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ShapingEntry(string entryType)
        {
            PropertyName = string.Empty;
            Header = string.Empty;
            Tooltip = string.Empty;
            EntryType = entryType;
        }

        #endregion

        #region Events

        /// <inheritdoc/>
#pragma warning disable CS0067 // Is used via Fody
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067

        /// <summary>
        /// Request that the collection shaping is updated
        /// </summary>
        public event EventHandler? RequestShapingUpdate;

        #endregion

        #region Properties

        /// <inheritdoc/>
        public string EntryType { get; }

        /// <summary>
        /// Header for the shaping control
        /// </summary>
        public string Header { get; set; }

        /// <inheritdoc/>
        public string Identifier => PropertyName;

        /// <summary>
        /// Name of the property that the information is for
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Tooltip for the sort order
        /// </summary>
        public string Tooltip { get; set; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Request that the <see cref="RequestShapingUpdate"/> event is fired
        /// </summary>
        protected void RequestShapingUpdateEvent()
        {
            RequestShapingUpdate?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}