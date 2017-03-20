// --------------------------------------------------------------------------------------------------
//  <copyright file="MarkdownToHtmlTestFixture.cs" company="RHEA System S.A.">
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
    using DocuPanel.Models;
    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="MarkdownToHtml"/> class.
    /// </summary>
    [TestFixture]
    public class MarkdownToHtmlTestFixture
    {
        private string PathDocumentationIndex { get; set; }
        private string PathMarkdownTestFile { get; set; }
        private string RootAppData { get; set; }
        private MarkdownToHtml markdownToHtml;
        private Book book;

        [SetUp]
        public void SetUp()
        {
            this.RootAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RHEA_Group");
            this.PathDocumentationIndex = Path.Combine(TestContext.CurrentContext.TestDirectory, @"Documentation\book.json");
            this.markdownToHtml = new MarkdownToHtml(this.RootAppData);
            this.PathMarkdownTestFile = this.markdownToHtml.CreateAbsolutePathFile(this.PathDocumentationIndex, "Test.md");
            
            var bookFileHandler = new BookFileHandler();
            this.book = bookFileHandler.DeserializeJsonBook(this.PathDocumentationIndex);
        }

        [Test]
        public void VerifyThatTransformToHtmlWorks()
        {
            var fileName = "1_intro_Introduction";

            var markdownFileName = string.Concat(fileName, ".md");
            var markdownFilePath = this.markdownToHtml.CreateAbsolutePathFile(this.PathDocumentationIndex, markdownFileName);
            this.markdownToHtml.TransformToHtml(markdownFilePath);

            var htmlFileName = string.Concat(fileName, ".html");
            var htmlFilePath = Path.Combine(this.RootAppData, "DocuPanel", htmlFileName);

            Assert.That(File.Exists(htmlFilePath), Is.True);
        }

        [Test]
        public void VerifyThatInvalidMarkdownPathThrowsException()
        {
            Assert.Throws<FileNotFoundException>(() => this.markdownToHtml.TransformToHtml("RHEA"));
        }

        [Test]
        public void VerifyThatIsFileAlreadyExistsInHtmlWorks()
        {
            var pathTestFile = Path.Combine(this.RootAppData, "DocuPanel", "Test.html");
            File.Create(pathTestFile);
            Assert.IsTrue(this.markdownToHtml.IsFileAlreadyExistsInHtml(this.PathMarkdownTestFile));

            var pathMarkdown = Path.Combine(this.PathMarkdownTestFile, "NotExists.md");
            Assert.IsFalse(this.markdownToHtml.IsFileAlreadyExistsInHtml(pathMarkdown));
        }

        [Test]
        public void VerifyThatConvertMarkdownFilePathToHtmlFilePathWorks()
        {
            var pathHtml = Path.Combine(this.RootAppData, "DocuPanel", "Test.html");
            var convertedPathHtml = markdownToHtml.ConvertMarkdownFilePathToHtmlFilePath(this.PathMarkdownTestFile);
            Assert.AreEqual(pathHtml,convertedPathHtml);
        }

        [Test]
        public void VerifyThatCreateAbsolutePathFileWorks()
        {
            var relativePathMarkdown = "Test.md";
            var absolutePath = markdownToHtml.CreateAbsolutePathFile(this.PathDocumentationIndex, relativePathMarkdown);
            Assert.AreEqual(absolutePath, this.PathMarkdownTestFile);
        }

        [Test]
        public void VerifyThatTransformTheWholeDocumentationIntoHtmlWorks()
        {
            var pathFictiveAppDataDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, "FictiveAppData");
            Directory.CreateDirectory(pathFictiveAppDataDirectory);
            DirectoryInfo directoryInfo = new DirectoryInfo(pathFictiveAppDataDirectory);

            foreach (var file in directoryInfo.GetFiles())
            {
                file.Delete();
            }

            this.markdownToHtml.PathDocuPanelAppData = pathFictiveAppDataDirectory;
            this.markdownToHtml.TransformTheWholeDocumentationIntoHtml(this.book, this.PathDocumentationIndex);
            
            Assert.That(directoryInfo.GetFiles().Length, Is.EqualTo(6));
            Assert.IsTrue(File.Exists(Path.Combine(pathFictiveAppDataDirectory, "1_installintro_Introduction.html")));
            Assert.IsTrue(File.Exists(Path.Combine(pathFictiveAppDataDirectory, "1_intro_Introduction.html")));
            Assert.IsTrue(File.Exists(Path.Combine(pathFictiveAppDataDirectory, "2_installstep_Step_by_Step.html")));
            Assert.IsTrue(File.Exists(Path.Combine(pathFictiveAppDataDirectory, "2_quickstart_Quick_Start.html")));
            Assert.IsTrue(File.Exists(Path.Combine(pathFictiveAppDataDirectory, "3_prop_User_Properties.html")));
            Assert.IsTrue(File.Exists(Path.Combine(pathFictiveAppDataDirectory, "index.html")));
        }
    }
}