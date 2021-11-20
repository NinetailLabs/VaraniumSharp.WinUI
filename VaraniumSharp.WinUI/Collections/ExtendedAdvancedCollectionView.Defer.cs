// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace VaraniumSharp.WinUI.Collections
{
    /// <summary>
    /// A collection view implementation that supports filtering, grouping, sorting and incremental loading
    /// </summary>
    public partial class ExtendedAdvancedCollectionView
    {
        #region Public Methods

        /// <summary>
        /// Stops refreshing until it is disposed
        /// </summary>
        /// <returns>An disposable object</returns>
        public IDisposable DeferRefresh()
        {
            return new NotificationDeferrer(this);
        }

        #endregion

        /// <summary>
        /// Notification deferrer helper class
        /// </summary>
        public sealed class NotificationDeferrer : IDisposable
        {
            #region Constructor

            /// <summary>
            /// Initializes a new instance of the <see cref="NotificationDeferrer"/> class.
            /// </summary>
            /// <param name="acvs">Source ACVS</param>
            public NotificationDeferrer(ExtendedAdvancedCollectionView acvs)
            {
                _acvs = acvs;
                _currentItem = _acvs.CurrentItem;
                _acvs._deferCounter++;
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            /// <filterpriority>2</filterpriority>
            public void Dispose()
            {
                _acvs.MoveCurrentTo(_currentItem);
                _acvs._deferCounter--;
                _acvs.Refresh();
            }

            #endregion

            #region Variables

            private readonly ExtendedAdvancedCollectionView _acvs;

            private readonly object? _currentItem;

            #endregion
        }
    }
}