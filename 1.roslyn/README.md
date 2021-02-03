# Roslyn

Welcome in the first part of this workshop.

This part is to get you familair with Roslyn, or *.NET Compiler Platform SDK* as it is officially called these days.

This part is not necesarry to be able to work on the second part of this workshop.
However, it helps you to understand how you can use Roslyn to analyze the structure and parts of your source codea and use it for your own solution.

If you are already familair with Roslyn, you get through this part quickly and take your time for the second part of this workshop.

If Roslyn is new for you, each chapter will add more to your understanding how it works and how you can get more out of it.

Some of the chapters might have *optional* and *awesomesauce* assignments.
Those can be used to test your understanding of the technology, but can be skipped for time.

Every chapter has a solution that can be used to verify your own work, or to mobe forward if you get stuck.

## Structure

* [Get familiar with Syntax Trees](11.visual-trees.md)

  Visually inspect the syntax tree of an application.

* [Parse Source Code](12.parse-trees.md)

  Parse a syntax tree and inspect the structure.

* [Compile Source Code](13.compile-code.md)

  Compile a syntax tree and inspect the semantic model.

* [Walk a Syntax Tree](14.walk-trees.md)

  Use the visitor pattern to act on specific parts of your tree.

* [Load a Project or Solution](15.load-a-project.md)

  Use [Buildalyzer](https://github.com/daveaglick/Buildalyzer) to import a whole project or solution to work with.
