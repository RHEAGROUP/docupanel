// --------------------------------------------------------------------------------------------------
//  <copyright file="AutoRepositionPopupBehavior.cs" company="RHEA System S.A.">
// 
//  Copyright 2017 RHEA System S.A.
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// 
//  </copyright>
// --------------------------------------------------------------------------------------------------

namespace DocuPanel.Resources
{
    using System;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Interactivity;

    /// <summary>
    /// This class allows a popup to be drawn at the right position.
    /// </summary>
    public class AutoRepositionPopupBehavior : Behavior<Popup>
    {
        /// <summary>
        /// Indicates if the <see cref="Behavior"/> is attached to the element. 
        /// </summary>
        private bool eventsAttached = false;

        /// <summary>
        /// Attaches the <see cref="Behavior"/> to the element when the popup is opened.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Opened += this.AssociatedObject_Opened;
        }

        /// <summary>
        /// Detaches the <see cref="Behavior"/> to the element when the popup is closed.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.Opened -= this.AssociatedObject_Opened;
        }

        /// <summary>
        /// Attaches the methods <see cref="Window_LocationChanged"/> and <see cref="Window_SizeChanged"/> to the <see cref="Window"/> changes.
        /// </summary>
        /// <param name="sender"> The sender.</param>
        /// <param name="e"> The arguments.</param>
        private void AssociatedObject_Opened(object sender, EventArgs e)
        {
            if (this.eventsAttached)
            {
                return;
            }

            var window = Window.GetWindow(this.AssociatedObject);

            if (window == null)
            {
                return;
            }

            window.LocationChanged += this.Window_LocationChanged;
            window.SizeChanged += this.Window_SizeChanged;
            this.eventsAttached = true;
        }

        /// <summary>
        /// When the location of the <see cref="Window"/> changes, the position of the <see cref="Popup"/> changes.
        /// </summary>
        /// <param name="sender"> The sender.</param>
        /// <param name="e"> The arguments.</param>
        private void Window_LocationChanged(object sender, EventArgs e)
        {
            this.ResetHorizontalOffset();
        }

        /// <summary>
        /// When the siwe of the <see cref="Window"/> changes, the position of the <see cref="Popup"/> changes.
        /// </summary>
        /// <param name="sender"> The sender.</param>
        /// <param name="e"> The arguments.</param>
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.ResetHorizontalOffset();
        }

        /// <summary>
        /// Resets the horizontal offset of the object associated.
        /// </summary>
        private void ResetHorizontalOffset()
        {
            this.AssociatedObject.HorizontalOffset += 1;
            this.AssociatedObject.HorizontalOffset -= 1;
        }
    }
}
