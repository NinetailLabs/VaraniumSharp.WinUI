using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VaraniumSharp.WinUI.ExtensionMethods
{
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
                    if (objData != null)
                    {
                        objData = property.GetValue(objData);
                    }
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
                        resultDictionary.Add(propertyName, RetrievePropertyInfo(type, propertyName));
                    }
                    else
                    {
                      resultDictionary.Add(propertyName, RetrieveNestedPropertyInfo(type, propertyName));
                    }
                }
            }

            return resultDictionary;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Retrieve the property info for a nested property
        /// </summary>
        /// <param name="type">The type for which property info should be retrieved</param>
        /// <param name="propertyName">The name of the property to retrieve</param>
        /// <exception cref="InvalidOperationException">Thrown if the property could not be found</exception>
        private static PropertyInfo RetrieveNestedPropertyInfo(Type type, string propertyName)
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
                    return property;
                }
                else
                {
                    typeToUse = property.PropertyType;
                }
            }

            // We should never reach here but there is no other simple way to do this
            throw new InvalidOperationException($"Could not find {propertyName} for {typeToUse.Name}");
        }

        /// <summary>
        /// Retrieve the property info for a top level property
        /// </summary>
        /// <param name="type">The type for which property info should be retrieved</param>
        /// <param name="propertyName">The name of the property to retrieve</param>
        /// <exception cref="InvalidOperationException">Thrown if the property could not be found</exception>
        private static PropertyInfo RetrievePropertyInfo(Type type, string propertyName)
        {
            var propType = type.GetProperty(propertyName);
            if (propType == null)
            {
                throw new InvalidOperationException($"{type.Name} does not have a property called {propertyName}.");
            }

            return propType;
        }

        #endregion
    }
}