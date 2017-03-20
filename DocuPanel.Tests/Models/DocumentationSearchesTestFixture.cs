// --------------------------------------------------------------------------------------------------
//  <copyright file="DocumentationSearchesTestFixture.cs" company="RHEA System S.A.">
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
    using System;
    using System.IO;
    using System.Linq;
    using DocuPanel.Models;
    using Lucene.Net.Analysis.Standard;
    using Lucene.Net.Index;
    using Lucene.Net.Search;
    using Lucene.Net.Store;
    using NUnit.Framework;
    using QueryParser = Lucene.Net.QueryParsers.Classic.QueryParser;

    /// <summary>
    /// Suite of tests for the <see cref="DocumentationSearches"/> class.
    /// </summary>
    [TestFixture]
    public class DocumentationSearchesTestFixture
    {
        private string PathDocumentationIndex { get; set; }
        private string RootAppData { get; set; }
        private DocumentationSearches documentationSearches;
        private FSDirectory IndexDirectory { get; set; }
        private RAMDirectory RamIndexDirectory { get; set; }
        private StandardAnalyzer Analyzer { get; set; }
        private IndexWriter IndexWriter { get; set; }
        private IndexWriter IndexWriterSuggest { get; set; }
        private SearcherManager SearcherManager { get; set; }
        private QueryParser QueryParser { get; set; }

        [SetUp]
        public void SetUp()
        {
            this.RootAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "RHEA_Group");
            this.PathDocumentationIndex = Path.Combine(TestContext.CurrentContext.TestDirectory,
                @"Documentation\book.json");
            var bookFileHandler = new BookFileHandler();
            var book = bookFileHandler.DeserializeJsonBook(this.PathDocumentationIndex);
            this.documentationSearches = new DocumentationSearches(this.PathDocumentationIndex, this.RootAppData, book, true);

            this.IndexDirectory = this.documentationSearches.IndexDirectory;
            this.RamIndexDirectory = this.documentationSearches.RamIndexDirectory;
            this.Analyzer = this.documentationSearches.Analyzer;
            this.IndexWriter = this.documentationSearches.IndexWriter;
            this.SearcherManager = this.documentationSearches.SearcherManager;
            this.QueryParser = this.documentationSearches.QueryParser;
            this.IndexWriterSuggest = this.documentationSearches.IndexWriterSuggest;
        }

        [Test]
        public void VerifyThatInitializationWorks()
        {
            Assert.IsNotNull(this.IndexDirectory);
            Assert.IsNotNull(this.RamIndexDirectory);
            Assert.IsNotNull(this.Analyzer);
            Assert.IsNotNull(this.IndexWriter);
            Assert.IsNotNull(this.IndexWriterSuggest);
            Assert.IsNotNull(this.SearcherManager);
            Assert.IsNotNull(this.QueryParser);

            // Check that indexes exist. 
            Assert.IsTrue(DirectoryReader.IndexExists(this.IndexDirectory));
            Assert.IsTrue(DirectoryReader.IndexExists(this.RamIndexDirectory));

            // Check all files have been indexed in the file system directory and there is no doubles.
            var readerFsd = this.IndexWriter.Reader;
            Assert.That(this.IndexWriter.NumDocs(), Is.EqualTo(5));
            Assert.That(readerFsd.GetDocCount("name"), Is.EqualTo(5));
            Assert.That(readerFsd.GetDocCount("path"), Is.EqualTo(5));

            // Check all file names and file paths have been indexed in the ram directory and there is no doubles.
            var readerRam = this.IndexWriterSuggest.Reader;
            Assert.That(this.IndexWriterSuggest.NumDocs(), Is.EqualTo(5));
            Assert.That(readerRam.GetDocCount("name"), Is.EqualTo(5));
            Assert.That(readerRam.GetDocCount("path"), Is.EqualTo(5));

            this.documentationSearches.IndexWriter.Dispose();
        }

        [Test]
        public void VerifyThatSuggestPagesWorks()
        {
            var results = this.documentationSearches.SuggestPages("Intro");
            Assert.That(results.Count, Is.EqualTo(2));
            Assert.That(results.ElementAt(0).Name, Is.EqualTo("<b>Intro</b>duction"));
            var path = results.ElementAt(0).Path;
            var pathResult = path.Equals("1_intro_Introduction.md") || path.Equals("1_installintro_Introduction.md");
            Assert.IsTrue(pathResult);
            Assert.That(results.ElementAt(1).Name, Is.EqualTo("<b>Intro</b>duction"));
            path = results.ElementAt(1).Path;
            pathResult = path.Equals("1_intro_Introduction.md") ||
                         path.Equals("Installation\\1_installintro_Introduction.md");
            Assert.IsTrue(pathResult);
            results = this.documentationSearches.SuggestPages("iNtRodUc");
            Assert.That(results.Count, Is.EqualTo(2));
            Assert.That(results.ElementAt(0).Name, Is.EqualTo("<b>Introduc</b>tion"));
            path = results.ElementAt(0).Path;
            pathResult = path.Equals("1_intro_Introduction.md") ||
                         path.Equals("Installation\\1_installintro_Introduction.md");
            Assert.IsTrue(pathResult);
            Assert.That(results.ElementAt(1).Name, Is.EqualTo("<b>Introduc</b>tion"));
            path = results.ElementAt(1).Path;
            pathResult = path.Equals("1_intro_Introduction.md") ||
                         path.Equals("Installation\\1_installintro_Introduction.md");
            Assert.IsTrue(pathResult);

            results = this.documentationSearches.SuggestPages("start");
            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results.ElementAt(0).Name, Is.EqualTo("Quick <b>Start</b>"));
            path = results.ElementAt(0).Path;
            Assert.That(path, Is.EqualTo("2_quickstart_Quick_Start.md"));

            results = this.documentationSearches.SuggestPages("step");
            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results.ElementAt(0).Name, Is.EqualTo("<b>Step</b> by <b>Step</b>"));
            path = results.ElementAt(0).Path;
            Assert.That(path, Is.EqualTo("Installation\\2_installstep_Step_by_Step.md"));

            results = this.documentationSearches.SuggestPages("Hello");
            Assert.That(results.Count, Is.EqualTo(0));

            results = this.documentationSearches.SuggestPages("installation");
            Assert.That(results.Count, Is.EqualTo(0));

            results = this.documentationSearches.SuggestPages(" ");
            Assert.IsNull(results);

            this.documentationSearches.IndexWriterSuggest.Dispose();
            this.documentationSearches.IndexWriter.Dispose();
        }

        [Test]
        public void VerifyThatSearchInTheDocumentationWorks()
        {
            var pathResultPage = this.documentationSearches.Search("Hello world");
            Assert.IsNotNull(pathResultPage);
            Assert.That(File.Exists(pathResultPage));
            var extensionResultPageIsHtml = Path.GetExtension(pathResultPage).Equals(".html");
            Assert.IsTrue(extensionResultPageIsHtml);

            var pagesList = this.documentationSearches.SearchInPagesContent("pietas");
            Assert.IsNotNull(pagesList);
            Assert.That(pagesList.Count, Is.GreaterThanOrEqualTo(1));
            var pathPageIsCorrect = false;
            foreach (var page in pagesList)
            {
                if (page.Path.Equals("2_quickstart_Quick_Start.md"))
                {
                    pathPageIsCorrect = true;
                    break;
                }
            }
            Assert.IsTrue(pathPageIsCorrect);

            pagesList = this.documentationSearches.SearchInPagesContent("Not_present_in_the_doc");
            Assert.IsNotNull(pagesList);
            Assert.That(pagesList.Count, Is.EqualTo(0));

            this.documentationSearches.IndexWriter.Dispose();
        }
    }
}
