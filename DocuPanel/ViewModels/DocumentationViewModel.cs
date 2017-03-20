// --------------------------------------------------------------------------------------------------
//  <copyright file="DocumentationViewModel.cs" company="RHEA System S.A.">
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

namespace DocuPanel.ViewModels
{
    using System.IO;
    using System;
    using System.Reactive.Linq;
    using CefSharp;
    using Models;
    using ReactiveUI;

    /// <summary>
    /// The view-model for the Documentation main view.
    /// </summary>
    public class DocumentationViewModel : ReactiveObject
    {
        /// <summary>
        /// The object to manage Markdown and HTML operations.
        /// </summary>
        private MarkdownToHtml markdownToHtml;

        /// <summary>
        /// Backing field for <see cref="SearchText"/>.
        /// </summary>
        private string searchText;

        /// <summary>
        /// Backing field for the <see cref="SuggestPagesList"/>.
        /// </summary>
        private ReactiveList<Page> suggestPagesList;

        /// <summary>
        /// Backing field for the <see cref="SuggestedPageSelected"/>.
        /// </summary>
        private Page suggestedPageSelected;

        /// <summary>
        /// Backing field for the <see cref="ComboSearchBoxIsPopupOpen"/>.
        /// </summary>
        private bool comboSearchBoxIsPopupOpen;

        /// <summary>
        /// Backing field for the <see cref="IsPopupSearchInPageOpen"/>.
        /// </summary>
        private bool isPopupSearchInPageOpen;

        /// <summary>
        /// Backing field for the <see cref="SearchInThePageText"/>.
        /// </summary>
        private string searchInThePageText;

        /// <summary>
        /// Backing field for the <see cref="FirstsRowSections"/>.
        /// </summary>
        private ReactiveList<SectionRowViewModel> firstsRowSections;

        /// <summary>
        /// Backing field for the <see cref="SelectedSection"/>.
        /// </summary>
        private SectionRowViewModel selectedSection;

        /// <summary>
        /// Backing field for <see cref="PathDocumentationIndex"/>.
        /// </summary>
        private string pathDocumentationIndex;

        /// <summary>
        /// Backing field for <see cref="Book"/>.
        /// </summary>
        private Book book;

        /// <summary>
        /// Backing field for <see cref="BookTitle"/>.
        /// </summary>
        private string bookTitle;

        /// <summary>
        /// The object which performs the searches.
        /// </summary>
        public DocumentationSearches DocumentationSearches;

        /// <summary>
        /// Gets or sets the state of the documentation loading. 
        /// This property indicates whether DocuPanel has been initialized whith the actual documentation.
        /// </summary>
        public bool IsDocumentationLoaded { get; set; }

        /// <summary>
        /// Gets or sets the root directory of the doccumentation.
        /// </summary>
        public string PathDocumentationIndex
        {
            get { return this.pathDocumentationIndex; }
            set { this.RaiseAndSetIfChanged(ref this.pathDocumentationIndex, value); }
        }

        /// <summary>
        /// Gets or sets the path of the application data folder.
        /// </summary>
        public string RootAppData { get; set; }

        /// <summary>
        /// Gets or sets the property which indicates whether the indexation of the documentation needs to be updated.
        /// </summary>
        public bool UpdateIndexation { get; set; }

