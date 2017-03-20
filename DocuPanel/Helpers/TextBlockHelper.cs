// --------------------------------------------------------------------------------------------------
//  <copyright file="TextBlockHelper.cs" company="RHEA System S.A.">
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

namespace DocuPanel.Helpers
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Xml;

    /// <summary>
    /// TextBlockHelper provides methods to display properly in a TextBlock a html/xml text which contains a bold tag. 
    /// </summary>
    public static class TextBlockHelper
    {
        /// <summary>
        /// Registered DependencyProperty for the <see cref="FormattedTextProperty"/> property.
        /// </summary>
        public static readonly DependencyProperty FormattedTextProperty = DependencyProperty.RegisterAttached(
            "FormattedText",
            typeof(string),
            typeof(TextBlockHelper),
            new UIPropertyMetadata("", OnFormattedTextChanged)
        );

        /// <summary>
        /// Gets the formatted text of the <see cref="TextBlock"/>.
        /// </summary>
        /// <param name="obj"> The <see cref="TextBlock"/> object.</param>
        /// <returns>The formatted text of the <see cref="TextBlock"/>.</returns>
        public static string GetFormattedText(DependencyObject obj)
        {
            return (string) obj.GetValue(FormattedTextProperty);
        }

        /// <summary>
        /// Sets the formatted text of the <see cref="TextBlock"/>.
        /// </summary>
        /// <param name="obj"> The <see cref="TextBlock"/> object.</param>
        /// <param name="value"> The formatted text.</param>
        public static void SetFormattedText(DependencyObject obj, string value)
        {
            obj.SetValue(FormattedTextProperty, value);
        }

        /// <summary>
        /// Event handler for the change event of the <see cref="FormattedTextProperty"/>.
        /// </summary>
        /// <param name="sender"> The sender.</param>
        /// <param name="e"> The arguments.</param>
        public static void OnFormattedTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            string value = e.NewValue as string;
            value = String.Concat("<body>", value, "</body>");
            value = value.Replace(" ", "&#160;");

            var textBlock = sender as TextBlock;

            if (textBlock != null)
            {
                textBlock.Inlines.Clear();
                textBlock.Inlines.Add(Process(value));
            }
        }

        /// <summary>
        /// Creates a new <see cref="Inline"/> value for a <see cref="TextBlock"/> from the xml value.
        /// </summary>
        /// <param name="value"> The xml value to transform.</param>
        /// <returns> An <see cref="Inline"/> value for a <see cref="TextBlock"/> </returns>
        public static Inline Process(string value)
        {
            var doc = new XmlDocument();
            doc.LoadXml(value);

            var span = new Span();
            InternalProcess(span, doc.ChildNodes[0]);

            return span;
        }

        /// <summary>
        /// Analyzes a <see cref="XmlNode"/> to transform the XML bold tags to be understood by the <see cref="TextBlock"/>.
        /// </summary>
        /// <param name="span"> A <see cref="Span"/> object.</param>
        /// <param name="xmlNode"> The <see cref="XmlNode"/> to analyze.</param>
        public static void InternalProcess(Span span, XmlNode xmlNode)
        {
            foreach (XmlNode child in xmlNode)
            {
                if (child is XmlText)
                {
                    span.Inlines.Add(new Run(child.InnerText));
                }

                else if (child is XmlElement && (child.Name.ToUpper() == "B" || child.Name.ToUpper() == "BOLD"))
                {
                    var boldSpan = new Span();
                    InternalProcess(boldSpan, child);
                    var bold = new Bold(boldSpan);
                    span.Inlines.Add(bold);
                }
            }
        }
    }
}
