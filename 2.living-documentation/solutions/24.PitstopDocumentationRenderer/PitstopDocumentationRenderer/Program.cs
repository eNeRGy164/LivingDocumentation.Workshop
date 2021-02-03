using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LivingDocumentation;
using LivingDocumentation.Uml;
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

            Types.PopulateInheritedBaseTypes();
            Types.PopulateInheritedMembers();

            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("# Pitstop Generated Documentation");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("## Service Architecture");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("![Pitstop solution architecture](https://github.com/EdwinVW/pitstop/wiki/img/solution-architecture.png)");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine("## Commands");
            stringBuilder.AppendLine();

            foreach (var group in Types.Where(t => t.ImplementsType("Pitstop.Infrastructure.Messaging.Command"))
                .GroupBy(t => t.Name)
                .OrderBy(g => g.Key))
            {
                stringBuilder.AppendLine($"### {group.Key.ToSentenceCase()}");
                stringBuilder.AppendLine();

                stringBuilder.AppendLine("#### Services");
                stringBuilder.AppendLine();
                foreach (var command in group.OrderBy(c => c.Namespace))
                {
                    stringBuilder.AppendLine($"- {command.Namespace.Split('.').Reverse().Skip(1).First().ToSentenceCase()}");
                }
                stringBuilder.AppendLine();

                if (group.SelectMany(t => t.Fields).Any())
                {
                    stringBuilder.AppendLine("#### Fields");
                    stringBuilder.AppendLine();

                    stringBuilder.AppendLine("|Property|Type|Description|");
                    stringBuilder.AppendLine("|-|-|-|");

                    foreach (var field in group.SelectMany(t => t.Fields)
                        .GroupBy(f => (f.Type, f.Name))
                        .Select(g => g.First())
                        .OrderBy(f => f.Name))
                    {
                        stringBuilder.AppendLine($"|{field.Name}|{field.Type.ForDiagram()}|{field.DocumentationComments?.Summary}|");
                    }

                    stringBuilder.AppendLine();
                }
            }

            stringBuilder.AppendLine("## Events");
            stringBuilder.AppendLine();

            foreach (var group in Types.Where(t => t.ImplementsType("Pitstop.Infrastructure.Messaging.Event"))
                .GroupBy(t => t.Name)
                .OrderBy(t => t.Key))
            {
                stringBuilder.AppendLine($"### {group.Key.ToSentenceCase()}");
                stringBuilder.AppendLine();

                stringBuilder.AppendLine("#### Services");
                stringBuilder.AppendLine();
                foreach (var @event in group.OrderBy(e => e.Namespace))
                {
                    stringBuilder.AppendLine($"- {@event.Namespace.Split('.').Reverse().Skip(1).First().ToSentenceCase()}");
                }

                stringBuilder.AppendLine();

                if (group.SelectMany(t => t.Fields).Any())
                {
                    stringBuilder.AppendLine("#### Fields");
                    stringBuilder.AppendLine();

                    stringBuilder.AppendLine("|Property|Type|Description|");
                    stringBuilder.AppendLine("|-|-|-|");

                    foreach (var field in group.SelectMany(t => t.Fields)
                        .GroupBy(f => (f.Type, f.Name))
                        .Select(g => g.First())
                        .OrderBy(f => f.Name))
                    {
                        stringBuilder.AppendLine($"|{field.Name}|{field.Type.ForDiagram()}|{field.DocumentationComments?.Summary}|");
                    }

                    stringBuilder.AppendLine();
                }
            }

            stringBuilder.AppendLine("## Aggregates");
            stringBuilder.AppendLine();

            foreach (var aggregateRoot in Types.Where(t => t.ImplementsTypeStartsWith("Pitstop.WorkshopManagementAPI.Domain.Core.AggregateRoot<")))
            {

                stringBuilder.AppendLine($"### {aggregateRoot.Name.ToSentenceCase()}");
                stringBuilder.AppendLine();

                var aggregateIDType = Types.First(aggregateRoot.BaseTypes.First(bt => bt.StartsWith("Pitstop.WorkshopManagementAPI.Domain.Core.AggregateRoot<")).GenericTypes().First());

                stringBuilder.AppendLine("```plantuml");
                stringBuilder.UmlDiagramStart();

                RenderClass(stringBuilder, aggregateIDType);

                stringBuilder.AppendLine($"{aggregateIDType.Name} -- {aggregateRoot.Name}");

                RenderClass(stringBuilder, aggregateRoot);

                stringBuilder.UmlDiagramEnd();
                stringBuilder.AppendLine("```");
                stringBuilder.AppendLine();
            }

            File.WriteAllText("pitstop.generated.md", stringBuilder.ToString());
        }

        private static void RenderClass(StringBuilder stringBuilder, TypeDescription type)
        {
            #region awesomesauce
            var stereotype = "entity";
            CustomSpot? customSpot = null;

            if (type.IsClass() && type.Namespace.Split('.').Contains("ValueObjects"))
            {
                stereotype = "value object";
                customSpot = new CustomSpot('O', "LightBlue");
            }
            else if (type.IsClass() && type.ImplementsTypeStartsWith("Pitstop.WorkshopManagementAPI.Domain.Core.AggregateRoot<"))
            {
                stereotype = "root";
                customSpot = new CustomSpot('R', "LightGreen");
            }
            #endregion

            stringBuilder.ClassStart(type.Name, displayName: type.Name.ToSentenceCase(), stereotype: stereotype, customSpot: customSpot);

            foreach (var property in type.Properties.Where(p => !p.IsPrivate()).OrderBy(p => p.Name))
            {
                stringBuilder.ClassMember($"{property.Name.ToSentenceCase()}: {property.Type.ForDiagram()}", property.IsStatic(), visibility: property.ToUmlVisibility());
            }

            foreach (var method in type.Methods.Where(m => !m.IsPrivate() && !m.IsOverride()))
            {
                var parameterList = method.Parameters.Select(p => p.Name.ToSentenceCase()).Aggregate("", (s, a) => a + ", " + s, s => s.Trim(',', ' '));
                stringBuilder.ClassMember($"{method.Name} ({parameterList})", method.IsStatic(), visibility: method.ToUmlVisibility());
            }

            stringBuilder.ClassEnd();


            foreach (var propertyDescription in type.Properties)
            {
                var property = Types.FirstOrDefault(t => string.Equals(t.FullName, propertyDescription.Type) || (propertyDescription.Type.IsEnumerable() && string.Equals(t.FullName, propertyDescription.Type.GenericTypes().First())));
                if (property != null)
                {
                    RenderClass(stringBuilder, property);

                    // Relation
                    stringBuilder.Append($"{type.Name} -- {property.Name}");
                    if (propertyDescription.Type.IsEnumerable()) stringBuilder.Append(" : 1..*");
                    stringBuilder.AppendLine();
                }
            }
        }
    }
}
