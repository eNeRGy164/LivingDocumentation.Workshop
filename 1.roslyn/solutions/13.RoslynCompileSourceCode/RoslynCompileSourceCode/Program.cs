using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynCompileSourceCode
{
    class Program
    {
        static void Main(string[] args)
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(""Hello World!"");
        }
    }
}");

            var root = (CompilationUnitSyntax)tree.GetRoot();

            var runtimeLocation = Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location), "System.Runtime.dll");

            var compilation = CSharpCompilation
                .Create("Workshop")
                .AddReferences(MetadataReference.CreateFromFile(runtimeLocation))
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(Console).Assembly.Location))
                .AddSyntaxTrees(tree);

            var diagnostics = compilation.GetDiagnostics();

            foreach (var error in diagnostics.Where(l => l.Severity > DiagnosticSeverity.Hidden))
            {
                Console.WriteLine(error);
            }

            var invocationExpression = root
                .Members.OfType<NamespaceDeclarationSyntax>().First()
                .Members.OfType<ClassDeclarationSyntax>().First()
                .Members.OfType<MethodDeclarationSyntax>().First()
                .Body.Statements.OfType<ExpressionStatementSyntax>().First()
                .Expression as InvocationExpressionSyntax;

            var semanticModel = compilation.GetSemanticModel(tree);

            var methodSymbol = semanticModel.GetSymbolInfo(invocationExpression).Symbol;

            Console.WriteLine(methodSymbol);
            Console.WriteLine(methodSymbol.ContainingType);
            Console.WriteLine(methodSymbol.ContainingType.ContainingAssembly);

            #region awesomesauce
            Console.WriteLine();

            var displayParts = methodSymbol.ToDisplayParts();
            foreach (var part in displayParts)
            {
                Console.WriteLine(string.Format("{0,-13} = {1}", part.Kind, part));
            }
            #endregion
        }
    }
}
