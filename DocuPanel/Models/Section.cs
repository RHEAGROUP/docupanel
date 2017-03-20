// --------------------------------------------------------------------------------------------------
//  <copyright file="Section.cs" company="RHEA System S.A.">
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

    /// <summary>
    /// Represents a documentation section. A section is composed by subsections or one page.
    /// </summary>
    public class Section
    {
        /// <summary>
        /// Gets or sets the name of the document.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="List{T}"/> of <see cref="Section"/>s.
        /// </summary>
        public List<Section> Sections { get; set; }

        /// <summary>
        /// Gets or sets the path of the potential markdown file associated to this section.
        /// </summary>
        public string PagePath { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Section"/> class.
        /// </summary>
        public Section()
        {
            this.Sections = new List<Section>();
        }
    }
}