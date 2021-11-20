using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.WinUI.UI;

namespace VaraniumSharp.WinUI.Shared.ShapingModule
{
    /// <summary>
    /// Base module that can be used to create collection shaping modules (eg GroupingPropertyModule)
    /// </summary>
    public abstract partial class ShapingPropertyModuleBase<T> : INotifyPropertyChanged where T : ShapingPropertyAttributeBase
    {
        #region Constructor

        /// <summary>
        /// Construct and populate with the collection that the module will shape
        /// </summary>
        /// <param name="viewSource">The collection that the module will shape</param>
        protected ShapingPropertyModuleBase(IAdvancedCollectionView viewSource)
        {
            ViewSource = viewSource;
            AvailableShapingEntries = new();
            EntriesShapedBy = new();
            EntriesShapedBy.CollectionChanged += EntriesShapedByOnCollectionChanged;
            NestedTypeList = new();
        }

        #endregion

        #region Events

        /// <inheritdoc/>
#pragma warning disable CS0067 // Is used via Fody
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067

        /// <summary>
        /// Occurs when the sort has changed
        /// </summary>
        public event EventHandler? ShapingChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Collection containing entries that are not currently being used to shape the collection
        /// </summary>
        public ObservableCollection<ShapingEntry> AvailableShapingEntries { get; }

        /// <summary>
        /// Indicate if default shaping should be done on the collection when the shaping entries are generated
        /// </summary>
        public bool DisableDefaultShaping { get; set; }

        /// <summary>
        /// Collection containing entries that are currently used to shape the collection
        /// </summary>
        public ObservableCollection<ShapingEntry> EntriesShapedBy { get; }

        /// <summary>
        /// Indicate if the collection is shaped
        /// </summary>
        public bool IsShaped { get; protected set; }

        /// <summary>
        /// Indicate if the UI button to move entries from the <see cref="AvailableShapingEntries"/> to the <see cref="EntriesShapedBy"/> collection should be enabled
        /// </summary>
        public bool MoveAvailableEnabled { get; private set; }

        /// <summary>
        /// Indicate if the UI button to move entries from the <see cref="EntriesShapedBy"/> to the <see cref="AvailableShapingEntries"/> collection should be enabled
        /// </summary>
        public bool MoveShapedByEnabled { get; private set; }

        /// <summary>
        /// List containing the nested types for generic properties
        /// </summary>
        public List<Type[]> NestedTypeList { get; }

        /// <summary>
        /// The <see cref="ShapingEntry"/> that is selected in the <see cref="AvailableShapingEntries"/> collection
        /// </summary>
        public ShapingEntry? SelectedAvailableEntry
        {
            get => _selectedAvailableEntry;
            set
            {
                _selectedAvailableEntry = value;
                MoveAvailableEnabled = value != null;
            }
        }

        /// <summary>
        /// The <see cref="ShapingEntry"/> that is selected in the <see cref="EntriesShapedBy"/> collection
        /// </summary>
        public ShapingEntry? SelectedShapedByEntry
        {
            get => _selectedShapingByEntry;
            set
            {
                _selectedShapingByEntry = value;
                MoveShapedByEnabled = value != null;
            }
        }

        #endregion

        #region Variables

        /// <summary>
        /// ViewSource to shape
        /// </summary>
        protected readonly IAdvancedCollectionView ViewSource;

        /// <summary>
        /// Backing variable for the <see cref="SelectedAvailableEntry"/> property
        /// </summary>
        private ShapingEntry? _selectedAvailableEntry;

        /// <summary>
        /// Backing variable for the <see cref="SelectedShapedByEntry"/>
        /// </summary>
        private ShapingEntry? _selectedShapingByEntry;

        /// <summary>
        /// Property to use for default sorting of the collection
        /// </summary>
        protected string? DefaultShapingProperty;

        #endregion
    }
}