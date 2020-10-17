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
        private string addConstraintText;
        private Vector2 affectedScrollPos = Vector2.zero;
        private string affectedText;
        private Viewer[] affectedViewers;
        private int affectedViewersCount;
        private string backText;

        private float bottomButtonWidth;
        private string clearConstraintsText;

        private string confirmText;
        private Vector2 constraintsScrollPos = Vector2.zero;
        private float exemptButtonWidth;
        private string exemptText;
        private float headerButtonWidth;
        private float removeButtonWidth;
        private string removeText;
        private string showAffectedText;
        private bool showingAffected;

        public PurgeViewersDialog()
        {
            doCloseX = true;
            forcePause = true;

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

        private static float LineHeight => Text.LineHeight * 1.5f;

        public override void PreOpen()
        {
            base.PreOpen();

            confirmText = "TKUtils.Buttons.Confirm".Localize();
            showAffectedText = "TKUtils.Buttons.ViewAffected".Localize();
            exemptText = "TKUtils.Buttons.Exempt".Localize();
            removeText = "TKUtils.Buttons.Remove".Localize();
            backText = "TKUtils.Buttons.Back".Localize();
            affectedText = "TKUtils.Purge.Affected".Localize();
            addConstraintText = "TKUtils.Buttons.AddConstraint".Localize();
            clearConstraintsText = "TKUtils.Buttons.ClearConstraints".Localize();


            headerButtonWidth = Mathf.Max(
                                    Text.CalcSize(addConstraintText).x,
                                    Text.CalcSize(backText).x,
                                    Text.CalcSize(clearConstraintsText).x
                                )
                                + 16f;
            bottomButtonWidth = Mathf.Max(Text.CalcSize(confirmText).x, Text.CalcSize(showAffectedText).x) + 16f;
            exemptButtonWidth = Text.CalcSize(exemptText).x + 16f;
            removeButtonWidth = Text.CalcSize(removeText).x + 16f;
        }

        public override void DoWindowContents(Rect inRect)
        {
            GUI.BeginGroup(inRect);

            string buttonText = showingAffected ? confirmText : showAffectedText;
            Rect buttonRect = new Rect(
                inRect.center.x - bottomButtonWidth / 2f,
                inRect.height - 30f,
                bottomButtonWidth,
                28f
            ).Rounded();
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
                    affectedViewers = GetAffectedViewers();
                    affectedViewersCount = affectedViewers.Length;
                }
            }

            GUI.EndGroup();

            GUI.BeginGroup(headerArea);
            DrawHeader(new Rect(0f, 0f, headerArea.width, headerArea.height));
            GUI.EndGroup();

            GUI.BeginGroup(contentArea);
            var contentInnerRect = new Rect(0f, 0f, contentArea.width, contentArea.height);

            if (showingAffected)
            {
                DrawAffectedViewers(contentInnerRect);
            }
            else
            {
                DrawConstraints(contentInnerRect);
            }

            GUI.EndGroup();

            GUI.EndGroup();
        }

        private void DrawConstraints(Rect inRect)
        {
            ConstraintBase toRemove = null;
            var listing = new Listing_Standard();
            int totalConstraints = constraints.Count;
            Rect viewRect = new Rect(0f, 0f, inRect.width - 16f, LineHeight * totalConstraints).Rounded();

            listing.BeginScrollView(inRect, ref constraintsScrollPos, ref viewRect);
            for (var i = 0; i < constraints.Count; i++)
            {
                ConstraintBase constraint = constraints[i];
                Rect lineRect = listing.GetRect(LineHeight);

                if (!lineRect.IsRegionVisible(viewRect, constraintsScrollPos))
                {
                    continue;
                }

                if (i % 2 == 0)
                {
                    Widgets.DrawLightHighlight(lineRect);
                }

                var constraintRect = new Rect(
                    lineRect.x,
                    lineRect.y,
                    lineRect.width - removeButtonWidth - 20f,
                    lineRect.height
                );
                var removeRect = new Rect(
                    lineRect.width - removeButtonWidth,
                    lineRect.y,
                    removeButtonWidth,
                    lineRect.height
                );

                constraint.Draw(constraintRect);
                if (Widgets.ButtonText(removeRect, removeText))
                {
                    toRemove = constraint;
                }
            }

            if (toRemove != null)
            {
                constraints.Remove(toRemove);
            }

            listing.EndScrollView(ref viewRect);
        }

        private void DrawAffectedViewers(Rect inRect)
        {
            if (affectedViewers == null)
            {
                return;
            }

            var listing = new Listing_Standard();
            Rect viewRect = new Rect(0f, 0f, inRect.width - 16f, LineHeight * affectedViewersCount).Rounded();

            listing.BeginScrollView(inRect, ref affectedScrollPos, ref viewRect);

            for (var i = 0; i < affectedViewersCount; i++)
            {
                Rect lineRect = listing.GetRect(LineHeight);

                if (!lineRect.IsRegionVisible(viewRect, affectedScrollPos))
                {
                    continue;
                }

                if (i % 2 == 0)
                {
                    Widgets.DrawLightHighlight(lineRect);
                }


                Viewer viewer = affectedViewers[i];
                var exemptRect = new Rect(
                    lineRect.x + (lineRect.width - exemptButtonWidth),
                    lineRect.y,
                    exemptButtonWidth,
                    lineRect.height
                );
                var labelRect = new Rect(
                    lineRect.x,
                    lineRect.y,
                    lineRect.width - exemptButtonWidth - 10f,
                    lineRect.height
                );

                SettingsHelper.DrawLabel(labelRect, viewer.username);
                if (!Widgets.ButtonText(exemptRect, exemptText))
                {
                    continue;
                }

                constraints.Add(new NameConstraint {Username = viewer.username, NameStrategy = NameStrategies.Not});
                affectedViewers = GetAffectedViewers();
                affectedViewersCount = affectedViewers.Length;
            }

            listing.EndScrollView(ref viewRect);
        }

        private void DrawHeader(Rect region)
        {
            var buttonRect = new Rect(0f, 0f, headerButtonWidth, Text.LineHeight);

            if (showingAffected)
            {
                var statusRect = new Rect(
                    headerButtonWidth + 10f,
                    0f,
                    region.width - headerButtonWidth - 10f,
                    Text.LineHeight
                );

                if (Widgets.ButtonText(buttonRect, backText))
                {
                    showingAffected = false;
                }

                Widgets.Label(statusRect, $"{affectedViewersCount:N0} {affectedText}");
            }
            else
            {
                if (Widgets.ButtonText(buttonRect, addConstraintText))
                {
                    Find.WindowStack.Add(new FloatMenu(constraintOptions));
                }

                buttonRect = buttonRect.ShiftRight();
                if (Widgets.ButtonText(buttonRect, clearConstraintsText))
                {
                    constraints.Clear();
                }
            }
        }

        private Viewer[] GetAffectedViewers()
        {
            return constraints.Count <= 0
                ? null
                : Viewers.All.Where(v => constraints.All(c => c.ShouldPurge(v))).ToArray();
        }

        private void Purge()
        {
            int count = affectedViewers.Count(viewer => Viewers.All.Remove(viewer));

            TkLogger.Warn($"Purged {count:N0} viewers out of the requested {affectedViewersCount:N0}!");
            ResetState();
        }

        private void ResetState()
        {
            showingAffected = false;

            constraints.Clear();
            affectedScrollPos = Vector2.zero;
            constraintsScrollPos = Vector2.zero;
        }
    }
}
