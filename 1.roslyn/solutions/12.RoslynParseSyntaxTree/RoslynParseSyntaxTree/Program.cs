using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynParseSyntaxTree
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

            var @using = root.Usings.First();
            var @namespace = root.Members.OfType<NamespaceDeclarationSyntax>().First();
            var @class = @namespace.Members.OfType<ClassDeclarationSyntax>().First();
            var method = @class.Members.OfType<MethodDeclarationSyntax>().First();
            var expressionStatement = method.Body.Statements.OfType<ExpressionStatementSyntax>().First();

            #region optional
            var invocationExpression = expressionStatement.Expression as InvocationExpressionSyntax;
            var memberAccessExpression = invocationExpression.Expression as MemberAccessExpressionSyntax;
            #endregion

            var sourceText = tree.GetText();
            Console.WriteLine(sourceText.GetSubText(@using.Span));
            Console.WriteLine(sourceText.GetSubText(expressionStatement.Span));

            #region optional
            Console.WriteLine(sourceText.GetSubText(memberAccessExpression.Expression.Span));
            Console.WriteLine(sourceText.GetSubText(memberAccessExpression.Name.Span));
            #endregion

            #region awesomesauce
            var argument = invocationExpression.ArgumentList.Arguments.First();
            Console.WriteLine(sourceText.GetSubText(argument.Span));
            #endregion
        }
    }
}
