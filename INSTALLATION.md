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

## Docker Desktop

Optional, but can replace remote or local dependencies like PlantUML Server.

<https://www.docker.com/products/docker-desktop>
