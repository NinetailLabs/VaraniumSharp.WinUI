// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Contains the additional logic used to handle nested property requests.
// Nested properties are in the format PropertyName.NestedProperty and can be of any depth.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.WinUI.UI;
using VaraniumSharp.WinUI.ExtensionMethods;

namespace VaraniumSharp.WinUI.Collections
{
    public partial class ExtendedAdvancedCollectionView
    {
        #region Private Methods

        /// <summary>
        /// IComparer implementation
        /// </summary>
        /// <param name="x">Object A</param>
        /// <param name="y">Object B</param>
        /// <returns>Comparison value</returns>
        int IComparer<object>.Compare(object? x, object? y)
        {
            if (x == null || y == null)
            {
                return 0;
            }

            GetSortProperties(x);

            foreach (var sd in _sortDescriptions)
            {
                object? cx, cy;

                if (string.IsNullOrEmpty(sd.PropertyName))
                {
                    cx = x;
                    cy = y;
                }
                else
                {
                    var pi = _sortProperties[sd.PropertyName];

                    if (!sd.PropertyName.Contains("."))
                    {

                        cx = pi.GetValue(x);
                        cy = pi.GetValue(y);
                    }
                    else
                    {
                        cx = x.GetNestedPropertyValue(sd.PropertyName);
                        cy = y.GetNestedPropertyValue(sd.PropertyName);
                    }
                }

                var cmp = sd.Comparer.Compare(cx, cy);

                if (cmp != 0)
                {
                    return sd.Direction == SortDirection.Ascending ? +cmp : -cmp;
                }
            }

            return 0;
        }

        /// <summary>
        /// Retrieve the sort properties for an object
        /// </summary>
        /// <param name="x">The object for which sort properties should be retrieved</param>
        private void GetSortProperties(object x)
        {
            if (_sortProperties.Any())
            {
                return;
            }

            var result = x.GetPropertyInfo(_sortDescriptions.Select(x => x.PropertyName));
            foreach (var (key, value) in result)
            {
                _sortProperties[key] = value;
            }
        }

        #endregion
    }
}