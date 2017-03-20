// --------------------------------------------------------------------------------------------------
//  <copyright file="MarkupHelper.cs" company="RHEA System S.A.">
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
    using System.Reflection;
    using System.IO;
    using System.Text;

    /// <summary>
    /// MarkupHelper provides methods to help with transformations on HTML and other markup.
    /// </summary>
    public static class MarkupHelper
    {
        private const string ResourceName = "DocuPanel.Resources.css.github-markdown.css";

        /// <summary>
        /// Wraps the given string into valid html and body tags.
        /// </summary>
        /// <param name="bodyText">The body markup.</param>
        /// <returns>The full html document.</returns>
        public static string WrapHtmlBody(string bodyText)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<html><head></head><body>");

            sb.AppendLine(string.Format("<style>{0}</style><article class=\"markdown-body\">", ReadInStyle()));
            sb.AppendLine(bodyText);
            sb.AppendLine("</article></body></html>");

            return sb.ToString();
        }

        /// <summary>
        /// Reads in the style file.
        /// </summary>
        /// <returns>The full css of the style file.</returns>
        private static string ReadInStyle()
        {
            var content = "";
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                using (var stream = assembly.GetManifestResourceStream(ResourceName))
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            content = reader.ReadToEnd();
                        }
                    }
            }

            catch (Exception e)
            {
                Console.Error.WriteLine(e);
                throw;
            }
            
            return content;
        }
    }
}
