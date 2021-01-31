# Installation

## Visual Studio 2019

<https://visualstudio.microsoft.com/downloads/>

Use Visual Studio 2019 to vissualy inspect code, and to write and debug code.
The syntax tree debugging experience is much better in Visual Studio 2019 compared to Visual Studio Code.

### Syntax Visualizer and DGML editor

Use these tools to visually inspect syntax trees in Visual Studio.

#### Workload

* Install the **.NET Compiler Platform SDK** workload.
* Add the DGML editor under individual components (**Code tools** section)

#### Individual components

* Add the **.NET Compiler Platform SDK** (**Compilers, build tools, and runtimes** section)
* Add the DGML editor under individual components (*Code tools* section)

More information about these tools can be found in the article [Explore code with the Roslyn syntax visualizer in Visual Studio](https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/syntax-visualizer?tabs=csharp).

## Visual Studio Code

<https://code.visualstudio.com/Download>

Use Visual Studio Code to write Markdown, PlantUML and AsciiDoc files.

### Plugin: AsciiDoc

`ext install asciidoctor.asciidoctor-vscode`

<https://marketplace.visualstudio.com/items?itemName=asciidoctor.asciidoctor-vscode>

#### Extension Settings

```json
"asciidoc.preview.useEditorStyle": false
```

### Plugin: PlantUML

`ext install jebbs.plantuml`

<https://marketplace.visualstudio.com/items?itemName=jebbs.plantuml>

#### Settings

```json
"plantuml.render": "PlantUMLServer",
"plantuml.server": "https://www.plantuml.com/plantuml",
```

For example code it's fine to use the plublic renderer. You can render locally by running a PlantUML server locally.
For example using Docker:

```sh
docker run -d -p 8080:8080 plantuml/plantuml-server:jetty
```

## AsciiDocFX

<https://asciidocfx.com/#truehow-to-install-asciidocfx>

With AsciiDocFX you can edit, view and export AsciiDoc files.

### Graphviz

For some diagrams, you can get an exception that graphviz is required.

Pick any of these to install:

* <https://www2.graphviz.org/Packages/stable/windows/10/cmake/Release/x64/graphviz-install-2.44.1-win64.exe>
  
  Make sure the executable is in you path, f.e. `C:\Graphviz\bin`
* `> choco install graphviz`
* `> winget install graphviz`

## Docker Desktop

Optional, but can replace remote or local dependencies like PlantUML Server and AsciiDoctor.

<https://www.docker.com/products/docker-desktop>
