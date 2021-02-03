using System;
using System.Collections.Generic;
using System.Linq;
using LivingDocumentation;
using LivingDocumentation.Uml;

namespace PitstopDocumentationRenderer
{
    internal class InteractionTraverser
    {
        private readonly Stack<string> activations = new Stack<string>();

        /// <summary>
        /// Extracts all consequences of a message being handled.
        /// </summary>
        /// <remarks>
        /// Keeps track of all <paramref name="services"/> that are passed with interactions.
        /// </remarks>
        public Interactions ExtractConcequences(TypeDescription originatingMessage, List<string> services, string previousService = null, string inAlternativeFlow = null, List<ArgumentDescription> arguments = null)
        {
            var result = new Interactions();

            var handlers = Program.Types.HandlersFor(originatingMessage);

            foreach (var handler in handlers)
            {
                var levelName = Service(handler);

                var source = previousService ?? "A";
                var target = levelName ?? "Q";

                var arrow = new Arrow
                {
                    Source = source,
                    Target = target,
                    Name = originatingMessage.Name,
                    Color = ArrowColor(originatingMessage)
                };
                result.AddFragment(arrow);

                if (!activations.Contains(levelName))
                {
                    activations.Push(levelName);
                }

                if (!services.Contains(target)) services.Add(target);

                var statements = HandlingMethod(handler, originatingMessage).Statements;
                foreach (var statement in statements)
                {
                    var statementInteractions = TraverseBody(services, handler, previousService ?? levelName, statement, inAlternativeFlow ?? levelName);
                    if (statementInteractions.Fragments.Count > 0) result.AddFragments(statementInteractions.Fragments);
                }

                if (activations.Count > 0 && activations.Peek() == levelName && levelName != inAlternativeFlow)
                {
                    activations.Pop();
                }
            }

            return result;
        }

        /// <summary>
        /// Traverse the statement and look of resulting interactions that are relevant for the sequence diagram.
        /// </summary>
        /// <remarks>
        /// Tracks flow over services, even if the same service becomes part of a different resulting event.
        /// </remarks>
        private Interactions TraverseBody(List<string> services, TypeDescription handler, string serviceName, Statement statement, string inAlternativeFlow)
        {
            switch (statement)
            {
                case InvocationDescription invocation when IsMessageCreation(invocation):

                    var message = Program.Types.First(invocation.Arguments.Skip(invocation.Name == "RaiseEvent" ? 0 : 1).First().Type);

                    return ExtractConcequences(message, services, Service(handler), inAlternativeFlow, invocation.Arguments);

                case InvocationDescription invocation:
                    {
                        var result = new Interactions();
                        var consequences = Program.Types.GetInvocationConsequenceStatements2(invocation).Where(s => s != invocation);

                        foreach (var consequence in consequences)
                        {
                            var invocationInteractions = TraverseBody(services, handler, serviceName, consequence, inAlternativeFlow);
                            if (invocationInteractions.Fragments.Count > 0) result.AddFragments(invocationInteractions.Fragments);
                        }

                        return result;
                    }

                case ForEach forEachStatement:
                    {
                        var result = new Interactions();
                        var interactions = new List<InteractionFragment>();

                        foreach (var invocation in forEachStatement.Statements)
                        {
                            var invocationInteractions = TraverseBody(services, handler, serviceName, invocation, inAlternativeFlow);
                            if (invocationInteractions.Fragments.Count > 0) interactions.AddRange(invocationInteractions.Fragments);
                        }

                        if (interactions.Count > 0)
                        {
                            var alt = new Alt();

                            var altSection = new AltSection();
                            altSection.AddFragments(interactions);
                            altSection.GroupType = "forEach";
                            altSection.Label = forEachStatement.Expression;

                            alt.AddSection(altSection);

                            result.AddFragment(alt);
                        }

                        return result;
                    }

                case Switch switchStatement:
                    {
                        var result = new Interactions();

                        var alt = new Alt();

                        foreach (var section in switchStatement.Sections)
                        {
                            var interactions = new List<InteractionFragment>();

                            foreach (var invocation in section.Statements)
                            {
                                var invocationInteractions = TraverseBody(services, handler, serviceName, invocation, inAlternativeFlow);
                                if (invocationInteractions.Fragments.Count > 0) interactions.AddRange(invocationInteractions.Fragments);
                            }

                            if (interactions.Count > 0)
                            {
                                var altSection = new AltSection();
                                if (alt.Sections.Count == 0) { altSection.GroupType = "case"; };
                                altSection.AddFragments(interactions);
                                altSection.Label = section.Labels.Aggregate(string.Empty, (s1, s2) => s1 + s2);
                                alt.AddSection(altSection);
                            }
                        }

                        if (alt.Sections.Count > 0)
                        {
                            result.AddFragment(alt);
                        }

                        return result;
                    }

                case If ifStatement:
                    {
                        var result = new Interactions();

                        var alt = new Alt();

                        foreach (var section in ifStatement.Sections)
                        {
                            var interactions = new List<InteractionFragment>();

                            foreach (var invocation in section.Statements)
                            {
                                var invocationInteractions = TraverseBody(services, handler, serviceName, invocation, inAlternativeFlow);
                                if (invocationInteractions.Fragments.Count > 0) interactions.AddRange(invocationInteractions.Fragments);
                            }

                            if (interactions.Count > 0)
                            {
                                var altSection = new AltSection();
                                if (alt.Sections.Count == 0) { altSection.GroupType = "if"; };
                                altSection.AddFragments(interactions);
                                altSection.Label = section.Condition;
                                alt.AddSection(altSection);
                            }
                        }

                        if (alt.Sections.Count > 0)
                        {
                            result.AddFragment(alt);
                        }

                        return result;
                    }
            }

            return new Interactions();
        }

