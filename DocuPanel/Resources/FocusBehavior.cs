// --------------------------------------------------------------------------------------------------
//  <copyright file="FocusBehavior.cs" company="RHEA System S.A.">
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
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// This class manages the behavior of the focus. 
    /// </summary>
    public static class FocusBehavior
    {
        /// <summary>
        /// Registered <see cref="DependencyProperty"/> for the <see cref="IsFocusProperty"/> property.
        /// </summary>
        public static readonly DependencyProperty IsFocusProperty = DependencyProperty.RegisterAttached(
            "IsFocus", typeof(bool), typeof(FocusBehavior), new UIPropertyMetadata(false, OnFocusChanged));

        /// <summary>
        /// Sets the focus of an element of the view.
        /// </summary>
        /// <param name="depObject"> The object element of the view.</param>
        /// <param name="value"> The value which indicates if the focus has to be set or not.</param>
        public static void SetIsFocus(DependencyObject depObject, bool value)
        {
            depObject.SetValue(IsFocusProperty, value);
        }

        /// <summary>
        /// Gets the state of the focus of an element of the view.
        /// </summary>
        /// <param name="depObject"> The object element.</param>
        /// <returns> The state of the focus for this element.</returns>
        public static bool GetIsFocus(DependencyObject depObject)
        {
            return (bool)depObject.GetValue(IsFocusProperty);
        }

        /// <summary>
        /// Event Handler for the change event of the <see cref="IsFocusProperty"/> dependency property.
        /// </summary>
        /// <param name="sender"> The sender.</param>
        /// <param name="e"> The arguments.</param>
        private static void OnFocusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = sender as Control;

            if (element == null)
            {
                return;
            }

            if (e.NewValue is bool && (bool)e.NewValue == true)
                {
                    element.Loaded += ElementLoaded;
                }
        }

        /// <summary>
        /// Put the focus on the sender.
        /// </summary>
        /// <param name="sender"> The sender.</param>
        /// <param name="e"> The arguments.</param>
        private static void ElementLoaded(object sender, RoutedEventArgs e)
        {
            var control = sender as Control;

            if (control != null)
            {
                if (control is TextBox)
                {
                    Keyboard.Focus(control);
                }
                else
                {
                    control.Focus();
                }
            }
        }
    }
}