        /// <summary>
        /// Gets or sets the reference to the <see cref="IWebBrowser"/>.
        /// </summary>
        public IWebBrowser WebBrowser { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="Models.Book"/> of the documentation.
        /// </summary>
        public Book Book {
            get { return this.book; }
            set { this.RaiseAndSetIfChanged(ref this.book, value); }
        }

        /// <summary>
        /// Gets or sets the title of the documentation;
        /// </summary>
        public string BookTitle {
            get { return this.bookTitle; }
            set { this.RaiseAndSetIfChanged(ref this.bookTitle, value); }
        }

        /// <summary>
        /// Command to navigate the browser backward.
        /// </summary>
        public ReactiveCommand<object> NavigateBackCommand { get; private set; }

        /// <summary>
        /// Command to navigate the browser forward.
        /// </summary>
        public ReactiveCommand<object> NavigateForwardCommand { get; private set; }

        /// <summary>
        /// Command to initiate search.
        /// </summary>
        public ReactiveCommand<object> SearchCommand { get; private set; }

        /// <summary>
        /// Command to access to the Home page of the documentation. 
        /// </summary>
        public ReactiveCommand<object> HomeCommand { get; private set; }

        /// <summary>
        /// Command to open or close the popup which contains a textBox to search a word in the page displayed.
        /// </summary>
        public ReactiveCommand<object> SearchInThePageCommand { get; private set; }

        /// <summary>
        /// Gets or sets the search text of the search bar.
        /// </summary>
        public string SearchText
        {
            get { return this.searchText; }
            set { this.RaiseAndSetIfChanged(ref this.searchText, value); }
        }

        /// <summary>
        /// Gets or sets the list of the sections displayed at the root of the documentation. These sections don't have a section parent and are always displayed.
        /// </summary>
        public ReactiveList<SectionRowViewModel> FirstsRowSections
        {
            get { return this.firstsRowSections; }
            set { this.RaiseAndSetIfChanged(ref this.firstsRowSections, value); }
        }

        /// <summary>
        /// Gets or sets the selected <see cref="SectionRowViewModel"/>.
        /// </summary>
        public SectionRowViewModel SelectedSection
        {
            get { return this.selectedSection; }
            set { this.RaiseAndSetIfChanged(ref this.selectedSection, value);}
        }

        /// <summary>
        /// Gets or sets the list which contains the <see cref="Page"/> to suggest to the user when he is writing in the search bar.
        /// </summary>
        public ReactiveList<Page> SuggestPagesList
        {
            get { return this.suggestPagesList; }
            set { this.RaiseAndSetIfChanged(ref this.suggestPagesList, value); }
        }

        /// <summary>
        /// Gets or sets the suggested <see cref="Page"/> selected by user. The page is an element of the <see cref="SuggestPagesList"/>.
        /// </summary>
        public Page SuggestedPageSelected
        {
            get { return this.suggestedPageSelected; }
            set { this.RaiseAndSetIfChanged(ref this.suggestedPageSelected, value); }
        }

        /// <summary>
        /// Gets or sets the state of the opening popup which prints the pages to suggest to the user when he is writing in the search bar.
        /// </summary>
        public bool ComboSearchBoxIsPopupOpen
        {
            get { return this.comboSearchBoxIsPopupOpen; }
            set { this.RaiseAndSetIfChanged(ref this.comboSearchBoxIsPopupOpen, value); }
        }


        /// <summary>
        /// Gets or sets the state of the opening popup which allow a user to type a text to search in the actual displayed page.
        /// </summary>
        public bool IsPopupSearchInPageOpen
        {
            get { return this.isPopupSearchInPageOpen; }
            set { this.RaiseAndSetIfChanged(ref this.isPopupSearchInPageOpen, value); }
        }

        /// <summary>
        /// Gets or sets the text of the text box associated to the search in the actual page.
        /// </summary>
        public string SearchInThePageText
        {
            get { return this.searchInThePageText; }
            set { this.RaiseAndSetIfChanged(ref this.searchInThePageText, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentationViewModel"/> class.
        /// </summary>
        /// <param name="webBrowser"> The browser control of the view <see cref="DocumentationView"/> that this view model will represent.</param>
        /// <param name="pathDocumentationIndex"> The path to the directory that contains the documentation files.</param>
        /// <param name="rootAppData"> The path of the application data.</param>
        /// <param name="updateIndexation"> Indicates whether the indexation of the documentation needs to be updated.</param>
        public DocumentationViewModel(IWebBrowser webBrowser, string pathDocumentationIndex, string rootAppData, bool updateIndexation)
        {
            this.PathDocumentationIndex = pathDocumentationIndex;
            this.RootAppData = rootAppData;
            this.UpdateIndexation = updateIndexation;

            this.WebBrowser = webBrowser;
            this.InitializeCommands();

            this.FirstsRowSections = new ReactiveList<SectionRowViewModel>();

            this.WhenAnyValue(vm => vm.PathDocumentationIndex)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => this.ReactOnPathDocumentationIndexChange());
        }

        /// <summary>
        /// Update the view model from  the new Documentation directory.
        /// </summary>
        public void ReactOnPathDocumentationIndexChange()
        {
            if (this.IsDocumentationLoaded)
            {
                return;
            }

            if (!File.Exists(this.PathDocumentationIndex))
            {
                throw new FileNotFoundException(string.Format("Could not find the index file at the path : '{0}'", this.pathDocumentationIndex));
            }

            this.FirstsRowSections.Clear();
            this.markdownToHtml = new MarkdownToHtml(this.RootAppData);

            var bookFileHandler = new BookFileHandler();
            this.Book = bookFileHandler.DeserializeJsonBook(this.PathDocumentationIndex);

            if (this.UpdateIndexation)
            {
                this.markdownToHtml.TransformTheWholeDocumentationIntoHtml(this.Book, this.PathDocumentationIndex);
            }

            this.DocumentationSearches = new DocumentationSearches(this.PathDocumentationIndex, this.RootAppData, this.Book, this.UpdateIndexation);

            this.BookTitle = this.Book.Title ?? "Table of Contents";

            if (this.Book.Sections.Count >= 1)
            {
                foreach (var section in this.Book.Sections)
                {
                    var sectionViewModel = new SectionRowViewModel(section, null);
                    this.FirstsRowSections.Add(sectionViewModel);
                }
            }

            this.WhenAnyValue(vm => vm.SelectedSection).ObserveOn(RxApp.MainThreadScheduler).Subscribe(_ => this.ReactOnSelectionChange());

            this.LoadBookMainPage();
            this.IsDocumentationLoaded = true;

            this.WhenAnyValue(vm => vm.SearchText)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => this.ReactOnSearchTextChange());

            this.WhenAnyValue(vm => vm.SuggestedPageSelected)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => this.ReactOnSuggestedPageSelected());
        }
        
        /// <summary>
        /// Reacts to the change in selected node.
        /// </summary>
        private void ReactOnSelectionChange()
        {
            if (this.SelectedSection?.PagePath == null)
            {
                return;
            }
            
            var pathFile = markdownToHtml.CreateAbsolutePathFile(this.pathDocumentationIndex, SelectedSection.PagePath);
            if (!markdownToHtml.IsFileAlreadyExistsInHtml(pathFile))
            {
                markdownToHtml.TransformToHtml(pathFile);
            }

            this.WebBrowser.Load(markdownToHtml.ConvertMarkdownFilePathToHtmlFilePath(pathFile));
        }

        /// <summary>
        /// Reacts when the text inside the search bar is changing. 
        /// </summary>
        public void ReactOnSearchTextChange()
        {
            if (this.SearchText?.Length > 2)
            {
                this.SuggestPagesList = new ReactiveList<Page>(this.DocumentationSearches.SuggestPages(this.SearchText));
                if (this.SuggestPagesList.IsEmpty)
                {
                    this.ComboSearchBoxIsPopupOpen = false;
                }
                else
                {
                    this.ComboSearchBoxIsPopupOpen = true;
                }
            }

            else
            {
                this.ComboSearchBoxIsPopupOpen = false;
            }
        }

        /// <summary>
        /// Reacts when a page from the <see cref="SuggestPagesList"/> is selected. 
        /// When the user selects a suggested page, the page is loaded. 
        /// </summary>
        public void ReactOnSuggestedPageSelected()
        {
            if (this.SuggestedPageSelected?.Path != null)
            {
                var pathHtml = this.markdownToHtml.ConvertMarkdownFilePathToHtmlFilePath(this.SuggestedPageSelected.Path);
                this.WebBrowser.Load(pathHtml);
            }
        }

        /// <summary>
        /// Initializes the commands.
        /// </summary>
        public void InitializeCommands()
        {
            var canGoBack = this.WhenAnyValue(vm => vm.WebBrowser.CanGoBack);
            this.NavigateBackCommand = ReactiveCommand.Create(canGoBack);
            this.NavigateBackCommand.ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => this.ExecuteNavigateBackCommand());

            var canGoForward = this.WhenAnyValue(vm => vm.WebBrowser.CanGoForward);
            this.NavigateForwardCommand = ReactiveCommand.Create(canGoForward);
            this.NavigateForwardCommand.ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => this.ExecuteNavigateForwardCommand());

