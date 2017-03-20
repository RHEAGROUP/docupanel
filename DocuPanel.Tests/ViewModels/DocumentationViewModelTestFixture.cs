// --------------------------------------------------------------------------------------------------
//  <copyright file="DocumentationViewModelTestFixture.cs" company="RHEA System S.A.">
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

namespace DocuPanel.Tests.ViewModels
{
    using System;
    using System.IO;
    using System.Reactive.Concurrency;
    using CefSharp;
    using DocuPanel.ViewModels;
    using Moq;
    using NUnit.Framework;
    using ReactiveUI;

    /// <summary>
    /// Suite of tests for the <see cref="DocumentationViewModel"/> class.
    /// </summary>
    [TestFixture]
    public class DocumentationViewModelTestFixture
    { 
        private Mock<IWebBrowser> webBrowser;
        private DocumentationViewModel documentationViewModel;
        private string PathDocumentationIndex { get; set; }
        private string RootAppData { get; set; }

        [SetUp]
        public void SetUp()
        {
            this.PathDocumentationIndex = Path.Combine(TestContext.CurrentContext.TestDirectory, @"Documentation\book.json");
            this.RootAppData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "RHEA_Group");
            
            RxApp.MainThreadScheduler = Scheduler.CurrentThread;

            //Initializes the web browser and its commands
            var browser = new Mock<IBrowser>();
            this.webBrowser = new Mock<IWebBrowser>();

            this.webBrowser.Setup(x => x.CanGoBack).Returns(true);
            this.webBrowser.Setup(x => x.CanGoForward).Returns(true);
            this.webBrowser.Setup(x => x.GetBrowser()).Returns(browser.Object);

            this.documentationViewModel = new DocumentationViewModel(this.webBrowser.Object, this.PathDocumentationIndex, this.RootAppData, true);
        }

        [Test]
        public void VerifyThatInitializeAndExecuteCommandsWorks()
        {
            Assert.IsTrue(this.documentationViewModel.NavigateBackCommand.CanExecute(null));
            Assert.IsTrue(this.documentationViewModel.NavigateForwardCommand.CanExecute(null));
            Assert.IsTrue(this.documentationViewModel.SearchInThePageCommand.CanExecute(null));
            Assert.IsFalse(this.documentationViewModel.SearchCommand.CanExecute(null));

            Assert.DoesNotThrow(() => this.documentationViewModel.NavigateBackCommand.Execute(null));
            Assert.DoesNotThrow(() => this.documentationViewModel.NavigateForwardCommand.Execute(null));
            Assert.DoesNotThrow(() => this.documentationViewModel.SearchInThePageCommand.Execute(null));

            this.documentationViewModel.SearchText = "DocuPanel";
            Assert.IsTrue(this.documentationViewModel.SearchCommand.CanExecute(null));
            Assert.DoesNotThrow(() => this.documentationViewModel.SearchCommand.Execute(null));

            this.documentationViewModel.DocumentationSearches.IndexWriter.Dispose();
        }

        [Test]
        public void VerifyThatHomeCommandWorks()
        {
            Assert.IsTrue(this.documentationViewModel.HomeCommand.CanExecute(null));
            Assert.DoesNotThrow(() => this.documentationViewModel.HomeCommand.Execute(null));

            this.documentationViewModel.Book.PagePath = null;
            Assert.IsFalse(this.documentationViewModel.HomeCommand.CanExecute(null));

            this.documentationViewModel.DocumentationSearches.IndexWriter.Dispose();
        }

        [Test]
        public void VerifyThatInvalidPathDocumentationIndexThrowException()
        {
            this.documentationViewModel.IsDocumentationLoaded = false;
            Assert.Throws<Exception>(() => this.documentationViewModel.PathDocumentationIndex = "RHEA");

            this.documentationViewModel.DocumentationSearches.IndexWriter.Dispose();
        }
    }
}
