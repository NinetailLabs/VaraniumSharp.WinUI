using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml.Data;

namespace VaraniumSharp.WinUI.SortModule
{
    /// <summary>
    /// Assist with sorting a <see cref="ICollectionView"/> based on <see cref="SortablePropertyAttribute"/>s
    /// </summary>
    public partial class SortablePropertyModule : INotifyPropertyChanged
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="viewSourceToSort">Advance Collection view to sort. Note the collection should have been constructed with isLiveShaping set to true</param>
        public SortablePropertyModule(IAdvancedCollectionView viewSourceToSort)
        {
            _viewSourceToSort = viewSourceToSort;
            AvailableSortEntries = new();
            EntriesSortedBy = new();
            EntriesSortedBy.CollectionChanged += EntriesSortedByOnCollectionChanged;
            NestedTypeList = new();
        }

        #endregion

        #region Events

        /// <inheritdoc/>
#pragma warning disable CS0067 // Is used via Fody
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067

        #endregion

        #region Properties

        /// <summary>
        /// Collection containing entries that are not currently being sorted by
        /// </summary>
        public ObservableCollection<SortOrderEntry> AvailableSortEntries { get; }

        /// <summary>
        /// Indicate if default sorting should be done on the collection when the sort entries are generated
        /// </summary>
        public bool DisableDefaultSort { get; set; }

        /// <summary>
        /// Collection containing entries that are currently sorted by
        /// </summary>
        public ObservableCollection<SortOrderEntry> EntriesSortedBy { get; }

        /// <summary>
        /// List containing the nested types for generic properties
        /// </summary>
        public List<Type[]> NestedTypeList { get; }

        #endregion

        #region Variables

        /// <summary>
        /// ViewSource to sort
        /// </summary>
        private readonly IAdvancedCollectionView _viewSourceToSort;

        /// <summary>
        /// Property to use for default sorting of the collection
        /// </summary>
        private string? _defaultSortProperty;

        #endregion
    }
}