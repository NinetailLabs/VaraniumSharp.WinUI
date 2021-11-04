// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Contains the additional logic used to handle nested property requests.
// Nested properties are in the format PropertyName.NestedProperty and can be of any depth.

using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.WinUI.UI;

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

            if (!_sortProperties.Any())
            {
                var type = x.GetType();
                foreach (var sd in _sortDescriptions)
                {
                    if (!string.IsNullOrEmpty(sd.PropertyName))
                    {
                        if (!sd.PropertyName.Contains("."))
                        {
                            var propType = type.GetProperty(sd.PropertyName);
                            if (propType == null)
                            {
                                throw new InvalidOperationException($"{type.Name} does not have a property called {sd.PropertyName}.");
                            }

                            _sortProperties[sd.PropertyName] = propType;
                        }
                        else
                        {
                            var path = sd.PropertyName.Split(".");
                            var typeToUse = type;
                            for (var r = 0; r < path.Length; r++)
                            {
                                var property = typeToUse
                                    .GetProperties()
                                    .FirstOrDefault(z => z.Name == path[r]);

                                if (property == null)
                                {
                                    throw new InvalidOperationException($"{typeToUse.Name} does not have a property called {path[r]}. Full requested path is {sd.PropertyName} and type is {type.Name}");
                                }

                                if (r == path.Length - 1)
                                {
                                    _sortProperties[sd.PropertyName] = property;
                                }
                                else
                                {
                                    typeToUse = property.PropertyType;
                                }
                            }
                        }
                    }
                }
            }

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
                        cx = GetNestedPropertyValue(sd, x);
                        cy = GetNestedPropertyValue(sd, y);
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
        /// Retrieve the property value when it is nested
        /// </summary>
        /// <param name="sd">SortDescription for the entry</param>
        /// <param name="obj">Object from which the value should be resolved</param>
        /// <returns>Value of the requested property</returns>
        private object? GetNestedPropertyValue(SortDescription sd, object obj)
        {
            var path = sd.PropertyName.Split(".");
            var typeToUse = obj.GetType();
            var objData = obj;
            for (var r = 0; r < path.Length; r++)
            {
                var property = typeToUse
                    .GetProperties()
                    .First(z => z.Name == path[r]);

                if (r < path.Length - 1)
                {

                    objData = property.GetValue(objData);
                    typeToUse = property.PropertyType;
                }
                else
                {
                    objData = property.GetValue(objData);
                }
            }

            return objData;
        }

        #endregion
    }
}