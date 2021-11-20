using Windows.UI.Core;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using VaraniumSharp.WinUI.Interfaces.CustomPaneBase;

namespace VaraniumSharp.WinUI.CustomPaneBase
{
    /// <summary>
    /// Base class that contains logic to help with <see cref="Thumb"/> resizing
    /// </summary>
    public abstract class ThumbBase : UserControl
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="customPaneContext">CustomPane context used for resizing</param>
        /// <param name="cursorType">The type of cursor to display for resizing</param>
        protected ThumbBase(ICustomPaneContext customPaneContext, InputSystemCursorShape cursorType)
        {
            _customPaneContext = customPaneContext;
            _cursorType = cursorType;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Occurs when the user finished dragging the thumb.
        /// This method is used to do a final resize of the controls to ensure nothing was pushed offscreen.
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        protected async void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            await _customPaneContext
                .UpdateChildrenSizeAsync(Width, Height)
                .ConfigureAwait(false);
            _doNotChangePointer = false;
        }

        /// <summary>
        /// Occurs when the user drags a thumb on a control
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        protected abstract void Thumb_DragDelta(object sender, DragDeltaEventArgs e);

        /// <summary>
        /// Occurs when the user starts dragging the thumb
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        protected void Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            _doNotChangePointer = true;
        }

        /// <summary>
        /// Occurs when the pointer enters the Thumb
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        protected void Thumb_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (_doNotChangePointer)
            {
                return;
            }
            ProtectedCursor = InputSystemCursor.Create(_cursorType);
        }

        /// <summary>
        /// Occurs when the pointer leaves the Thumb
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        protected void Thumb_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (_doNotChangePointer)
            {
                return;
            }
            ProtectedCursor = null;
        }

        #endregion

        #region Variables

        /// <summary>
        /// The type of cursor to display for resizing
        /// </summary>
        private readonly InputSystemCursorShape _cursorType;

        /// <summary>
        /// CustomPaneContext instance
        /// </summary>
        private readonly ICustomPaneContext _customPaneContext;

        /// <summary>
        /// Indicate if pointer should not be changed
        /// </summary>
        private bool _doNotChangePointer;

        #endregion
    }
}