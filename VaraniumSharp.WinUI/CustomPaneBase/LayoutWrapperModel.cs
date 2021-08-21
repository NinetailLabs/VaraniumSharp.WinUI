using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VaraniumSharp.WinUI.CustomPaneBase
{
    /// <summary>
    /// Wrapper used to store controls
    /// </summary>
    public class LayoutWrapperModel
    {
        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public LayoutWrapperModel()
        {
            Controls = new List<ControlStorageModel>();
        }

        /// <summary>
        /// Construct with details
        /// </summary>
        /// <param name="layoutName">Layout identifier</param>
        /// <param name="controls">Controls for the layout</param>
        public LayoutWrapperModel(Guid layoutName, List<ControlStorageModel> controls)
        {
            LayoutName = layoutName;
            Controls = controls;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Collection of controls
        /// </summary>
        [JsonInclude]
        public List<ControlStorageModel> Controls { get; set; }

        /// <summary>
        /// The name of the layout being stored
        /// </summary>
        [JsonInclude]
        public Guid LayoutName { get; set; }

        #endregion
    }
}