using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VaraniumSharp.WinUI.ExtensionMethods
{
    // TODO - Unit test
    /// <summary>
    /// Extension methods for <see cref="object"/>
    /// </summary>
    public static class ObjectExtensionMethods
    {
        #region Public Methods

        /// <summary>
        /// Retrieve the property value when it is nested
        /// </summary>
        /// <param name="obj">Object from which the value should be resolved</param>
        /// <param name="propertyName">Full name of the property</param>
        /// <returns>Value of the requested property</returns>
        public static object? GetNestedPropertyValue(this object obj, string propertyName)
        {
            var path = propertyName.Split(".");
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

        /// <summary>
        /// Retrieve <see cref="PropertyInfo"/> entries for the object.
        /// This method will also retrieve nested properties.
        /// </summary>
        /// <param name="obj">Object for which properties should be retrieved</param>
        /// <param name="propertyNames">The name of the properties to retrieve. Names for nested entries should be separated with a .</param>
        /// <returns>Dictionary containing the <see cref="PropertyInfo"/> for each property name</returns>
        /// <exception cref="InvalidOperationException">Thrown if a property cannot be found</exception>
        public static Dictionary<string, PropertyInfo> GetPropertyInfo(this object obj, IEnumerable<string> propertyNames)
        {
            var type = obj.GetType();
            var resultDictionary = new Dictionary<string, PropertyInfo>();

            foreach (var propertyName in propertyNames)
            {
                if (!string.IsNullOrEmpty(propertyName))
                {
                    if (!propertyName.Contains("."))
                    {
                        var propType = type.GetProperty(propertyName);
                        if (propType == null)
                        {
                            throw new InvalidOperationException($"{type.Name} does not have a property called {propertyName}.");
                        }

                        resultDictionary.Add(propertyName, propType);
                    }
                    else
                    {
                        var path = propertyName.Split(".");
                        var typeToUse = type;
                        for (var r = 0; r < path.Length; r++)
                        {
                            var property = typeToUse
                                .GetProperties()
                                .FirstOrDefault(z => z.Name == path[r]);

                            if (property == null)
                            {
                                throw new InvalidOperationException($"{typeToUse.Name} does not have a property called {path[r]}. Full requested path is {propertyName} and type is {type.Name}");
                            }

                            if (r == path.Length - 1)
                            {
                                resultDictionary.Add(propertyName, property);
                            }
                            else
                            {
                                typeToUse = property.PropertyType;
                            }
                        }
                    }
                }
            }

            return resultDictionary;
        }

        #endregion
    }
}