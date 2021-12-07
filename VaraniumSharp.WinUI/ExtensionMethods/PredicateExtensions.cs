using System;

namespace VaraniumSharp.WinUI.ExtensionMethods
{
    /// <summary>
    /// Extension methods for predicates
    /// </summary>
    public static class PredicateExtensions
    {
        #region Public Methods

        /// <summary>
        /// Combines two predicates with &&
        /// </summary>
        /// <typeparam name="T">Type of parameter passed to the predicated</typeparam>
        /// <param name="predicate1">First predicate</param>
        /// <param name="predicate2">Second predicate</param>
        /// <returns>Resulting predicate</returns>
        public static Func<T, bool> AndAlso<T>(this Func<T, bool> predicate1, Func<T, bool> predicate2)
        {
            return arg => predicate1(arg) && predicate2(arg);
        }

        /// <summary>
        /// Combines two predicates with ||
        /// </summary>
        /// <typeparam name="T">Type of parameter passed to the predicated</typeparam>
        /// <param name="predicate1">First predicate</param>
        /// <param name="predicate2">Second predicate</param>
        /// <returns>Resulting predicate</returns>
        public static Func<T, bool> OrElse<T>(this Func<T, bool> predicate1, Func<T, bool> predicate2)
        {
            return arg => predicate1(arg) || predicate2(arg);
        }

        #endregion
    }
}