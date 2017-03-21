# How does it work

DocuPanel is a wpf MVVM library which aims to display properly a documentation written in Markdown and make possible searches.

Below are described the steps followed by DocuPanel

1. Initialization of the basic components of the user interface
2. Conversion of the Markdown files into HTML files \(if `UpdateIndexation` is true\)
3. Indexation of the content to perform searches \(if `UpdateIndexation` is true\)
4. Display the user interface initialized

DocuPanel is composed of a treeView which prints the sections, a search bar which allows searches, and the Chromium Browser [CefSharp](https://cefsharp.github.io/) to display the HTML pages. During the initialization, DocuPanel loads frame, the search bar and the Chromium Browser with the different commands and buttons.

Then the value of the property_UpdateIndexation_is checked.

If true the index file is deserialized into a book object using [Json.Net](http://www.newtonsoft.com/json). From this book object, DocuPanel retrieves the information of the documentation and the path of the files which should be indexed. It then browses the files and converts them into HTML if they not already exist. The conversion is realized using the package [MarkdownDeep](http://www.toptensoftware.com/markdowndeep/).

Once this step is over DocuPanel realizes the indexation of the content using the package [Lucene.Net](https://lucenenet.apache.org/). The name and the path of each page are stored in the RAM to allow a instant search which provides suggestion to the user. If_UpdateIndexation_is true, the name and the path of each page are also stored in a file system directory and the content of the pages is tokenized to be searchable.

Finally the sections which need to be displayed at the beginning are created and the user interface loaded. DocuPanel can now be used.

