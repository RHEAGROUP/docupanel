// --------------------------------------------------------------------------------------------------
//  <copyright file="MarkdownToHtml.cs" company="RHEA System S.A.">
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
    using Helpers;
    using MarkdownDeep;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using HtmlAgilityPack;

    /// <summary>
    /// A class to manage the markdown and HTML operations.
    /// </summary>
    public class MarkdownToHtml
    {
        /// <summary>
        /// Gets or sets the path of the DocuPanel folder which is located into ApplicationData.
        /// </summary>
        public string PathDocuPanelAppData { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkdownToHtml"/> class.
        /// </summary>
        /// <param name="rootAppData"> The path of the application data folder corresponding to the application.</param>
        public MarkdownToHtml(string rootAppData)
        {
            this.PathDocuPanelAppData = Path.Combine(rootAppData, "DocuPanel");
        }

        /// <summary>
        /// Transforms all the markdown files contained in the documentation into html. 
        /// If a file already exists in html, it is not converted. 
        /// </summary>
        /// <param name="book"> The <see cref="Book"/> associated to the documentation.</param>
        /// <param name="pathDocumentationIndex"> The path to the index of the documentation.</param>
        public void TransformTheWholeDocumentationIntoHtml(Book book, string pathDocumentationIndex)
        {
            if (book == null)
            {
                Console.Error.WriteLine("The book does not exist, no files were indexed");
                return;
            }

            if (book.PagePath != null)
            {
                var absolutePath = this.CreateAbsolutePathFile(pathDocumentationIndex, book.PagePath);
                if (!IsFileAlreadyExistsInHtml(absolutePath))
                {
                    this.TransformToHtml(absolutePath);
                }
            }

            foreach (var section in book.Sections)
            {
                this.BrowseSection(section, pathDocumentationIndex);
            }
        }

        /// <summary>
        /// Browses a <see cref="Section"/> and all its subsection and convert the markdown file associated into html.
        /// If a file already exists in html, it is not converted. 
        /// </summary>
        /// <param name="section"> The <see cref="Section"/> to browse.</param>
        /// <param name="pathDocumentationIndex"> The path to the  index of the documentation.</param>
        public void BrowseSection(Section section, string pathDocumentationIndex)
        {
            foreach (var subsection in section.Sections)
            {
                this.BrowseSection(subsection, pathDocumentationIndex);
            }
            
            if (section.PagePath == null)
            {
                return;
            }
            
            var absolutePath = this.CreateAbsolutePathFile(pathDocumentationIndex, section.PagePath);
            if (!IsFileAlreadyExistsInHtml(absolutePath))
            {
                this.TransformToHtml(absolutePath);
            }
        }

        /// <summary>
        /// Transforms a markdown file into valid HTML and create a HTML file into the local folder in ApplicationData.
        /// </summary>
        /// <param name="markdownFilePath"> The path of the markdown file.</param>
        public void TransformToHtml(string markdownFilePath)
        {
            if (!File.Exists(markdownFilePath))
            {
                throw new FileNotFoundException(string.Format("Could not find the file at the path : '{0}'", markdownFilePath));
            }

            if (!Directory.Exists(PathDocuPanelAppData))
            {
                Directory.CreateDirectory(PathDocuPanelAppData);
            }

            // Initalize the MarkdownDeep object.
            var md = new Markdown
            {
                ExtraMode = true,
                SafeMode = true,
                AutoHeadingIDs = true
            };

            // Read the markdown file and transform the content into HTML.
            var content = File.ReadAllText(markdownFilePath);
            var transformed = md.Transform(content);
            var wrapped = MarkupHelper.WrapHtmlBody(transformed);

            // Create or override a HTML file.
            var htmlFilePath = this.ConvertMarkdownFilePathToHtmlFilePath(markdownFilePath);
            File.WriteAllText(htmlFilePath, wrapped);
        }

        /// <summary>
        /// Verify if a markdown file has already been converted into html.
        /// </summary>
        /// <param name="markdownFilePath"> The path or the name of the markdown file.</param>
        /// <returns>Returns true if the markdown file has already been converted and false if not.</returns>
        public bool IsFileAlreadyExistsInHtml(string markdownFilePath)
        {
            var htmlFilePath = this.ConvertMarkdownFilePathToHtmlFilePath(markdownFilePath);
            if (File.Exists(htmlFilePath))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Convert a markdown file path in a HTML file path.
        /// </summary>
        /// <param name="markdownFilePath"> The markdown file path.</param>
        /// <returns>The HTML file path.</returns>
        public string ConvertMarkdownFilePathToHtmlFilePath(string markdownFilePath)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(markdownFilePath);
            var fileNameWithHtmlExtension = string.Concat(fileNameWithoutExtension, ".html");
            var htmlFilePath = Path.Combine(PathDocuPanelAppData, fileNameWithHtmlExtension);
            return htmlFilePath;
        }

        /// <summary>
        /// Creates the absolute path of a file contained in the documentation. 
        /// </summary>
        /// <param name="pathDocumentationIndex"> The relative path of a documentation file.</param>
        /// <param name="relativeFilePath"> The path of the documentation index.</param>
        /// <returns>The absolute path of the documentation file.</returns>
        public string CreateAbsolutePathFile(string pathDocumentationIndex, string relativeFilePath)
        {
            var directory = Path.GetDirectoryName(pathDocumentationIndex);
            if (directory == null)
            {
                return null;
            }
            return Path.Combine(directory, relativeFilePath);
        }

        /// <summary>
        /// Creates a HTML page to display the results of the query.
        /// </summary>
        /// <param name="query"> The query of the search.</param>
        /// <param name="results"> The pages which include the query in the title.</param>
        /// <param name="listPagesContentResult"> The pages which containe the query in the content.</param>
        /// <param name="pathIndexDirectory"> The path to the index directory.</param>
        /// <returns> The path of the result Html page.</returns>
        public string CreateHtmlPageFromResults(string query, List<Page> results, List<Page> listPagesContentResult, string pathIndexDirectory)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("# Results for *{0}*", query));

            foreach (var result in results)
            {
                if (result.Path != "")
                {
                    var resultHtmlPath = this.ConvertMarkdownFilePathToHtmlFilePath(result.Path);
                    sb.AppendLine(String.Format("## [{0}]({1})", result.Name, resultHtmlPath));

                    sb = this.AddContentText(sb, resultHtmlPath);
                }

                else
                {
                    sb.AppendLine(String.Format("## {0}", result.Name));
                }
            }

            if (listPagesContentResult.Count == 1 && listPagesContentResult.ElementAt(0).Path != "")
            {
                sb.AppendLine(string.Format("## *{0}* is contained in the following page", query));
                var resultHtmlPath = this.ConvertMarkdownFilePathToHtmlFilePath(listPagesContentResult.ElementAt(0).Path);
                sb.AppendLine(String.Format("### [{0}]({1})", listPagesContentResult.ElementAt(0).Name, resultHtmlPath));
                sb = this.AddContentText(sb, resultHtmlPath);
            }

            else if (listPagesContentResult.Count > 1)
            {
                sb.AppendLine(string.Format("## *{0}* is contained in the following pages", query));
                foreach (var result in listPagesContentResult)
                {
                    if (result.Path != "")
                    {
                        var resultHtmlPath = this.ConvertMarkdownFilePathToHtmlFilePath(result.Path);
                        sb.AppendLine(String.Format("### [{0}]({1})", result.Name, resultHtmlPath));
                        sb = this.AddContentText(sb, resultHtmlPath);
                    }
                }
            }

            var pathResultFile = Path.Combine(pathIndexDirectory, "Lucene_Result.md");
            File.WriteAllText(pathResultFile, sb.ToString());
            this.TransformToHtml(pathResultFile);
            return this.ConvertMarkdownFilePathToHtmlFilePath(pathResultFile);
        }
    

    /// <summary>
    /// Adds the beginning of the content of a page to the <see cref="StringBuilder"/>.
    /// </summary>
    /// <param name="sb"> The <see cref="StringBuilder"/>.</param>
    /// <param name="pathHtmlFile"> The path of the file to retrieve the content.</param>
    /// <returns> The <see cref="StringBuilder"/> initial with the beginning of the content of the page.</returns>
    public StringBuilder AddContentText(StringBuilder sb, string pathHtmlFile)
        {
            if (File.Exists(pathHtmlFile))
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(File.ReadAllText(pathHtmlFile));
                var firstParagraph = doc.DocumentNode.SelectSingleNode(".//p");

                if (firstParagraph.InnerText.Length > 300)
                {
                    sb.AppendLine(String.Format("{0}...", firstParagraph.InnerText.Substring(0, 300)));
                }

                else
                {
                    sb.AppendLine(firstParagraph.InnerText.Substring(0, firstParagraph.InnerText.Length));
                }
            }

            return sb;
        }
    }
}