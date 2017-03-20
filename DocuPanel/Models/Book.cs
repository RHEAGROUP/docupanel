// --------------------------------------------------------------------------------------------------
//  <copyright file="Book.cs" company="RHEA System S.A.">
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

namespace DocuPanel.Models
{
    using System.Collections.Generic;
    using ReactiveUI;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the documentation book, which contains information about the documentation.
    /// </summary>
    /// The DataContract is useful for the tests, to not include the attributes of ReactiveObject into the serialization of the Json file. 
    [DataContract]
    public class Book : ReactiveObject
    {
        /// <summary>
        /// Backing field for <see cref="PagePath"/>.
        /// </summary>
        private string pagePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="Book"/> class.
        /// </summary>
        /// <param name="title"> The title of the documentation.</param>
        /// <param name="pagePath">The path of the home page of the documentation.</param>
        /// <param name="author">The author of the documentation.</param>
        /// <param name="sections">The first sections of the documentation. These sections don't have a parent section.</param>
        public Book(string title, string pagePath, string author, List<Section> sections )
        {
            this.Title = title;
            this.PagePath = pagePath;
            this.Author = author;
            this.Sections = sections;
        }

        /// <summary>
        /// Gets or sets the title of the documentation.
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the author of the documentation.
        /// </summary>
        [DataMember]
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the main page of the documentation.
        /// </summary>
        [DataMember]
        public string PagePath
        {
            get { return this.pagePath; }
            set { this.RaiseAndSetIfChanged(ref this.pagePath, value); }
        }

        /// <summary>
        /// Gets or sets the sections of type <see cref="Section"/> of the documentation.
        /// </summary>
        [DataMember]
        public List<Section> Sections { get; set; }
    }
}