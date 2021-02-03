using System;
using System.Collections.Generic;
using System.Linq;
using LivingDocumentation;

namespace PitstopDocumentationRenderer
{
    internal static class TypeDescriptionListExtensions
    {
        public static IReadOnlyList<TypeDescription> HandlersFor(this IEnumerable<TypeDescription> types, TypeDescription type)
        {
            var eventHandlers = types.EventHandlersFor(type);
            var commandHandler = types.CommandHandlerFor(type);

            if (commandHandler == null)
            {
                return eventHandlers;
            }

            return eventHandlers.Concat(new[] { commandHandler }).ToList();
        }

        /// <summary>
        /// Returns the type with a method handling <paramref name="type"/> as a command.
        /// </summary>
        public static TypeDescription CommandHandlerFor(this IEnumerable<TypeDescription> types, TypeDescription type)
        {
            return types
                .FirstOrDefault(t => t.IsClass() && t.Methods.Any(m => m.Parameters.Any(p => p.Type == type.FullName && p.Attributes.Any(a => string.Equals(a.Type, "Microsoft.AspNetCore.Mvc.FromBodyAttribute", StringComparison.Ordinal)))));
        }

        /// <summary>
        /// Returns the types with a method handling <paramref name="type"/> as an event.
        /// </summary>
        public static IReadOnlyList<TypeDescription> EventHandlersFor(this IEnumerable<TypeDescription> types, TypeDescription type)
        {
            return types
                .Where(t => t.IsClass() && t.ImplementsType("Pitstop.Infrastructure.Messaging.IMessageHandlerCallback"))
                .Where(t => t.Methods.Any(m => m.Name == "HandleAsync" && m.Parameters.Any(p => p.Type.EndsWith("." + type.Name, StringComparison.Ordinal))))
                .ToList();
        }

        public static IReadOnlyList<TypeDescription> GetSourceCommands(this IEnumerable<TypeDescription> types, TypeDescription type)
        {
            var commands =
                from t in types
                from m in t.Methods
                from s in m.Statements
                from i in FlattenStatements(s).OfType<InvocationDescription>()
                where i.ContainingType.EndsWith(type.Name) && i.Name == type.Name
                select types.First(i.ContainingType);

            return commands.ToList();
        }

        /// <summary>
        /// Returns a flat list of statements, including those part of groups like if/else/foreach blocks.
        /// </summary>
        private static IEnumerable<Statement> FlattenStatements(Statement sourceStatement, List<Statement> statements = null)
        {
            if (statements == null)
            {
                statements = new List<Statement>();
            }

            switch (sourceStatement)
            {
                case InvocationDescription invocationDescription:
                    statements.Add(invocationDescription);
                    break;

                case Switch sourceSwitch:
                    foreach (var statement in sourceSwitch.Sections.SelectMany(s => s.Statements))
                    {
                        FlattenStatements(statement, statements);
                    }
                    break;

                case If sourceIf:
                    foreach (var statement in sourceIf.Sections.SelectMany(s => s.Statements))
                    {
                        FlattenStatements(statement, statements);
                    }
                    break;

                case Statement statementBlock:
                    foreach (var statement in statementBlock.Statements)
                    {
                        FlattenStatements(statement, statements);
                    }
                    break;
            }

            return statements;
        }

        /// <summary>
        /// Get consequences of an invocation.
        /// </summary>
        /// <remarks>
        /// Will try to use the implementation of an interface.
        /// </remarks>
        public static IReadOnlyList<Statement> GetInvocationConsequenceStatements2(this IEnumerable<TypeDescription> types, InvocationDescription invocation)
        {
            InvocationDescription implementatedInvocation = null;

            // If the type is an interface, look for the first implementation of that interface and act as if that one was invoked.
            if (Program.Types.FirstOrDefault(invocation.ContainingType)?.Type == TypeType.Interface)
            {
                var implementation = Program.Types.First(t => t.ImplementsType(invocation.ContainingType));
                implementatedInvocation = new InvocationDescription(implementation.FullName, invocation.Name);
                implementatedInvocation.Arguments.AddRange(invocation.Arguments);
            }

            var statements = types.GetInvokedMethod(implementatedInvocation ?? invocation)
                .SelectMany(m => m.Statements);

            var consequences = new List<Statement>();

            foreach (var statement in statements)
            {
                var innerStatements = TraverseStatement(types, statement);
                if (innerStatements.Count > 0)
                {
                    consequences.AddRange(innerStatements);
                }
                else
                {
                    consequences.Add(statement);
                }
            }

            return consequences;
        }

        /// <summary>
        /// Returns all statements including those nested in foreach, switch and if blocks keeping hiearchy intact.
        /// </summary>
        private static IReadOnlyList<Statement> TraverseStatement(this IEnumerable<TypeDescription> types, Statement sourceStatement)
        {
            switch (sourceStatement)
            {
                case ForEach forEachStatement:
                    {
                        var destinationForEach = new ForEach();
                        foreach (var statement in forEachStatement.Statements)
                        {
                            destinationForEach.Statements.AddRange(types.TraverseStatement(statement));
                        }

                        destinationForEach.Expression = forEachStatement.Expression;

                        return new List<Statement> { destinationForEach };
                    }

                case Switch sourceSwitch:
                    var destinationSwitch = new Switch();
                    foreach (var switchSection in sourceSwitch.Sections)
                    {
                        var section = new SwitchSection();
                        section.Labels.AddRange(switchSection.Labels);

                        foreach (var statement in switchSection.Statements)
                        {
                            section.Statements.AddRange(types.TraverseStatement(statement));
                        }

                        destinationSwitch.Sections.Add(section);
                    }

                    destinationSwitch.Expression = sourceSwitch.Expression;

                    return new List<Statement> { destinationSwitch };

                case If sourceIf:
                    var destinationÍf = new If();

                    foreach (var ifElseSection in sourceIf.Sections)
                    {
                        var section = new IfElseSection();

                        foreach (var statement in ifElseSection.Statements)
                        {
                            section.Statements.AddRange(types.TraverseStatement(statement));
                        }

                        section.Condition = ifElseSection.Condition;

                        destinationÍf.Sections.Add(section);
                    }

                    return new List<Statement> { destinationÍf };

                default:
                    return new List<Statement>(0);
            }
        }
    }
}
