using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LivingDocumentation;
using Newtonsoft.Json;

namespace PitstopDocumentationRenderer
{
    class Program
    {
        internal static List<TypeDescription> Types;

        static void Main(string[] args)
        {
            var fileContents = File.ReadAllText(@"D:\workshop\pitstop\pitstop.analyzed.json");

            Types = JsonConvert.DeserializeObject<List<TypeDescription>>(fileContents, JsonDefaults.DeserializerSettings()).ToList();

            var type = Types.FirstOrDefault("Pitstop.TimeService.Events.DayHasPassed");

            Types.PopulateInheritedBaseTypes();
            Types.PopulateInheritedMembers();

            Console.WriteLine("Base types:");
            foreach (var baseType in type.BaseTypes)
            {
                Console.WriteLine($"- {baseType}");
            }
            Console.WriteLine();

            Console.WriteLine("Fields:");
            foreach (var field in type.Fields)
            {
                Console.WriteLine($"- {field.Name}");
            }

            Console.WriteLine();
            Console.WriteLine("Commands:");
            var commands = Types.Where(t => t.ImplementsType("Pitstop.Infrastructure.Messaging.Command"));
            foreach (var command in commands)
            {
                Console.WriteLine($"- {command.FullName}");
            }

            Console.WriteLine();
            Console.WriteLine("Events:");
            var @events = Types.Where(t => t.ImplementsType("Pitstop.Infrastructure.Messaging.Event"));
            foreach (var @event in @events)
            {
                Console.WriteLine($"- {@event.FullName}");
            }


            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Command Handlers:");
            var commandHandlers = Types
                .Where(t => t.IsClass() && t.Methods.Any(m => m.Parameters.Any(p => p.Attributes.Any(a => a.Type.Equals("Microsoft.AspNetCore.Mvc.FromBodyAttribute")))))
                .ToDictionary(t => t.FullName, t => t.Methods.Where(m => m.Parameters.Any(p => p.Attributes.Any(a => a.Type.Equals("Microsoft.AspNetCore.Mvc.FromBodyAttribute")))).Select(m => Types.First(m.Parameters.Last().Type).Name).ToList());

            foreach (var kv in commandHandlers)
            {
                Console.WriteLine(kv.Key);

                foreach (var @namespace in kv.Value)
                {
                    Console.WriteLine($"- {@namespace}");
                }

                Console.WriteLine();
            }

            var eventHandlerClasses = Types
                .Where(t => t.IsClass() && t.ImplementsType("Pitstop.Infrastructure.Messaging.IMessageHandlerCallback"))
                .Where(t => t.Methods.Any(m => m.Name == "HandleAsync" && m.Parameters.Any(p => Types.First(p.Type).ImplementsType("Pitstop.Infrastructure.Messaging.Event"))))
                .Select(t => (EventHandlerClass: t, Events: t.Methods.Where(m => m.Name == "HandleAsync" && m.Parameters.Any(p => Types.First(p.Type).ImplementsType("Pitstop.Infrastructure.Messaging.Event"))).Select(m => Types.First(m.Parameters[0].Type).Name).ToList()));

            // Pivot events and handlers
            var query = from ehc in eventHandlerClasses
                        from e in ehc.Events
                        group ehc.EventHandlerClass.Namespace by e into g
                        select g;

            Console.WriteLine();
            Console.WriteLine("Event Receiving Services:");
            foreach (var group in query)
            {
                Console.WriteLine(group.Key);

                foreach (var @namespace in group)
                {
                    Console.WriteLine($"- {@namespace}");
                }

                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine("Trace Statements:");
            var customerController = Types.First("Pitstop.Application.CustomerManagementAPI.Controllers.CustomersController");
            var handlingMethod = customerController.Methods.First(m => m.Name == "RegisterAsync" && m.Parameters[0].Type == "Pitstop.CustomerManagementAPI.Commands.RegisterCustomer");
            var messagePublishStatements = handlingMethod.Statements.SelectMany(s => FlattenStatements(s).OfType<InvocationDescription>().Where(s => s.Name == "PublishMessageAsync"));

            foreach (var statement in messagePublishStatements)
            {
                Console.WriteLine($"When a RegisterCustomer command is handled, a {Types.First(statement.Arguments[1].Type).Name} event will be published.");
            }
        }

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
    }
}
