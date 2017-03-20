// --------------------------------------------------------------------------------------------------
//  <copyright file="ChromiumWebBrowserSearchBehavior.cs" company="RHEA System S.A.">
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
    using System.Windows.Input;
    using System.Windows.Interactivity;
    using System;
    using System.Reactive.Linq;
    using CefSharp;
    using CefSharp.Wpf;
    using ReactiveUI;

    /// <summary>
    /// Represents the <see cref="Behavior"/> adopted by the <see cref="ChromiumWebBrowser"/>.
    /// </summary>
    public class ChromiumWebBrowserSearchBehavior : Behavior<ChromiumWebBrowser>
    {
        private bool isSearchEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChromiumWebBrowserSearchBehavior"/> class.
        /// </summary>
        public ChromiumWebBrowserSearchBehavior()
        {
            var nextCommand = ReactiveCommand.Create();
            nextCommand.ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_  => OnNext());            
            NextCommand = nextCommand;

            var previousCommand = ReactiveCommand.Create();
            previousCommand.ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => OnPrevious());
            PreviousCommand = previousCommand;
        }

        /// <summary>
        /// When the <see cref="NextCommand"/> is performed, the <see cref="ChromiumWebBrowser"/> searchs the next match with the user query.
        /// </summary>
        private void OnNext()
        {
            AssociatedObject.Find(1, SearchText, true, false, true);
        }

        /// <summary>
        /// When the <see cref="PreviousCommand"/> is performed, the <see cref="ChromiumWebBrowser"/> searchs the previous match with the user query.
        /// </summary>
        private void OnPrevious()
        {
            AssociatedObject.Find(1, SearchText, false, false, true);
        }

        /// <summary>
        /// Attaches a Frame to the browser.
        /// </summary>
        protected override void OnAttached()
        {
            AssociatedObject.FrameLoadEnd += ChromiumWebBrowserOnFrameLoadEnd;
        }

        /// <summary>
        /// Loads a Frame in the browser.
        /// </summary>
        /// <param name="sender"> The sender.</param>
        /// <param name="frameLoadEndEventArgs"> The arguments.</param>
        private void ChromiumWebBrowserOnFrameLoadEnd(object sender, FrameLoadEndEventArgs frameLoadEndEventArgs)
        {
            isSearchEnabled = frameLoadEndEventArgs.Frame.IsMain;

            Dispatcher.Invoke(() =>
            {
                if (isSearchEnabled && !string.IsNullOrEmpty(SearchText))
                {
                    AssociatedObject.Find(1, SearchText, true, false, false);
                }
            });
        }

        /// <summary>
        /// Registered DependencyProperty for the <see cref="SearchTextProperty"/> property.
        /// </summary>
        public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register(
            "SearchText", typeof(string), typeof(ChromiumWebBrowserSearchBehavior), new PropertyMetadata(default(string), OnSearchTextChanged));

        /// <summary>
        /// Gets or sets the text of the search.
        /// </summary>
        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }

        /// <summary>
        /// Registered DependencyProperty for the <see cref="NextCommandProperty"/> property.
        /// </summary>
        public static readonly DependencyProperty NextCommandProperty = DependencyProperty.Register(
            "NextCommand", typeof(ICommand), typeof(ChromiumWebBrowserSearchBehavior), new PropertyMetadata(default(ICommand)));

        /// <summary>
        /// Gets or sets the command which allows to go to the next search match.
        /// </summary>
        public ICommand NextCommand
        {
            get { return (ICommand)GetValue(NextCommandProperty); }
            set { SetValue(NextCommandProperty, value); }
        }

        /// <summary>
        /// Registered DependencyProperty for the <see cref="PreviousCommandProperty"/> property.
        /// </summary>
        public static readonly DependencyProperty PreviousCommandProperty = DependencyProperty.Register(
            "PreviousCommand", typeof(ICommand), typeof(ChromiumWebBrowserSearchBehavior), new PropertyMetadata(default(ICommand)));


        /// <summary>
        /// Gets or sets the command which allows to go to the previous search match.
        /// </summary>
        public ICommand PreviousCommand
        {
            get { return (ICommand)GetValue(PreviousCommandProperty); }
            set { SetValue(PreviousCommandProperty, value); }
        }

        /// <summary>
        /// Event Handler for the change event of the <see cref="SearchTextProperty"/> dependency property
        /// When the search text changes, the actual search is stopped and a new one is created.
        /// </summary>
        /// <param name="dependencyObject"> The sender.</param>
        /// <param name="dependencyPropertyChangedEventArgs"> The arguments.</param>
        private static void OnSearchTextChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var behavior = dependencyObject as ChromiumWebBrowserSearchBehavior;

            if (behavior != null && behavior.isSearchEnabled)
            {
                var newSearchText = dependencyPropertyChangedEventArgs.NewValue as string;

                if (string.IsNullOrEmpty(newSearchText))
                {
                    behavior.AssociatedObject.StopFinding(true);
                }
                else
                {
                    behavior.AssociatedObject.Find(1, newSearchText, true, false, false);
                }
            }
        }

        /// <summary>
        /// Detaches a frame from the browser.
        /// </summary>
        protected override void OnDetaching()
        {
            AssociatedObject.FrameLoadEnd -= ChromiumWebBrowserOnFrameLoadEnd;
        }
    }
}