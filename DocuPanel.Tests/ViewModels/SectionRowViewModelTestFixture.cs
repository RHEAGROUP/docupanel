// --------------------------------------------------------------------------------------------------
//  <copyright file="SectionRowViewModelTestFixture.cs" company="RHEA System S.A.">
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
    using DocuPanel.Models;
    using NUnit.Framework;
    using DocuPanel.ViewModels;

    /// <summary>
    /// Suite of tests for the <see cref="SectionRowViewModel"/> class.
    /// </summary>
    [TestFixture]
    public class SectionRowViewModelTestFixture
    {
        private SectionRowViewModel sectionRowViewModel;

        [SetUp]
        public void SetUp()
        {
            var section = new Section();
            var subSection = new Section();
            section.Sections.Add(subSection);
            this.sectionRowViewModel = new SectionRowViewModel(section, null);
        }
        
        [Test]
        public void VerifyThatHasSectionWorks()
        {
            Assert.That(this.sectionRowViewModel.HasSection, Is.True);

            var section2 = new Section();
            var sectionRowViewModel2 = new SectionRowViewModel(section2, null);
            Assert.That(sectionRowViewModel2.HasSection, Is.False);
        }

        [Test]
        public void VerifyThatLoadSubsectionsWorks()
        {
            Assert.That(this.sectionRowViewModel.ContainedSections.Count, Is.EqualTo(1));
            this.sectionRowViewModel.LoadSubsections();
            Assert.That(this.sectionRowViewModel.ContainedSections.Count, Is.EqualTo(1));
        }
    }
}