using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using VaraniumSharp.Attributes;
using VaraniumSharp.WinUI.Interfaces.SettingPane;

namespace VaraniumSharp.WinUI.SettingPane
{
    /// <summary>
    /// Context for the setting pane
    /// </summary>
    [AutomaticContainerRegistration(typeof(ISettingPaneContext))]
    public class SettingPaneContext : ISettingPaneContext, IAsyncDisposable
    {
        #region Constructor

        /// <summary>
        /// DI Constructor
        /// </summary>
        public SettingPaneContext(IEnumerable<ISettingControl> settingControls)
        {
            SettingCategories = new();
            SettingControls = new();
            _controlDictionary = new();

            var orderedControls = settingControls
                .OrderBy(x => x.ControlIndex)
                .ThenBy(x => x.ControlTitle);

            var navigationGroups = orderedControls.GroupBy(x => x.NavigationEntryTitle);
            var tempGroup = new Dictionary<SettingCategory, List<ISettingControl>>();
            foreach (var group in navigationGroups)
            {
                var category = new SettingCategory
                {
                    Name = group.Key,
                    Glyph = group.First().NavigationGlyph,
                    Index = group.First().NavigationEntryIndex
                };
                tempGroup.Add(category, group.ToList());
            }

            var orderedGroups = tempGroup
                .OrderBy(x => x.Key.Index)
                .ThenBy(x => x.Key.Name);

            foreach (var item in orderedGroups)
            {
                _controlDictionary.Add(item.Key, item.Value);
                SettingCategories.Add(item.Key);
            }

            SelectedCategory = SettingCategories.FirstOrDefault();
        }

        #endregion

        #region Events

        /// <inheritdoc />
#pragma warning disable CS0067 // Is used via Fody
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067

        #endregion

        #region Properties

        /// <inheritdoc/>
        public SettingCategory? SelectedCategory
        {
            get => _settingCategory;
            set
            {
                _settingCategory = value;
                SettingControls.Clear();
                if (_settingCategory != null)
                {
                    foreach (var item in _controlDictionary[_settingCategory])
                    {
                        SettingControls.Add(item);
                    }
                }
            }
        }

        /// <inheritdoc />
        public ObservableCollection<SettingCategory> SettingCategories { get; }

        /// <inheritdoc />
        public ObservableCollection<ISettingControl> SettingControls { get; }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            SettingControls.Clear();
            foreach (var entry in _controlDictionary.SelectMany(x => x.Value))
            {
                if (entry is IAsyncDisposable disposableEntry)
                {
                    await disposableEntry.DisposeAsync();
                }
            }
        }

        #endregion

        #region Variables

        private readonly Dictionary<SettingCategory, List<ISettingControl>> _controlDictionary;

        /// <summary>
        /// Backing variable for the <see cref="SelectedCategory"/> property
        /// </summary>
        private SettingCategory? _settingCategory;

        #endregion
    }
}