using LivingDocumentation.Uml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PitstopDocumentationRenderer
{
    internal class UmlFragmentRenderer
    {
        private static readonly string[] ExternalTargets = new[] { "[", "]" };

        /// <summary>
        /// Render a tree of interactions.
        /// </summary>
        public static void RenderTree(StringBuilder stringBuilder, IEnumerable<InteractionFragment> branch, Interactions tree)
        {
            RenderTree(stringBuilder, branch, tree, null);
        }

        /// <summary>
        /// Render a tree of interactions, tracking activations.
        /// </summary>
        private static void RenderTree(StringBuilder stringBuilder, IEnumerable<InteractionFragment> branch, Interactions tree, List<string> activations)
        {
            if (activations == null) activations = new List<string>();

            var branchActivations = new List<string>(activations);

            foreach (var leaf in branch)
            {
                var leafActivations = new List<string>(activations);

                switch (leaf)
                {
                    case Interactions statementList:
                        RenderTree(stringBuilder, statementList.Fragments, tree, leafActivations);
                        break;

                    case Arrow arrow:
                        RenderArrow(stringBuilder, arrow, branch.ToList(), tree, leafActivations);
                        break;

                    case Alt alt:
                        leafActivations = new List<string>(branchActivations);
                        RenderGroup(stringBuilder, alt, tree, leafActivations);
                        break;
                }

                branchActivations.AddRange(leafActivations.Except(branchActivations));
            }

            foreach (var branchActivation in branchActivations.Except(activations))
            {
                stringBuilder.Deactivate(branchActivation);
            }
        }

        /// <summary>
        /// Renders a group.
        /// </summary>
        /// <remarks>
        /// A group can be if/alt/else/case/etc.
        /// </remarks>
        private static void RenderGroup(StringBuilder stringBuilder, Alt alt, Interactions tree, List<string> activations)
        {
            var switchBuilder = new StringBuilder();

            foreach (var section in alt.Sections)
            {
                var sectionBuilder = new StringBuilder();

                RenderTree(sectionBuilder, section.Fragments, tree, new List<string>(activations));

                if (sectionBuilder.Length > 0)
                {
                    var first = switchBuilder.Length == 0;
                    if (first)
                    {
                        switchBuilder.Space(5);

                        if (string.IsNullOrWhiteSpace(section.GroupType))
                        {
                            switchBuilder.AltStart();
                        }
                        else
                        {
                            switchBuilder.Append($"group {(section.GroupType == "case" || section.GroupType == "switch" ? "switch" : section.GroupType)}");

                            if (section.GroupType == "case")
                            {
                                switchBuilder.AppendLine();
                                switchBuilder.ElseStart();
                            }
                        }
                    }
                    else
                    {
                        switchBuilder.ElseStart();
                    }

                    switchBuilder.AppendLine(string.IsNullOrWhiteSpace(section.GroupType) || section.GroupType == "case" ? $" {section.Label}" : $" [{section.Label}]");
                    switchBuilder.Append(sectionBuilder);
                    switchBuilder.Space(5);
                }
            }

            if (switchBuilder.Length > 0)
            {
                stringBuilder.Append(switchBuilder);
                stringBuilder.GroupEnd();
            }
        }

        /// <summary>
        /// Renders an arrow between two participants.
        /// </summary>
        /// <remarks>
        /// Takes scope (if/alt/group/etc.) into account for correctly close activation lines.
        /// </remarks>
        private static void RenderArrow(StringBuilder stringBuilder, Arrow arrow, IReadOnlyList<InteractionFragment> scope, Interactions tree, List<string> activations)
        {
            var target = (arrow.Source != "W" && arrow.Target == "A") ? "Q" : arrow.Target;

            stringBuilder.Arrow(arrow.Source, $"-{arrow.Color}>", target, label: arrow.Name);

            if (!activations.Contains(arrow.Source))
            {
                // Scope was activated in current scope

                if (arrow.Target != "]" && arrow.Source != "A" && arrow.Source != "W" && !arrow.Source.StartsWith("x ", StringComparison.Ordinal) && scope.Descendants<Arrow>().Last(a => a.Source == arrow.Source) == arrow && arrow.Source != arrow.Target)
                {
                    // This is the last arrow from this source
                    stringBuilder.Deactivate(arrow.Source);

                    activations.Remove(arrow.Source);
                }
            }

            if (!activations.Contains(arrow.Target))
            {
                // Scope was not activated by the parent scope

                if ((arrow.Target != "A" || arrow.Source == "W") && arrow.Target != "Q" && arrow.Target != "W" && !arrow.Target.StartsWith("x ", StringComparison.Ordinal) && arrow.Source != arrow.Target && scope.OfType<Arrow>().First(a => a.Target == arrow.Target) == arrow)
                {
                    var previousArrows = arrow.Ancestors().SelectMany(a => a.StatementsBeforeSelf()).OfType<Arrow>().ToList();
                    if (!previousArrows.Any(a => a.Target == arrow.Target) && !ExternalTargets.Contains(arrow.Target))
                    {
                        // There was no earlier activation in the current scope
                        stringBuilder.Activate(arrow.Target);

                        activations.Add(arrow.Target);
                    }
                }
            }
        }
    }
}
