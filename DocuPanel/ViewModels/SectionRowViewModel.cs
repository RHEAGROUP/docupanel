// --------------------------------------------------------------------------------------------------
//  <copyright file="SectionRowViewModel.cs" company="RHEA System S.A.">
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
    using Models;
    using ReactiveUI;
    using System;
    using System.Reactive.Linq;

    /// <summary>
    /// The row view model for the <see cref="Section"/>
    /// </summary>
    public class SectionRowViewModel : ReactiveObject
    {
        /// <summary>
        /// Path of the file icon;
        /// </summary>
        private const string PageImage = "Resources/Images/File_16x.png";

        /// <summary>
        /// Path of the folder icon;
        /// </summary>
        private const string FolderImage = "Resources/Images/VSO_Folder_16x.png";

        /// <summary>
        /// Path of the open folder icon
        /// </summary>
        private const string OpenFolderImage = "Resources/Images/VSO_FolderOpen_16x.png";

        /// <summary>
        /// Backing field for <see cref="DisplayedImagePath"/>.
        /// </summary>
        private string displayedImagePath;

        /// <summary>
        /// Backing field for <see cref="IsSelected"/>.
        /// </summary>
        private bool isSelected;

        /// <summary>
        /// Backing field for <see cref="IsExpanded"/>.
        /// </summary>
        private bool isExpanded;

        /// <summary>
        /// Backing field for <see cref="ContainedSections"/>.
        /// </summary>
        private ReactiveList<SectionRowViewModel> containedSections;

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionRowViewModel"/> class.
        /// </summary>
        /// <param name="section"> The <see cref="Models.Section"/> associated to this section row view-model.</param>
        /// <param name="sectionParent">The parent of the section.</param>
        public SectionRowViewModel(Section section, SectionRowViewModel sectionParent)
        {
            this.SectionParent = sectionParent;
            this.Section = section;
            this.ContainedSections = new ReactiveList<SectionRowViewModel>();
            this.Name = section.Name;
            this.PagePath = section.PagePath;
            this.AreSubsectionsLoaded = false;
            this.InitializeRowViewIcons();
            this.WhenAnyValue(vm => vm.IsExpanded)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => this.ReactOnIsExpandedChange());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionRowViewModel"/> class. This constructor is used to create an empty dummy child.
        /// </summary>
        public SectionRowViewModel()
        {
        }

        /// <summary>
        /// Gets or sets the value indicating whether if the subsections are loaded.
        /// </summary>
        public bool AreSubsectionsLoaded { get; set; }

        /// <summary>
        /// Gets or sets the name of the document.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the path of the section page.
        /// </summary>
        public string PagePath { get; set; }

        /// <summary>
        /// Gets or sets the path of the image displayed before the name of the section.
        /// </summary>
        public string DisplayedImagePath
        {
            get { return this.displayedImagePath; }
            set { this.RaiseAndSetIfChanged(ref this.displayedImagePath, value); }
        }

        /// <summary>
        /// Gets the <see cref="SectionRowViewModel"/> parent of this view-model.
        /// </summary>
        public SectionRowViewModel SectionParent { get; }

        /// <summary>
        /// Gets or sets the list of contained sections.
        /// </summary>
        public ReactiveList<SectionRowViewModel> ContainedSections
        {
            get { return this.containedSections; }
            set { this.RaiseAndSetIfChanged(ref this.containedSections, value); }
        }

        /// <summary>
        /// Gets or sets the state of the selection of this section row view-model.
        /// </summary>
        public bool IsSelected
        {
            get { return this.isSelected; }
            set { this.RaiseAndSetIfChanged(ref this.isSelected, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="Models.Section"/> associated to this section row view-model.
        /// </summary>
        public Section Section { get; set; }

        /// <summary>
        /// Gets or sets the state of the expension of this section row.
        /// </summary>
        public bool IsExpanded
        {
            get { return this.isExpanded; }
            set { this.RaiseAndSetIfChanged(ref this.isExpanded, value); }
        }

        /// <summary>
        /// Create a dummy child if the section represented by this view-model contains subsections. The creation of a dummy child and the inclusion
        /// in the <see cref="ContainedSections"/> list reveal the expandable button of the view associated to this view-model.
        /// This method also choose the appropriate icon to display in the view associated to this view-model.
        /// </summary>
        public void InitializeRowViewIcons()
        {
            if (this.HasSection())
            {
                var dummyChild = new SectionRowViewModel();
                this.ContainedSections.Add(dummyChild);
                this.DisplayedImagePath = FolderImage;
                return;
            }

            if (this.PagePath != null)
            {
                this.DisplayedImagePath = PageImage;
                return;
            }

            this.DisplayedImagePath = null;
        }

        /// <summary>
        /// Load the view-models of the subsections of this section view-model.
        /// </summary>
        public void LoadSubsections()
        {
            if (this.HasSection())
            {
                this.containedSections.Clear();
                foreach (var section in this.Section.Sections)
                {
                    this.ContainedSections.Add(new SectionRowViewModel(section, this));
                }
            }

            this.AreSubsectionsLoaded = true;
        }

        /// <summary>
        /// Verify if the <see cref="Models.Section"/> represented by this view-model contains subsections.
        /// </summary>
        /// <returns>Return true if the the <see cref="Models.Section"/> represented by this view-model contains at least one subsection and false if not.</returns>
        public bool HasSection()
        {
            if (this.Section.Sections.Count >= 1)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Update the view associated to this view-model when the property IsExpanded changes.
        /// </summary>
        public void ReactOnIsExpandedChange()
        {
            if (!this.IsExpanded && this.HasSection())
            {
                this.DisplayedImagePath = FolderImage;
                return;
            }

            if (!this.IsExpanded)
            {
                return;
            }

            if (this.SectionParent != null)
            {
                this.SectionParent.IsExpanded = true;
            }

            if (!this.HasSection())
            {
                return;
            }

            this.DisplayedImagePath = OpenFolderImage;

            if (this.AreSubsectionsLoaded)
            {
                return;
            }

            this.LoadSubsections();
        }
    }
}