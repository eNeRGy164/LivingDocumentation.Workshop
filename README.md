# Living Documentation Workshop

Hello, welcome to the Living Documentation Workshop.

We are going to discover how we can use Roslyn to generate documentation from .NET source code.

## Prerequisites

See [INSTALLATION.md](INSTALLATION.md) for details about all prerequisites.

## Required Knowledge

* Be very experienced with C#
* Able to write LINQ queries
* Know how to clone git repos
* Have an understanding of DDD
* Have knowledge of Markdown
* Able to draw UML diagrams

Optional knowledge:

* Familair with AsciiDoc and PlantUML

## Download this Workshop

Clone this workshop on your local system.

```sh
cd c:\git
git clone "https://github.com/eNeRGy164/LivingDocumentation.Workshop.git"
```

## Structure

### Part 1: [Roslyn](1.roslyn/README.md)

* [Get familiar with Syntax Trees](1.roslyn/11.visual-trees.md)

  Visually inspect the syntax tree of an application.

* [Parse Source Code](1.roslyn/12.parse-trees.md)

  Parse a syntax tree and inspect the structure.

* [Compile Source Code](1.roslyn/13.compile-code.md)

  Compile a syntax tree and inspect the semantic model.

* [Walk a Syntax Tree](1.roslyn/14.walk-trees.md)

  Use the visitor pattern to act on specific parts of your tree.

* [Load a Project or Solution](1.roslyn/15.load-a-project.md)

  Use [Buildalyzer](https://github.com/daveaglick/Buildalyzer) to import a whole project or solution to work with.

### Part 2: [Generate documentation](2.living-documentation/README.md)

* [Living Documentation](2.living-documentation/21.living-documentation.md)

  Quickly start acting on your code by generating an intermediate file with all relevant parts extracted from the syntax trees.

* [Work with Types](2.living-documentation/22.work-with-types.md)

  Get familiar with the types and start querying relations between classes, methods, invocations, etc.

* [Generate documentation](2.living-documentation/23.generate-documentation.md)

  Build a document describing components based on the parts of you code.

* [Generate a Class Diagram](2.living-documentation/24.generate-class-diagram.md)

  Build a class diagram from an aggregate.

* Generate a sequence diagram

  Build a sequence diagram from services interacting with each other.
