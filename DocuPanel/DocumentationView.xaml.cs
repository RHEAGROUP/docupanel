// --------------------------------------------------------------------------------------------------
//  <copyright file="DocumentationView.xaml.cs" company="RHEA System S.A.">
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

namespace DocuPanel
{
    using ViewModels;
    using System.Windows;   

    /// <summary>
    /// Interaction logic for DocuPanelView.xaml
    /// </summary>
    public partial class DocumentationView
    {
        /// <summary>
        /// Registered DependencyProperty for the <see cref="PathDocumentationIndex"/> property.
        /// </summary>
        public static readonly DependencyProperty PathDocumentationIndexProperty = DependencyProperty.RegisterAttached(
            "PathDocumentationIndex",
            typeof(string),
            typeof(DocumentationView),
            new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnRootDocumentationChanged)
        );

        /// <summary>
        /// Registered DependencyProperty for the <see cref="RootAppDataProperty"/> property.
        /// </summary>
        public static readonly DependencyProperty RootAppDataProperty = DependencyProperty.RegisterAttached(
            "RootAppData",
            typeof(string),
            typeof(DocumentationView),
            new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnRootAppDataChanged)
        );

        /// <summary>
        /// Registered DependencyProperty for the <see cref="UpdateIndexationProperty"/> property.
        /// </summary>
        public static readonly DependencyProperty UpdateIndexationProperty = DependencyProperty.RegisterAttached(
            "UpdateIndexation",
            typeof(bool),
            typeof(DocumentationView),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnUpdateIndexationChanged)
            );

        /// <summary>
        /// Gets or Sets the Root directory of the documentation.
        /// </summary>
        public string PathDocumentationIndex
        {
            get { return (string)GetValue(PathDocumentationIndexProperty); }
            set { SetValue(PathDocumentationIndexProperty, value); }
        }

        /// <summary>
        /// Gets or sets the path of the application data folder.
        /// </summary>
        public string RootAppData
        {
            get { return (string) GetValue(RootAppDataProperty); }
            set { SetValue(RootAppDataProperty, value);}
        }

        /// <summary>
        /// Gets or sets the property which indicates whether the indexation of the documentation needs to be updated.
        /// </summary>
        public string UpdateIndexation
        {
            get { return (string)GetValue(UpdateIndexationProperty); }
            set { SetValue(UpdateIndexationProperty, value); }
        }
        
        /// <summary>
        /// Event Handler for the change event of the <see cref="PathDocumentationIndex"/> dependency property.
        /// </summary>
        /// <param name="sender"> The sender.</param>
        /// <param name="eventArgs">The arguments.</param>
        private static void OnRootDocumentationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            var documentationView = (DocumentationView) sender;           
            var viewmodel = (DocumentationViewModel) documentationView.DataContext;
            
            viewmodel.PathDocumentationIndex = eventArgs.NewValue.ToString();                        
        }

        /// <summary>
        /// Event Handler for the change event of the <see cref="RootAppData"/> dependency property.
        /// </summary>
        /// <param name="sender"> The sender.</param>
        /// <param name="eventArgs">The arguments.</param>
        private static void OnRootAppDataChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            var documentationView = (DocumentationView) sender;
            var viewmodel = (DocumentationViewModel) documentationView.DataContext;

            viewmodel.RootAppData = eventArgs.NewValue.ToString();
        }

        private static void OnUpdateIndexationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            var documentationView = (DocumentationView)sender;
            var viewmodel = (DocumentationViewModel)documentationView.DataContext;
            viewmodel.UpdateIndexation = (bool) eventArgs.NewValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentationView"/> class.
        /// </summary>
        public DocumentationView()
        {
            this.InitializeComponent();
            var viewmodel = new DocumentationViewModel(this.DocuBrowser, "", "", false);
            this.DataContext = viewmodel;
        }

        /// <summary>
        /// Event handler for the change of the <see cref="SectionRowViewModel"/> selected .
        /// </summary>
        /// <param name="sender"> The sender.</param>
        /// <param name="e"> The arguments.</param>
        private void TreeView_OnSelectedSectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var viewModel = (DocumentationViewModel) this.DataContext;
            viewModel.SelectedSection = (SectionRowViewModel) e.NewValue;
        }
    }
}