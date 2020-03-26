using System;
using System.Collections.Generic;
using System.Linq;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public class DialogPurgeViewers : Window
    {
        private static readonly List<Tuple<string, Type>> Registry = new List<Tuple<string, Type>>
        {
            //new Tuple<string, Type>("Banned", typeof(BannedConstraint)),
            new Tuple<string, Type>("Coin", typeof(CoinConstraint)),
            new Tuple<string, Type>("Karma", typeof(KarmaConstraint)),
            //new Tuple<string, Type>("Mod", typeof(ModConstraint)),
            new Tuple<string, Type>("Name", typeof(NameConstraint))
            //new Tuple<string, Type>("Subscriber", typeof(SubscriberConstraint)),
            //new Tuple<string, Type>("Vip", typeof(VipConstraint))
        };

        private readonly List<ConstraintBase> constraints;
        private Vector2 scrollPos = Vector2.zero;
        private bool showingAffected;

        public DialogPurgeViewers()
        {
            doCloseX = true;
            forcePause = true;

            constraints = new List<ConstraintBase>();
        }

        public override Vector2 InitialSize => new Vector2(900f, 740f);

        public override void DoWindowContents(Rect inRect)
        {
            var midpoint = inRect.width / 2;
            var buttonText =
                (showingAffected ? "TKUtils.Windows.Purge.Buttons.Confirm" : "TKUtils.Windows.Purge.Buttons.Execute")
                .Translate();
            var buttonWidth = Text.CalcSize(buttonText).x * 1.5f;
            var buttonRect = new Rect(midpoint - buttonWidth / 2f, inRect.height - 30f, buttonWidth, 28f);
            var headerArea = new Rect(inRect.x, inRect.y, inRect.width, 28f);
            var contentArea = new Rect(
                inRect.x,
                headerArea.height + 10f,
                inRect.width,
                inRect.height - headerArea.height - 40f
            );
            var contentView = new Rect(contentArea.x, contentArea.y, contentArea.width * 0.97f, 96f * 36f);

            if (Widgets.ButtonText(buttonRect, buttonText))
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

            DrawHeader(headerArea);

            Widgets.BeginScrollView(contentArea, ref scrollPos, contentView);
            if (showingAffected)
            {
                var affected = GetAffectedViewers();
                var exemptText = "TKUtils.Windows.Purge.Buttons.ExemptViewer".Translate();
                var exemptWidth = Text.CalcSize(exemptText).x * 1.5f;

                for (var i = 0; i < affected.Length; i++)
                {
                    var viewer = affected[i];
                    var closeRect = new Rect(
                        contentView.width - exemptWidth,
                        contentView.y + i * 40f,
                        exemptWidth,
                        30f
                    );

                    Widgets.Label(
                        new Rect(contentView.x, contentView.y + i * 40f, inRect.width - exemptWidth - 20f, 30f),
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
                var exemptText = "TKUtils.Windows.Purge.Buttons.Remove".Translate();
                var exemptWidth = Text.CalcSize(exemptText).x * 1.5f;
                ConstraintBase toRemove = null;

                for (var i = 0; i < constraints.Count; i++)
                {
                    var constraint = constraints[i];

                    constraint.Draw(
                        new Rect(contentView.x, contentView.y + i * 40f, contentView.width - exemptWidth - 20f, 30f)
                    );

                    if (Widgets.ButtonText(
                        new Rect(contentView.width - exemptWidth, contentView.y + i * 40f, exemptWidth, 30f),
                        exemptText
                    ))
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
        }

        private void DrawHeader(Rect region)
        {
            if (showingAffected)
            {
                var backText = "TKUtils.Windows.Purge.Buttons.Back".Translate();
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

                var affected = GetAffectedViewers();
                Widgets.Label(
                    textRegion,
                    "TKUtils.Windows.Purge.Headers.Affected".Translate(affected.Length.ToString())
                );
            }
            else
            {
                var width = region.width * 0.25f;

                if (Widgets.ButtonText(
                    new Rect(region.x, region.y, width, region.height),
                    "TKUtils.Windows.Purge.Buttons.Add".Translate()
                ))
                {
                    var keys = Registry.Select(i => i.Item1).ToArray();
                    var options = keys.Select(
                            key => new FloatMenuOption(
                                key,
                                delegate
                                {
                                    var constraint = Registry.First(i => i.Item1.Equals(key));
                                    var t = constraint.Item2;

                                    constraints.Add((ConstraintBase) Activator.CreateInstance(t));
                                }
                            )
                        )
                        .ToList();

                    Find.WindowStack.Add(new FloatMenu(options));
                }

                if (Widgets.ButtonText(
                    new Rect(region.x + width + 10f, region.y, width, region.height),
                    "TKUtils.Windows.Purge.Buttons.ClearConstraints".Translate()
                ))
                {
                    constraints.Clear();
                }
            }
        }

        private Viewer[] GetAffectedViewers()
        {
            return Viewers.All
                .Where(v => constraints.All(c => c.ShouldPurge(v)))
                .ToArray();
        }

        private void Purge()
        {
            var affected = GetAffectedViewers();
            var count = affected.Count(viewer => Viewers.All.Remove(viewer));

            Logger.Warn($"Purged {count.ToString()} viewers out of the requested {affected.Length.ToString()}!");
            showingAffected = false;
        }
    }
}
