using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using VaraniumSharp.WinUI.Interfaces.CustomPaneBase;

namespace VaraniumSharp.WinUI.CustomPaneBase
{
    /// <summary>
    /// Stores the details of a <see cref="IDisplayComponent"/> entry
    /// </summary>
    public class ControlStorageModel
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ControlStorageModel()
        {
            ChildItems = new List<ControlStorageModel>();
            ContentId = Guid.Empty;
            Title = string.Empty;
        }

        /// <summary>
        /// Construct from a layoutDisplay entry
        /// </summary>
        /// <param name="displayComponent">The display component that will be serialized</param>
        public ControlStorageModel(IDisplayComponent displayComponent)
        {
            ChildItems = new List<ControlStorageModel>();
            ContentId = displayComponent.ContentId;
            Width = displayComponent.Width;
            Height = displayComponent.Height;
            Title = displayComponent.Title;
            InstanceId = displayComponent.InstanceId;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Collection containing child items if the control can store children of its own
        /// </summary>
        [JsonInclude]
        public List<ControlStorageModel> ChildItems { get; set; }

        /// <summary>
        /// The content Id of the component
        /// </summary>
        [JsonInclude]
        public Guid ContentId { get; set; }

        /// <summary>
        /// The height of the control
        /// </summary>
        [JsonInclude]
        public double Height { get; set; }

        /// <summary>
        /// The instance Id for the control
        /// </summary>
        [JsonInclude]
        public Guid InstanceId { get; set; }

        /// <summary>
        /// The title for the control
        /// </summary>
        [JsonInclude]
        public string Title { get; set; }

        /// <summary>
        /// Unique identifier to match control back during initialization
        /// </summary>
        public Guid UniqueControlIdentifier { get; set; }

        /// <summary>
        /// The width of the control
        /// </summary>
        [JsonInclude]
        public double Width { get; set; }

        #endregion
    }
}