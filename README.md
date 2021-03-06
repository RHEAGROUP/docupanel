# DocuPanel #
[![NuGet Stats](https://img.shields.io/badge/nuget-v0.3.0-blue.svg)](https://www.nuget.org/packages/DocuPanel/)

A WPF markdown/html Documentation Control with search that can be used to create a help facility in WPF x64 applications. The help pages are written using markdown and the structure of the help file is created using a json document.

## Installation ##

NuGet available at https://www.nuget.org/packages/DocuPanel/

## Usage ##
To have a better understanding of DocuPanel we recommend you to read [our documentation](https://rheagroup.gitbooks.io/docupanel/).

### Quick Start ###
To add DocuPanel to you project you can read this [tutorial](https://www.codeproject.com/Articles/1177702/Display-your-Markdown-documentation-using-DocuPane) or read the following instructions.

First add this code block with your own values inside your view
``` xaml
<docuPanel:DocumentationView
            PathDocumentationIndex="C:\Projects\DocuPanel\DocuPanelSupport\bin\x64\Debug\Documentation\book.json"
            RootAppData="Users\<userName>\AppData\Local\<YourAppDataFolder>"
            UpdateIndexation="true"/>
```

**PathDocumentationIndex** `string` which corresponds to the path of the index file of your documentation. This file must be a `.json` file and be present at the root of the documentation.

**RootAppData** `string` which corresponds to the path of the application data folder of your application.  
DocuPanel will create on this path a directory called `DocuPanel` to store its datas.  

**UpdateIndexation** `bool` which indicates whether the indexation needs to be updated.  
If true, DocuPanel will browse all the files indicated in the index, and will convert them into HTML if they don't already exist. DocuPanel converts your Markdown files into HTML files to be displayed by CefSharp. The indexation for the searches will also be updated with the new documentation content. Note that if you want to update the content of a file, to consider the changes you have to delete the HTML file from the application data folder. This property needs to be `true` the first time you use DocuPanel and each time you make changes in your documentation.

### Structure of your documentation ###
Your documentation files should be present inside your project directory and can be ordered the way you want. However they must have different names.

`Pages` should have the following properties  

| Build Action | Copy to Outpout Directory |
|:-------------|:--------------------------|
| None         | Copy if newer             |

The `index` must be `.json` file and should have the same properties.

It has to be structured as the following example
``` json
{
  "Title": "Documentation",
  "Author": "RHEA System S.A.",
  "PagePath": "index.md",
  "Sections": [
    {
      "Name": "Quick Start",
      "PagePath": "quickstart_Quick_Start.md"
    },
    {
      "Name": "Installation",
      "Sections": [
        {
          "Name": "Introduction",
          "PagePath": "Installation\\1_installintro_Introduction.md"
        },
        {
          "Name": "Configuration",
          "Sections": [
            {
              "Name": "Step by Step",
              "PagePath": "Installation\\2_installstep_Step_by_Step.md"
            },
          ]
        }
      ]
    }
  ]
}
```
The structure of the previous code gives

![Image](https://github.com/RHEAGROUP/docupanel/blob/master/hierarchy.PNG)

**Title** is the title of your documentation.  
**Author** is the author of the documentation. Can be empty.  
**PagePath** is the path of the page. Note that a section does not necessary contains PagePath. For example a section can contain only children pages, it's what happens with *Installation* and *Configuration* in our example.    
**Sections** is the list of the subsections.   
**Name** is the name displayed by DocuPanel. It is possible to have two sections with the same name.

### Sample ###
You can see a sample which implements DocuPanel in the directory [DocuPanelSupport](https://github.com/RHEAGROUP/docupanel/tree/master/DocuPanelSupport).

## How can I Contribute ##
If you have any idea to improve the DocuPanel your help is welcomed.  
Feel free to contribute and don't hesitate to contact us or create a pull request !

## Sponsors ##
The docupanel is sponsored by the [RHEA GROUP](https://www.rheagroup.com/)
