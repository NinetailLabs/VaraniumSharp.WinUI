﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using VaraniumSharp.Attributes;
using VaraniumSharp.WinUI.Interfaces.Pickers;

namespace VaraniumSharp.WinUI.Pickers
{
    /// <summary>
    /// Wrapper class for Pickers
    /// </summary>
    [AutomaticContainerRegistration(typeof(IPickerWrapper))]
    public class PickerWrapper : IPickerWrapper
    {
        #region Constructor

        /// <summary>
        /// DI Constructor
        /// </summary>
        public PickerWrapper(IOwnerWindow ownerWindow)
        {
            _ownerWindow = ownerWindow;
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public async Task<StorageFolder?> PickFolderAsync()
        {
            var directoryPicker = new FolderPicker();

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(_ownerWindow.ParentWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(directoryPicker, hwnd);

            return await directoryPicker.PickSingleFolderAsync();
        }

        /// <inheritdoc />
        public async Task<StorageFile?> PickSaveFileAsync(KeyValuePair<string, List<string>> fileTypes, string? suggestedFilename)
        {
            var savePicker = new FileSavePicker();
            savePicker.FileTypeChoices.Add(fileTypes.Key, fileTypes.Value);

            if (!string.IsNullOrEmpty(suggestedFilename))
            {
                savePicker.SuggestedFileName = suggestedFilename;
            }

            // See: https://github.com/microsoft/WindowsAppSDK/issues/467#issuecomment-901220636
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(_ownerWindow.ParentWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);

            return await savePicker.PickSaveFileAsync();
        }

        /// <inheritdoc />
        public async Task<StorageFile?> PickSingleFileToOpenAsync(List<string> fileExtensionsToPick)
        {
            var openPicker = new FileOpenPicker();
            foreach (var fileExtension in fileExtensionsToPick)
            {
                openPicker.FileTypeFilter.Add(fileExtension);
            }

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(_ownerWindow.ParentWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hwnd);

            return await openPicker.PickSingleFileAsync();
        }

        #endregion

        #region Variables

        /// <summary>
        /// OwnerWindow instance
        /// </summary>
        private readonly IOwnerWindow _ownerWindow;

        #endregion
    }
}