// --------------------------------------------------------------------------------------------------
//  <copyright file="DocumentationSearches.cs" company="RHEA System S.A.">
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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Lucene.Net.Analysis.Standard;
    using Lucene.Net.Documents;
    using Lucene.Net.Index;
    using Lucene.Net.QueryParsers.Classic;
    using Lucene.Net.Search;
    using Lucene.Net.Util;
    using Lucene.Net.Search.Suggest;
    using Lucene.Net.Search.Suggest.Analyzing;
    using Lucene.Net.Store;

    /// <summary>
    /// A class to perform searches in the documentation.
    /// </summary>
    public class DocumentationSearches : IDisposable
    {
        /// <summary>
        /// A <see cref="MarkdownToHtml"/> object.
        /// </summary>
        private readonly MarkdownToHtml markdownToHtml;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentationSearches"/> class.
        /// </summary>
        /// <param name="pathDocumentationIndex"> The path to the index of the documentation.</param>
        /// <param name="rootAppData"> The path to the application data folder.</param>
        /// <param name="book"> The <see cref="Book"/> associated to the documentation.</param>
        /// <param name="updateIndexation"> Indicates whether the indexation of the documentation needs to be updated.</param>
        public DocumentationSearches(string pathDocumentationIndex, string rootAppData, Book book, bool updateIndexation)
        {
            this.markdownToHtml = new MarkdownToHtml(rootAppData);
            this.PathIndexDirectory = Path.Combine(rootAppData, "DocuPanel", "lucene-index");
            this.IndexDirectory = FSDirectory.Open(new DirectoryInfo(this.PathIndexDirectory));

            this.Analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);

            this.RamIndexDirectory = new RAMDirectory();
            var indexWriterConfig = new IndexWriterConfig(LuceneVersion.LUCENE_48, this.Analyzer);
            this.IndexWriterSuggest = new IndexWriter(this.RamIndexDirectory, indexWriterConfig);

            this.QueryParser = new MultiFieldQueryParser(LuceneVersion.LUCENE_48, new[] {"name"}, this.Analyzer);
            this.IndexWriter = new IndexWriter(this.IndexDirectory,
                new IndexWriterConfig(LuceneVersion.LUCENE_48, this.Analyzer));
            this.SearcherManager = new SearcherManager(this.IndexWriter, true);

            this.AddDocumentationToIndex(book, pathDocumentationIndex, updateIndexation);
        }

        /// <summary>
        /// Gets or sets the path of the index directory.
        /// </summary>
        public string PathIndexDirectory { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="FSDirectory"/>.
        /// </summary>
        public FSDirectory IndexDirectory { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="RAMDirectory"/>.
        /// </summary>
        public RAMDirectory RamIndexDirectory { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="StandardAnalyzer"/>.
        /// </summary>
        public StandardAnalyzer Analyzer { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="AnalyzingInfixSuggester"/>.
        /// </summary>
        public AnalyzingInfixSuggester AnalyzingInfixSuggester { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IndexWriter"/> for the basic search.
        /// </summary>
        public IndexWriter IndexWriter { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="IndexWriter"/> for the suggestion search.
        /// </summary>
        public IndexWriter IndexWriterSuggest { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="SearcherManager"/>.
        /// </summary>
        public SearcherManager SearcherManager { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="QueryParser"/>.
        /// </summary>
        public QueryParser QueryParser { get; private set; }

        #region Indexing

        /// <summary>
        /// Add all the <see cref="Section"/> contained in the index file to the Lucene index./>
        /// </summary>
        /// <param name="book"> The <see cref="Book"/> which represents the index file.</param>
        /// <param name="pathDocumentationIndex"> The path of the index file of the documentation.</param>
        /// <param name="updateIndexation"> Indicates whether the indexation needs to be updated. If true, the documents stored in the file system directory are deleted and recreated.</param>
        public void AddDocumentationToIndex(Book book, string pathDocumentationIndex, bool updateIndexation)
        {
            if (book == null)
            {
                Console.Error.WriteLine("The book does not exist, no files were indexed");
                return;
            }

            if (book.Sections.Count == 0)
            {
                return;
            }

            foreach (var section in book.Sections)
            {
                this.BrowseSection(section, pathDocumentationIndex, updateIndexation);
            }

            this.IndexWriter.Flush(true, true);
            this.IndexWriter.Commit();

            IndexWriterSuggest.Flush(true, true);
            this.IndexWriterSuggest.Commit();
        }

        /// <summary>
        /// Add a <see cref="Section"/> and all its subsections to the Lucene index.
        /// </summary>
        /// <param name="section"> The <see cref="Section"/> to index.</param>
        /// <param name="pathDocumentationIndex"> The path of the index file of the documentation.</param>
        /// <param name="updateIndexation"> Indicates whether the indexation needs to be updated. If true, the documents stored in the file system directory are deleted and recreated.</param>
        public void BrowseSection(Section section, string pathDocumentationIndex, bool updateIndexation)
        {
            string pageContent = "";

            if (section.Sections.Count >= 1)
            {
                foreach (var subsection in section.Sections)
                {
                    this.BrowseSection(subsection, pathDocumentationIndex, updateIndexation);
                }
            }

            if (section.PagePath == null)
            {
                return;
            }

            var absolutePath = this.markdownToHtml.CreateAbsolutePathFile(pathDocumentationIndex, section.PagePath);

            if (File.Exists(absolutePath))
            {
                pageContent = File.ReadAllText(absolutePath);

                // Add the name and the path of the pages in a Lucene index stored in the ram.
                if (section.Name != null)
                {
                    var pageDocument = new Document
                    {
                        new StringField("path", section.PagePath, Field.Store.YES),
                        new StringField("name", section.Name, Field.Store.YES),
                    };
                    this.IndexWriterSuggest.UpdateDocument(new Term("path", section.PagePath), pageDocument);
                }
            }

            else
            {
                Console.Error.WriteLine("No file could be read at the path '{0}'", absolutePath);
            }

            if (!updateIndexation)
            {
                return;
            }

            // Add the pages to the Lucene index stored in a file system directory if the index needs to be updated.
            var document = new Document
            {
                new StringField("path", section.PagePath, Field.Store.YES),
                new TextField("name", section.Name ?? "", Field.Store.YES),
                new TextField("content", pageContent, Field.Store.NO),
            };

            this.IndexWriter.UpdateDocument(new Term("path", section.PagePath), document);
        }

        #endregion

        #region Search

        /// <summary>
        /// Performs a search to suggest a page. To suggest a page we use the AnalyzingInfixSuggester.
        /// </summary>
        /// <param name="query"> The query of the search. The beginning of a page name.</param>
        /// <returns> A list that contains the pages which correspond to the query.</returns>
        public List<Page> SuggestPages(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return null;
            }

            if (IndexWriter.IsLocked(this.RamIndexDirectory))
            {
                this.IndexWriterSuggest.Dispose();
            }

            if (this.AnalyzingInfixSuggester == null)
            {
                this.AnalyzingInfixSuggester = new AnalyzingInfixSuggester(LuceneVersion.LUCENE_48,
                    this.RamIndexDirectory, this.Analyzer);
                try
                {
                    var reader = DirectoryReader.Open(this.RamIndexDirectory);
                    var dictionary = new DocumentDictionary(reader, "name", "name", "path");
                    this.AnalyzingInfixSuggester.Build(dictionary);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }
            }

            var results = AnalyzingInfixSuggester.DoLookup(query, false, 5);
            var listSuggestResult = new List<Page>();
            foreach (var result in results)
            {
                listSuggestResult.Add(new Page
                {
                    Name = result.key,
                    Path = result.payload.Utf8ToString(),
                });
            }

            return listSuggestResult;
        }

        /// <summary>
        /// Performs a search to find information across the whole documentation.
        /// </summary>
        /// <param name="queryString"> The query of the search.</param>
        /// <returns>A list which contains all the pages which match to the query.</returns>
        public string Search(string queryString)
        {
            if (string.IsNullOrWhiteSpace(queryString))
            {
                return null;
            }

            var listSearchResult = new List<Page>();

            // Parse the query - assuming it's not a single term but an actual query string
            var query = QueryParser.Parse(queryString);

            // Execute the search with a fresh indexSearcher
            SearcherManager.MaybeRefreshBlocking();
            SearcherManager.ExecuteSearch(searcher =>
            {
                var topDocs = searcher.Search(query, 10);
                foreach (var result in topDocs.ScoreDocs)
                {
                    var doc = searcher.Doc(result.Doc);
                    listSearchResult.Add(new Page
                    {
                        Name = doc.GetField("name")?.StringValue,
                        Path = doc.GetField("path")?.StringValue,
                    });
                }
            }, exception => { Console.WriteLine(exception.ToString()); });

            var listPagesContentResult = this.SearchInPagesContent(queryString);
            var listPagesContentResultUpdate = new List<Page>();

            foreach (var result in listPagesContentResult)
            {
                if (!listSearchResult.Exists(x => x.Path == result.Path))
                {
                    listPagesContentResultUpdate.Add(result);
                }
            }

            return this.markdownToHtml.CreateHtmlPageFromResults(queryString, listSearchResult, listPagesContentResultUpdate, this.PathIndexDirectory);
        }

        /// <summary>
        /// Performs a search to find a match in the content of the pages;
        /// </summary>
        /// <param name="queryString"> The query of the search.</param>
        /// <returns> The list of the <see cref="Page"/> which contain the query.</returns>
        public List<Page> SearchInPagesContent(string queryString)
        {
            if (string.IsNullOrWhiteSpace(queryString))
            {
                return null;
            }

            queryString = queryString.ToLowerInvariant();

            var listSearchResult = new List<Page>();

            var reader = this.IndexWriter.Reader;
            var contentsearcher = new IndexSearcher(reader);

            var query = new TermQuery(new Term("content", queryString));

            var topDocs = contentsearcher.Search(query, 10);

            foreach (var result in topDocs.ScoreDocs)
            {
                var doc = contentsearcher.Doc(result.Doc);
                listSearchResult.Add(new Page
                {
                    Name = doc.GetField("name")?.StringValue,
                    Path = doc.GetField("path")?.StringValue,
                });
            }

            return listSearchResult;
        }

        #endregion

        /// <summary>
        /// Releases resources if necesary.
        /// </summary>
        public void Dispose()
        {
            this.IndexWriter?.Dispose();
            this.SearcherManager?.Dispose();
            this.AnalyzingInfixSuggester?.Dispose();
        }
    }
}
