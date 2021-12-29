// ToolkitUtils
// Copyright (C) 2021  SirRandoo
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TwitchToolkit;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public class PurgeViewersDialog : Window
    {
        private readonly List<FloatMenuOption> _constraintOptions;
        private readonly List<ConstraintBase> _constraints;
        private string _addConstraintText;
        private Vector2 _affectedScrollPos = Vector2.zero;
        private string _affectedText;
        private Viewer[] _affectedViewers;
        private int _affectedViewersCount;
        private string _backText;

        private float _bottomButtonWidth;
        private string _clearConstraintsText;

        private string _confirmText;
        private Vector2 _constraintsScrollPos = Vector2.zero;
        private float _exemptButtonWidth;
        private string _exemptText;
        private float _headerButtonWidth;
        private float _removeButtonWidth;
        private string _removeText;
        private string _showAffectedText;
        private bool _showingAffected;

        public PurgeViewersDialog()
        {
            doCloseX = true;
            forcePause = true;

            _constraints = new List<ConstraintBase>();

            _constraintOptions = new List<FloatMenuOption>
            {
                new FloatMenuOption("TKUtils.PurgeMenu.Coins".Localize().CapitalizeFirst(), () => _constraints.Add(new CoinConstraint())),
                new FloatMenuOption("TKUtils.PurgeMenu.Karma".Localize().CapitalizeFirst(), () => _constraints.Add(new KarmaConstraint())),
                new FloatMenuOption("TKUtils.PurgeMenu.Name".Localize().CapitalizeFirst(), () => _constraints.Add(new NameConstraint()))
            };
        }

        public override Vector2 InitialSize => new Vector2(900f, 740f);

        private static float LineHeight => Text.LineHeight * 1.5f;

        public override void PreOpen()
        {
            base.PreOpen();

            _confirmText = "TKUtils.Buttons.Confirm".Localize();
            _showAffectedText = "TKUtils.Buttons.ViewAffected".Localize();
            _exemptText = "TKUtils.Buttons.Exempt".Localize();
            _removeText = "TKUtils.Buttons.Remove".Localize();
            _backText = "TKUtils.Buttons.Back".Localize();
            _affectedText = "TKUtils.Purge.Affected".Localize();
            _addConstraintText = "TKUtils.Buttons.AddConstraint".Localize();
            _clearConstraintsText = "TKUtils.Buttons.ClearConstraints".Localize();


            _headerButtonWidth = Mathf.Max(Text.CalcSize(_addConstraintText).x, Text.CalcSize(_backText).x, Text.CalcSize(_clearConstraintsText).x) + 16f;
            _bottomButtonWidth = Mathf.Max(Text.CalcSize(_confirmText).x, Text.CalcSize(_showAffectedText).x) + 16f;
            _exemptButtonWidth = Text.CalcSize(_exemptText).x + 16f;
            _removeButtonWidth = Text.CalcSize(_removeText).x + 16f;
        }

        public override void DoWindowContents(Rect inRect)
        {
            GUI.BeginGroup(inRect);

            string buttonText = _showingAffected ? _confirmText : _showAffectedText;
            Rect buttonRect = new Rect(inRect.center.x - _bottomButtonWidth / 2f, inRect.height - 30f, _bottomButtonWidth, 28f).Rounded();
            var headerArea = new Rect(inRect.x, inRect.y, inRect.width, 28f);
            var contentArea = new Rect(inRect.x, headerArea.height + 10f, inRect.width, inRect.height - headerArea.height - 40f);

            GUI.BeginGroup(buttonRect);

            if (Widgets.ButtonText(new Rect(0f, 0f, buttonRect.width, buttonRect.height), buttonText))
            {
                if (_showingAffected)
                {
                    Purge();
                }
                else
                {
                    _showingAffected = true;
                    _affectedViewers = GetAffectedViewers();
                    _affectedViewersCount = _affectedViewers.Length;
                }
            }

            GUI.EndGroup();

            GUI.BeginGroup(headerArea);
            DrawHeader(new Rect(0f, 0f, headerArea.width, headerArea.height));
            GUI.EndGroup();

            GUI.BeginGroup(contentArea);
            var contentInnerRect = new Rect(0f, 0f, contentArea.width, contentArea.height);

            if (_showingAffected)
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
            int totalConstraints = _constraints.Count;
            Rect viewRect = new Rect(0f, 0f, inRect.width - 16f, LineHeight * totalConstraints).Rounded();

            Widgets.BeginScrollView(inRect, ref _constraintsScrollPos, viewRect);
            listing.Begin(viewRect);

            for (var i = 0; i < _constraints.Count; i++)
            {
                ConstraintBase constraint = _constraints[i];
                Rect lineRect = listing.GetRect(LineHeight);

                if (!lineRect.IsRegionVisible(viewRect, _constraintsScrollPos))
                {
                    continue;
                }

                if (i % 2 == 0)
                {
                    Widgets.DrawLightHighlight(lineRect);
                }

                var constraintRect = new Rect(lineRect.x, lineRect.y, lineRect.width - _removeButtonWidth - 20f, lineRect.height);
                var removeRect = new Rect(lineRect.width - _removeButtonWidth, lineRect.y, _removeButtonWidth, lineRect.height);

                constraint.Draw(constraintRect);

                if (Widgets.ButtonText(removeRect, _removeText))
                {
                    toRemove = constraint;
                }
            }

            if (toRemove != null)
            {
                _constraints.Remove(toRemove);
            }

            listing.End();
            Widgets.EndScrollView();
        }

        private void DrawAffectedViewers(Rect inRect)
        {
            if (_affectedViewers == null)
            {
                return;
            }

            var listing = new Listing_Standard();
            Rect viewRect = new Rect(0f, 0f, inRect.width - 16f, LineHeight * _affectedViewersCount).Rounded();

            Widgets.BeginScrollView(inRect, ref _affectedScrollPos, viewRect);
            listing.Begin(viewRect);

            for (var i = 0; i < _affectedViewersCount; i++)
            {
                Rect lineRect = listing.GetRect(LineHeight);

                if (!lineRect.IsRegionVisible(viewRect, _affectedScrollPos))
                {
                    continue;
                }

                if (i % 2 == 0)
                {
                    Widgets.DrawLightHighlight(lineRect);
                }


                Viewer viewer = _affectedViewers[i];
                var exemptRect = new Rect(lineRect.x + (lineRect.width - _exemptButtonWidth), lineRect.y, _exemptButtonWidth, lineRect.height);
                var labelRect = new Rect(lineRect.x, lineRect.y, lineRect.width - _exemptButtonWidth - 10f, lineRect.height);

                SettingsHelper.DrawLabel(labelRect, viewer.username);

                if (!Widgets.ButtonText(exemptRect, _exemptText))
                {
                    continue;
                }

                _constraints.Add(new NameConstraint { Username = viewer.username, NameStrategy = NameStrategies.Not });
                _affectedViewers = GetAffectedViewers();
                _affectedViewersCount = _affectedViewers.Length;
            }

            listing.End();
            Widgets.EndScrollView();
        }

        private void DrawHeader(Rect region)
        {
            var buttonRect = new Rect(0f, 0f, _headerButtonWidth, Text.LineHeight);

            if (_showingAffected)
            {
                var statusRect = new Rect(_headerButtonWidth + 10f, 0f, region.width - _headerButtonWidth - 10f, Text.LineHeight);

                if (Widgets.ButtonText(buttonRect, _backText))
                {
                    _showingAffected = false;
                }

                Widgets.Label(statusRect, $"{_affectedViewersCount:N0} {_affectedText}");
            }
            else
            {
                if (Widgets.ButtonText(buttonRect, _addConstraintText))
                {
                    Find.WindowStack.Add(new FloatMenu(_constraintOptions));
                }

                buttonRect = buttonRect.ShiftRight();

                if (Widgets.ButtonText(buttonRect, _clearConstraintsText))
                {
                    _constraints.Clear();
                }
            }
        }

        [CanBeNull]
        private Viewer[] GetAffectedViewers()
        {
            return _constraints.Count <= 0 ? null : Viewers.All.Where(v => _constraints.All(c => c.ShouldPurge(v))).ToArray();
        }

        private void Purge()
        {
            int count = _affectedViewers.Count(viewer => Viewers.All.Remove(viewer));

            LogHelper.Warn($"Purged {count:N0} viewers out of the requested {_affectedViewersCount:N0}!");
            ResetState();
        }

        private void ResetState()
        {
            _showingAffected = false;

            _constraints.Clear();
            _affectedScrollPos = Vector2.zero;
            _constraintsScrollPos = Vector2.zero;
        }
    }
}
