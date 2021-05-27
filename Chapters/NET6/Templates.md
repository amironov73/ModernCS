### Поиск шаблонов приложений

Preview 4 introduces a new search capability for templates. dotnet new --search will search NuGet.org for matching templates. During upcoming previews the data used for this search will be updated more frequently.

Templates installed in the CLI are available for both the CLI and Visual Studio. An earlier problem with user installed templates being lost when a new version of the SDK was installed has been resolved, however templates installed prior to .NET 6 Preview 4 will need to be reinstalled.

Other improvements to template installation include support for the --interactive switch to support authorization credentials for private NuGet feeds.

Once CLI templates are installed, you can check if updates are available via --update-check and --update-apply. This will now reflect template updates much more quickly, support the NuGet feeds you have defined, and support --interactive for authorization credentials.

In Preview 4 and upcoming Previews, the output of dotnet new commands will be cleaned up to focus on the information you need most. For example, the dotnet new --install <package> lists only the templates just installed, rather than all templates.

To support these and upcoming changes to dotnet new, we are making significant changes to the Template Engine API that may affect anyone hosting the template engine. These changes will appear in Preview 4 and Preview 5. If you are hosting the template engine, please connect with us at https://github.com/dotnet/templating so we can work with you to avoid or minimize disruption.

```
> dotnet new xml --search
Searching for the templates...
Matches from template source: NuGet.org
 
Template Name                            Short Name               Author  Language  Package                    Downloads
---------------------------------------  -----------------------  ------  --------  -------------------------  ---------
RTI Connext DDS XML-defined Application  dds-xml-console          RTI     [C#]      Rti.ConnextDds.Templates          1k
Soneta Addon Item BusinessXml            soneta-item-businessxml  Soneta  [C#]      Soneta.Platform.Developer         1k
 
 
To use the template, run the following command to install the package: dotnet new -i <package>
Example:
        dotnet new -i Rti.ConnextDds.Templates
```
