// --------------------------------------------------------------------------------------------------
//  <copyright file="BookFileHandlerTestFixture.cs" company="RHEA System S.A.">
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

namespace DocuPanel.Tests.Models
{
    using DocuPanel.Models;
    using NUnit.Framework;
    using System.IO;
    using Newtonsoft.Json;
    using DeepEqual.Syntax;
    using System.Collections.Generic;

    /// <summary>
    /// Suite of tests for the <see cref="BookFileHandler"/> class.
    /// </summary>
    [TestFixture]
    public class BookFileHandlerTestFixture
    {
        private string BookPath { get; set; }
        private Book book;
        private string BookTitle { get; set; }
        private BookFileHandler bookFileHandler;

        [SetUp]
        public void SetUp()
        {
            this.BookPath = Path.Combine(TestContext.CurrentContext.TestDirectory, @"Documentation\book.json");
            var listSections = new List<Section>();

            // Creation of the book
            this.BookTitle = "Documentation";
            var pagePath = "index.md";
            
            // Creation of the first section of the book and its content
            Section section = new Section();
            section.Name = "Introduction";
            section.PagePath = "1_intro_Introduction.md";
            listSections.Add(section);

            // Creation of the second section of the book and its content
            section = new Section();
            section.Name = "Quick Start";
            section.PagePath = "2_quickstart_Quick_Start.md";
            listSections.Add(section);

            /* Creation of the third section of the book and its content
             * Installation
             *  -> Introduction
             *  -> Configuration
             *      -> Step by Step
             *      -> User Properties
            */
            section = new Section();
            section.Name = "Installation";

            Section subSection = new Section();
            subSection.Name = "Introduction";
            subSection.PagePath = @"Installation\1_installintro_Introduction.md";
            section.Sections.Add(subSection);

            subSection = new Section();
            subSection.Name = "Configuration";
            Section subSubSection = new Section();
            subSubSection.Name = "Step by Step";
            subSubSection.PagePath = @"Installation\2_installstep_Step_by_Step.md";
            subSection.Sections.Add(subSubSection);
            subSubSection = new Section();
            subSubSection.Name = "User Properties";
            subSubSection.PagePath = "3_prop_User_Properties.md";
            subSection.Sections.Add(subSubSection);
            section.Sections.Add(subSection);

            listSections.Add(section);

            this.book = new Book(this.BookTitle, pagePath, "RHEA System S.A.", listSections);

            // Serialization of the book object to a Json file
            using (StreamWriter file = File.CreateText(this.BookPath))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Formatting = Formatting.Indented;
                serializer.NullValueHandling = NullValueHandling.Ignore;
                serializer.Serialize(file, this.book);
            }

            this.bookFileHandler = new BookFileHandler();
        }

        /// <summary>
        /// Check if a Book object is created from the Book Json file and, verify that its content is the same than the book object we created in the SetUp function.
        /// </summary>
        [Test]
        public void VerifyThatDeserializeWorks()
        {
            var bookDeserialized = this.bookFileHandler.DeserializeJsonBook(this.BookPath);

            var result = this.book.IsDeepEqual(bookDeserialized);
            Assert.That(result, Is.True);
        }

        [Test]
        public void VerifyThatInvalidBookPathThrowsException()
        {
            Assert.Throws<FileNotFoundException>(() => this.bookFileHandler.DeserializeJsonBook("RHEA"));
        }

        [Test]
        public void VerifyThatInvalidExtensionThrowsException()
        {
            var pathWithBadExtension = Path.Combine(TestContext.CurrentContext.TestDirectory, @"Documentation\index.md");
            Assert.Throws<FileFormatException>(() => this.bookFileHandler.DeserializeJsonBook(pathWithBadExtension));
        }
    }
}
