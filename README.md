# Living Documentation Workshop

Hello, welcome to the Living Documentation Workshop.

We are going to discover how we can use Roslyn to generate documentation from .NET source code.

## Prerequisites

See [INSTALLATION.md](INSTALLATION.md) for details about all prerequisites.

## Download this workshop

Clone this workshop on your local system.

```sh
cd c:\git
git clone "https://github.com/eNeRGy164/LivingDocumentation.Workshop.git"
```

## Structure

### Part 1: Roslyn

* [Get familiar with Syntax Trees](1.roslyn/11.visual-trees.md)

  Visually inspect the syntax tree of an application.

* [Parse source code](1.roslyn/12.parse-trees.md)

  Parse a syntax tree and inspect the structure.

* Compile source code

  Compile a syntax tree and inspect the semantic model.

* Walk the tree

  Use the visitor pattern to act on specific parts of your tree.

* Load a whole project

  Use [Buildalyzer](https://github.com/daveaglick/Buildalyzer) to import a whole project or solution to inspect.

### Part 2: Generate documentation

* Living Documentation

  Quickly start acting on your code by generating an intermediate file with all relevant parts extracted from the syntax trees.

* Generate documentation

  Build a document describing components based on the parts of you code.

* Generate a class diagram

  Build a class diagram from a POCO.

* Generate a sequence diagram

  Build a sequence diagram from services interacting with each other.

* Generate an entity diagram

  Extend on class diagrams including relations with other parts.
