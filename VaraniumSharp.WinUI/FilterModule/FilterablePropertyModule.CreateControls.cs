using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using VaraniumSharp.WinUI.FilterModule.Controls;

namespace VaraniumSharp.WinUI.FilterModule
{
    /// <summary>
    /// Contains the methods used to create the controls
    /// </summary>
    public sealed partial class FilterablePropertyModule
    {
        #region Private Methods

        /// <summary>
        /// Create a boolean filter control and add it to the collection
        /// </summary>
        /// <param name="fullPropertyName">Full name of the property to filter on</param>
        /// <param name="attribute">Attribute used to get filter values</param>
        /// <param name="property">Property the filter is for</param>
        [FilterableControlCreation(FilterableType.Boolean)]
        [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Method is used via Reflection")]
        [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification = "Property is required for signature to match Action")]
        private void AddBooleanFilterControl(string fullPropertyName, FilterablePropertyAttribute attribute, PropertyInfo property)
        {
            if (!FilterAlreadyExists(attribute.FilterDisplayName))
            {

                var shapingEntry = new FilterShapingEntry(string.Empty)
                {
                    PropertyName = fullPropertyName,
                    Header = attribute.FilterDisplayName,
                    Tooltip = attribute.ToolTip
                };

                var control = new DropDownBoolFilter(shapingEntry);
                HookupFilterControl(control);
                
            }
        }

        #endregion
    }
}