            var canSearch = this.WhenAnyValue(vm => vm.SearchText, (text) => !String.IsNullOrWhiteSpace(text));
            this.SearchCommand = ReactiveCommand.Create(canSearch);
            this.SearchCommand.ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => this.ExecuteSearchCommand());

            var canGoHome = this.WhenAnyValue(vm => vm.Book, vm => vm.Book.PagePath , (bookDocu, pagePath) => bookDocu != null && pagePath != null);
            this.HomeCommand = ReactiveCommand.Create(canGoHome);
            this.HomeCommand.ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => this.ExecuteHomeCommand());

            this.SearchInThePageCommand = ReactiveCommand.Create();
            this.SearchInThePageCommand.ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => this.ExecuteSearchInThePageCommand());
        }

        /// <summary>
        /// Executes the Home command.
        /// </summary>
        private void ExecuteHomeCommand()
        {
            this.LoadBookMainPage();   
        }

        /// <summary>
        /// Executes the search command associated to the search bar.
        /// </summary>
        private void ExecuteSearchCommand()
        {
            var resultPage = this.DocumentationSearches.Search(this.SearchText);
            this.ComboSearchBoxIsPopupOpen = false;
            if (resultPage != null)
            {
                this.WebBrowser.Load(resultPage);
            }
        }

        /// <summary>
        /// Opens or closes a dialog that allows a user to type the text he wants to search in the actual displayed page.
        /// </summary>
        private void ExecuteSearchInThePageCommand()
        {
            this.IsPopupSearchInPageOpen = !this.IsPopupSearchInPageOpen;
            if (this.IsPopupSearchInPageOpen == false)
            {
                this.WebBrowser.StopFinding(true);
            }
        }

        /// <summary>
        /// Executes the browser forward navigation.
        /// </summary>
        private void ExecuteNavigateForwardCommand()
        {
            this.WebBrowser.Forward();
            this.DeselectSection();
        }

        /// <summary>
        /// Execute the browser back navigation.
        /// </summary>
        private void ExecuteNavigateBackCommand()
        {
            this.WebBrowser.Back();
            this.DeselectSection();
        }

        /// <summary>
        /// Load the home HTML page of the documentation if it exists.
        /// </summary>
        private void LoadBookMainPage()
        {
            if (this.Book?.PagePath == null)
            {
                return;
            }

            var pathFile = markdownToHtml.CreateAbsolutePathFile(this.pathDocumentationIndex, this.Book.PagePath);
            if (!markdownToHtml.IsFileAlreadyExistsInHtml(pathFile))
            {
                markdownToHtml.TransformToHtml(pathFile);
            }
            this.WebBrowser.Load(markdownToHtml.ConvertMarkdownFilePathToHtmlFilePath(pathFile));

            this.DeselectSection();
        }

        /// <summary>
        /// If a <see cref="SectionRowViewModel"/> is selected, deselect it;
        /// </summary>
        private void DeselectSection()
        {
            if (this.SelectedSection != null)
            {
                this.SelectedSection.IsSelected = false;
            }
        }
    }
}