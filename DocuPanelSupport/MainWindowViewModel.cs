// --------------------------------------------------------------------------------------------------
//  <copyright file="MainWindowViewModel.cs" company="RHEA System S.A.">
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

namespace DocuPanelSupport
{
    using System;
    using System.IO;
    using ReactiveUI;
    using System.Reflection;

    /// <summary>
    /// The view-model for the main window.
    /// </summary>
    public class MainWindowViewModel : ReactiveObject
    {
        /// <summary>
        /// Backing field for <see cref="PathDocumentationIndex"/>.
        /// </summary>
        private string pathDocumentationIndex;

        /// <summary>
        /// Backing field for <see cref="AppDataRoot"/>.
        /// </summary>
        private string appDataRoot;
        
        /// <summary>
        /// Path to the assembly.
        /// </summary>
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        /// <summary>
        /// Gets or sets the root directory of the documentation.
        /// </summary>
        public string PathDocumentationIndex
        {
            get { return this.pathDocumentationIndex; }
            set { this.RaiseAndSetIfChanged(ref this.pathDocumentationIndex, value); }
        }

        /// <summary>
        /// Gets or sets the path of the application data folder.
        /// </summary>
        public string AppDataRoot
        {
            get { return this.appDataRoot; }
            set { this.RaiseAndSetIfChanged(ref this.appDataRoot, value); }
        }

        /// <summary>
        /// Gets or sets the property which indicates whether the indexation of the documentation needs to be updated.
        /// </summary>
        public bool UpdateIndexation { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        public MainWindowViewModel()
        {
            this.SetProperties();
        }

        /// <summary>
        /// Sets the properties of this view-model.
        /// </summary>
        private void SetProperties()
        {
            this.PathDocumentationIndex = Path.Combine(AssemblyDirectory, @"Documentation\book.json");
            this.AppDataRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RHEA_Group");
            this.UpdateIndexation = true;
        }
    }
}
