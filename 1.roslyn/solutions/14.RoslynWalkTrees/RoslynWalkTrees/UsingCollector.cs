using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynWalkTrees;

class UsingCollector : CSharpSyntaxWalker
{
    public readonly List<UsingDirectiveSyntax> Usings = new();

    public override void VisitUsingDirective(UsingDirectiveSyntax node)
    {
        if (node.Name.ToString() != "System" &&
            !node.Name.ToString().StartsWith("System."))
        {
            this.Usings.Add(node);
        }
    }
}
