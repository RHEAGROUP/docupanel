// --------------------------------------------------------------------------------------------------
//  <copyright file="BookFileHandler.cs" company="RHEA System S.A.">
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
    using System.IO;
    using Newtonsoft.Json;
    using System;

    /// <summary>
    /// Reads the Json index file of the documentation to create <see cref="Book"/> object.   
    /// </summary>
    public class BookFileHandler
    {
        /// <summary>
        /// Reads the Json book file and return the <see cref="Book"/> object associated to the documentation.
        /// </summary>
        /// <param name="pathDocumentationIndex"> The path where is located the root of the documentation.</param>
        /// <returns>The <see cref="Book"/> object associated to the documentation.</returns>
        public Book DeserializeJsonBook(string pathDocumentationIndex)
        {
            if (!File.Exists(pathDocumentationIndex))
            {
                throw new FileNotFoundException(
                    string.Format("the index file failed to load. The file at the path '{0}' was not found", pathDocumentationIndex));
            }

            if (Path.GetExtension(pathDocumentationIndex) != ".json")
            {
                throw new FileFormatException("the index file must be of type 'json'.");
            }

            var book = JsonConvert.DeserializeObject<Book>(File.ReadAllText(pathDocumentationIndex));

            if (book == null)
            {
                throw new NullReferenceException("An error occured during the creation of the book, the book has not been created");
            }

            return book;
        }
    }
}
