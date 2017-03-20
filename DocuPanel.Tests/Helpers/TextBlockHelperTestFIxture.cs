// --------------------------------------------------------------------------------------------------
//  <copyright file="TextBlockHelperTestFIxture.cs" company="RHEA System S.A.">
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

namespace DocuPanel.Tests.Helpers
{
    using DocuPanel.Helpers;
    using NUnit.Framework;
    using System.Windows.Documents;

    /// <summary>
    /// Suite of tests for the <see cref="TextBlockHelper"/> class.
    /// </summary>
    [TestFixture]
    public class TextBlockHelperTestFIxture
    {
        [Test]
        public void VerifyThatProcessAndInternalProcessWork()
        {
            var xmlString = "<body> Hi Sam, <b> what do </b> you want to know ?</body>";
            var inlineResult = TextBlockHelper.Process(xmlString);

            var span = new Span();
            span.Inlines.Add(new Run("Hi Sam, "));
            var boldSpan = new Span();
            boldSpan.Inlines.Add(new Run(" what do "));
            var bold = new Bold(boldSpan);
            span.Inlines.Add(bold);
            span.Inlines.Add(new Run(" you want to know ?"));

            Assert.IsNotNull(inlineResult);
            var spanInlineResult = (Span) inlineResult;
            Assert.IsNotNull(spanInlineResult.Inlines);
            Assert.AreEqual(span.Inlines.Count, spanInlineResult.Inlines.Count);
            Assert.AreEqual(span.ToString(), spanInlineResult.ToString());
        }
    }
}
