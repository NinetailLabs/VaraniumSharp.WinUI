using System.Collections.Specialized;
using System.Linq;
using VaraniumSharp.WinUI.Collections;
using VaraniumSharp.WinUI.ExtensionMethods;
using VaraniumSharp.WinUI.Shared.ShapingModule;

namespace VaraniumSharp.WinUI.GroupModule
{
    // TODO - Unit test
    /// <summary>
    /// Assists with grouping a <see cref="GroupingAdvancedCollectionView"/>
    /// </summary>
    public class GroupingPropertyModule : ShapingPropertyModuleBase<GroupingPropertyAttribute>
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="viewSource">The collection to group.</param>
        public GroupingPropertyModule(GroupingAdvancedCollectionView viewSource) 
            : base(viewSource)
        { }

        #endregion

        #region Private Methods

        /// <inheritdoc />
        protected override ShapingEntry? CreateShapingEntry(string propertyName, ShapingPropertyAttributeBase attribute)
        {
            if (attribute is GroupingPropertyAttribute)
            {
                return new GroupShapingEntry
                {
                    PropertyName = propertyName,
                    Header = attribute.Header,
                    Tooltip = attribute.ToolTip
                };
            }

            return null;
        }

        /// <inheritdoc />
        protected override void EntriesShapedByOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Shape("");
            IsShaped = EntriesShapedBy.Count > 0;
        }

        /// <summary>
        /// Method used to get the group that an entry belongs to
        /// </summary>
        /// <param name="obj">Object that group should be found for</param>
        /// <returns>Name of the group for the object</returns>
        private object Group(object obj)
        {
            var groupProperties = obj.GetPropertyInfo(EntriesShapedBy.Select(x => x.PropertyName));

            var groupNames = new string[groupProperties.Count];
            var counter = 0;
            foreach (var (key, pi) in groupProperties)
            {
                if (!key.Contains("."))
                {
                    groupNames[counter] = pi.GetValue(obj)?.ToString() ?? string.Empty;
                }
                else
                {
                    groupNames[counter] = obj.GetNestedPropertyValue(key)?.ToString() ?? string.Empty;
                }

                counter++;
            }

            return string.Join(", ", groupNames);
        }

        /// <inheritdoc />
        protected override void Shape(string propertyName)
        {
            var groupSource = (GroupingAdvancedCollectionView) ViewSource;
            groupSource.Group = null;
            groupSource.Group = Group;
        }

        #endregion
    }
}