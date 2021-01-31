using System;
using System.IO;
using System.Linq;
using Buildalyzer;
using Buildalyzer.Workspaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynReadProject
{
    class Program
    {
        static void Main(string[] args)
        {
            var analyzerManager = new AnalyzerManager();
            var projectAnalyzer = analyzerManager.GetProject(@"D:\workshop\ConsoleApp1\ConsoleApp1\ConsoleApp1.csproj");

            var analyzerResults = projectAnalyzer.Build().First();

            Console.WriteLine($"{analyzerResults.References.Length} references");
            Console.WriteLine($"{analyzerResults.SourceFiles.Length} source files");

            Console.WriteLine();
            foreach (var file in analyzerResults.SourceFiles)
            {
                Console.WriteLine(file);
            }

            var workspace = projectAnalyzer.GetWorkspace();

            var project = workspace.CurrentSolution.Projects.First();
            var compilation = project.GetCompilationAsync().Result;

            var diagnostics = compilation.GetDiagnostics();

            foreach (var error in diagnostics.Where(l => l.Severity > DiagnosticSeverity.Hidden))
            {
                Console.WriteLine(error);
            }

            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                var root = (CompilationUnitSyntax)syntaxTree.GetRoot();
                var text = root.GetText();

                Console.WriteLine();
                Console.WriteLine(Path.GetFileName(syntaxTree.FilePath));
                Console.WriteLine(text.GetSubText(root.Span));

                var semanticModel = compilation.GetSemanticModel(syntaxTree);
            }
        }
    }
}