        /// <summary>
        /// Returns <c>true</c> if the <paramref name="invocation"/> would result in a new message.
        /// </summary>
        public static bool IsMessageCreation(InvocationDescription invocation)
        {
            return invocation.Name == "PublishMessageAsync" || invocation.Name == "RaiseEvent";
        }

        /// <summary>
        /// Return the color an arrow should have in the diagram, based on the <paramref name="type"/> being a command or event.
        /// </summary>
        internal static string ArrowColor(TypeDescription type)
        {
            return $"[#{(IsCommand(type) ? "DodgerBlue" : "ForestGreen")}]";
        }

        /// <summary>
        /// Get the service name from the namespace.
        /// </summary>
        /// <remarks>
        /// Needs to deal with inconsistent pattern used over all the different application.</remarks>
        public static string Service(TypeDescription type)
        {
            return type.FullName.Split('.').SkipWhile(n => n.Equals("Pitstop", StringComparison.OrdinalIgnoreCase) || n.Equals("Application")).FirstOrDefault();
        }

        /// <summary>
        /// Return <c>true</c> if <paramref name="type"/> is an event.
        /// </summary>
        public static bool IsEvent(TypeDescription type)
        {
            return type.ImplementsType("Pitstop.Infrastructure.Messaging.Event");
        }

        /// <summary>
        /// Return <c>true</c> if <paramref name="type"/> is a command.
        /// </summary>
        public static bool IsCommand(TypeDescription type)
        {
            return type.ImplementsType("Pitstop.Infrastructure.Messaging.Command");
        }

        /// <summary>
        /// Return the method on <paramref name="type"/> that handles <paramref name="message"/>.
        /// </summary>
        public static MethodDescription HandlingMethod(TypeDescription type, TypeDescription message)
        {
            if (IsEvent(message))
            {
                return type.Methods.FirstOrDefault(m => string.Equals(m.Name, "HandleAsync") && m.Parameters.Any(p => p.Type.EndsWith("." + message.Name, StringComparison.Ordinal)));
            }

            if (IsCommand(message))
            {
                return type.Methods.FirstOrDefault(m =>
                    m.Parameters.Any(p => p.Type == message.FullName && p.Attributes.Any(a => string.Equals(a.Type, "Microsoft.AspNetCore.Mvc.FromBodyAttribute", StringComparison.Ordinal))) ||
                    m.Name == "HandleCommandAsync" && m.Parameters.Any(p => p.Type.EndsWith("." + message.Name, StringComparison.Ordinal)));
            }

            return null;
        }
    }
}
