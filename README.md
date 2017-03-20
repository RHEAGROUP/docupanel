# DocuPanel #
A WPF markdown/html Documentation Control with search that can be used to create a help facility in WPF applications. The help pages are written using markdown and the structure of the help file is created using a json document.

## Installation ##

NuGet available at https://www.nuget.org/packages/DocuPanel/

## Usage ##

### Quick Start ###
To add DocuPanel to you project simply add this code block inside your view
``` xaml
<docuPanel:DocumentationView
  PathDocumentationIndex="{Binding DataContext.PathDocumentationIndex, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged, ElementName=MWindow}"
  RootAppData="{Binding DataContext.AppDataRoot, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged, ElementName=MWindow}"
  UpdateIndexation="{Binding DataContext.UpdateIndexation, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged, ElementName=MWindow}"/>
```

**PathDocumentationIndex** `string` which corresponds to the path of the index file of your documentation. This file must be a `.json` file and be present at the root of the documentation.

**RootAppData** `string` which corresponds to the path of the application data folder of your application.  
DocuPanel will create on this path a directory called `DocuPanel` to store its datas.  

**UpdateIndexation** `bool` which indicates whether the indexation needs to be updated.  
If true, DocuPanel will browse all the files present in the index, and will convert them into HTML if they don't already exist. Note that if you want to update the content of a file, you have to delete the html file from the application data folder. The indexation for the searches will also be updated with the new documentation content.

### Structure of your documentation ###
Your documentation files should be present inside your project directory and can be ordered the way you want. However they must have different names.

`Pages` should have the following properties  

| Build Action | Copy to Outpout Directory |
|:-------------|:--------------------------|
| content      | Copy if newer             |

The `index` must be `.json` file and should have the properties

| Build Action | Copy to Outpout Directory |
|:-------------|:--------------------------|
| None         | Copy always               |

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

**Title** is the title of you documentation.  
**Author** is the author of the documentation. Can be empty.  
**PagePath** is the path of the page. Note that a section does not necessary contains PagePath. For example a section can contains only children pages, it's what happens with *Installation* and *Configuration* in our example.    
**Sections** is the list of the subsections.   
**Name** is the name displayed by DocuPanel. It is possible to have two sections with the same name.

### Sample ###
You can see a sample which implements DocuPanel in the directory [DocuPanelSupport](https://github.com/RHEAGROUP/docupanel/DocuPanelSupport).

## How can I Contribute ##
If you have any idea to improve the DocuPanel your help is welcomed.  
Feel free to contribute and don't hesitate to contact us or create a pull request !

## Sponsors ##
The docupanel is sponsored by the [RHEA GROUP](https://www.rheagroup.com/)
