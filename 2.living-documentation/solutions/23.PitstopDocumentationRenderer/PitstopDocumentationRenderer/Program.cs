using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

            File.WriteAllText("pitstop.generated.md", stringBuilder.ToString());
        }
    }
}
