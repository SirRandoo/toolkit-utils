using System.Collections.Generic;
using System.Linq;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public class PurgeViewersDialog : Window
    {
        private readonly List<FloatMenuOption> constraintOptions;
        private readonly List<ConstraintBase> constraints;
        private Vector2 scrollPos = Vector2.zero;
        private bool showingAffected;

        public PurgeViewersDialog()
        {
            doCloseX = true;
            forcePause = true;
            onlyOneOfTypeAllowed = true;

            constraints = new List<ConstraintBase>();
            constraintOptions = new List<FloatMenuOption>
            {
                new FloatMenuOption(
                    "TKUtils.PurgeMenu.Coins".Localize().CapitalizeFirst(),
                    () => constraints.Add(new CoinConstraint())
                ),
                new FloatMenuOption(
                    "TKUtils.PurgeMenu.Karma".Localize().CapitalizeFirst(),
                    () => constraints.Add(new KarmaConstraint())
                ),
                new FloatMenuOption(
                    "TKUtils.PurgeMenu.Name".Localize().CapitalizeFirst(),
                    () => constraints.Add(new NameConstraint())
                )
            };
        }

        public override Vector2 InitialSize => new Vector2(900f, 740f);

        public override void DoWindowContents(Rect inRect)
        {
            GUI.BeginGroup(inRect);

            float midpoint = inRect.width / 2;
            TaggedString buttonText =
                (showingAffected ? "TKUtils.Buttons.Confirm" : "TKUtils.Buttons.ViewAffected")
                .Localize();
            float buttonWidth = Text.CalcSize(buttonText).x * 1.5f;
            var buttonRect = new Rect(midpoint - buttonWidth / 2f, inRect.height - 30f, buttonWidth, 28f);
            var headerArea = new Rect(inRect.x, inRect.y, inRect.width, 28f);
            var contentArea = new Rect(
                inRect.x,
                headerArea.height + 10f,
                inRect.width,
                inRect.height - headerArea.height - 40f
            );

            GUI.BeginGroup(buttonRect);
            if (Widgets.ButtonText(new Rect(0f, 0f, buttonRect.width, buttonRect.height), buttonText))
            {
                if (showingAffected)
                {
                    Purge();
                }
                else
                {
                    showingAffected = true;
                }
            }

            GUI.EndGroup();

            DrawHeader(headerArea);

            GUI.BeginGroup(contentArea);

            var constraintsArea = new Rect(0f, 0f, contentArea.width, contentArea.height);
            var contentView = new Rect(
                constraintsArea.x,
                constraintsArea.y,
                constraintsArea.width - 16f,
                (showingAffected ? GetAffectedViewers().Length : constraints.Count) * Text.LineHeight
            );

            Widgets.BeginScrollView(constraintsArea, ref scrollPos, contentView);
            if (showingAffected)
            {
                Viewer[] affected = GetAffectedViewers();
                TaggedString exemptText = "TKUtils.Buttons.Exempt".Localize();
                float exemptWidth = Text.CalcSize(exemptText).x * 1.5f;

                for (var i = 0; i < affected.Length; i++)
                {
                    Viewer viewer = affected[i];
                    var closeRect = new Rect(
                        contentView.width - exemptWidth,
                        i * Text.LineHeight,
                        exemptWidth,
                        Text.LineHeight
                    );

                    if (!closeRect.IsRegionVisible(constraintsArea, scrollPos))
                    {
                        continue;
                    }

                    Widgets.Label(
                        new Rect(0f, i * Text.LineHeight, inRect.width - exemptWidth - 20f, Text.LineHeight),
                        viewer.username
                    );

                    if (!Widgets.ButtonText(closeRect, exemptText))
                    {
                        continue;
                    }

                    var c = new NameConstraint();
                    c.SetUsername(viewer.username);
                    c.SetComparison(NameComparisonTypes.Not);

                    constraints.Add(c);
                }
            }
            else
            {
                TaggedString exemptText = "TKUtils.Buttons.Remove".Localize();
                float exemptWidth = Text.CalcSize(exemptText).x * 1.5f;
                ConstraintBase toRemove = null;

                for (var i = 0; i < constraints.Count; i++)
                {
                    ConstraintBase constraint = constraints[i];
                    var lineRect = new Rect(contentView.x, i * Text.LineHeight, contentView.width, Text.LineHeight);
                    var constraintRect = new Rect(
                        lineRect.x,
                        lineRect.y,
                        lineRect.width - exemptWidth - 20f,
                        lineRect.height
                    );
                    var exemptRect = new Rect(
                        contentView.width - exemptWidth,
                        i * Text.LineHeight,
                        exemptWidth,
                        Text.LineHeight
                    );

                    if (!lineRect.IsRegionVisible(constraintsArea, scrollPos))
                    {
                        continue;
                    }

                    constraint.Draw(constraintRect);

                    if (Widgets.ButtonText(exemptRect, exemptText))
                    {
                        toRemove = constraint;
                    }
                }

                if (toRemove != null)
                {
                    constraints.Remove(toRemove);
                }
            }

            Widgets.EndScrollView();
            GUI.EndGroup();

            GUI.EndGroup();
        }

        private void DrawHeader(Rect region)
        {
            GUI.BeginGroup(region);

            if (showingAffected)
            {
                TaggedString backText = "TKUtils.Buttons.Back".Localize();
                var backRegion = new Rect(region.x, region.y, Text.CalcSize(backText).x * 1.5f, region.height);
                var textRegion = new Rect(
                    region.x + backRegion.width + 10f,
                    region.y,
                    region.width - backRegion.width - 10f,
                    region.height
                );

                if (Widgets.ButtonText(backRegion, backText))
                {
                    showingAffected = false;
                }

                Viewer[] affected = GetAffectedViewers();
                Widgets.Label(
                    textRegion,
                    $"{affected:N0} {"TKUtils.Purge.Affected".Localize()}"
                );
            }
            else
            {
                float width = region.width * 0.25f;

                if (Widgets.ButtonText(
                    new Rect(region.x, region.y, width, region.height),
                    "TKUtils.Buttons.AddConstraint".Localize()
                ))
                {
                    Find.WindowStack.Add(new FloatMenu(constraintOptions));
                }

                if (Widgets.ButtonText(
                    new Rect(region.x + width + 10f, region.y, width, region.height),
                    "TKUtils.Buttons.ClearConstraints".Localize()
                ))
                {
                    constraints.Clear();
                }
            }

            GUI.EndGroup();
        }

        private Viewer[] GetAffectedViewers()
        {
            return Viewers.All
                .Where(v => constraints.All(c => c.ShouldPurge(v)))
                .ToArray();
        }

        private void Purge()
        {
            Viewer[] affected = GetAffectedViewers();
            int count = affected.Count(viewer => Viewers.All.Remove(viewer));

            TkLogger.Warn($"Purged {count:N0} viewers out of the requested {affected.Length:N0}!");
            showingAffected = false;
        }
    }
}
