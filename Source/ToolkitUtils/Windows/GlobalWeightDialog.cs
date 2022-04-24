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

using System;
using System.Collections.Generic;
using SirRandoo.CommonLib.Helpers;
using TwitchToolkit;
using TwitchToolkit.Storytellers.StorytellerPackWindows;
using TwitchToolkit.Votes;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    /// <summary>
    ///     A dialog for editing the event weights Twitch Toolkit's
    ///     storyteller uses for its polls.
    /// </summary>
    public class GlobalWeightDialog : Window_GlobalVoteWeights
    {
        private readonly Dictionary<string, string> _bufferCache = new Dictionary<string, string>();
        private readonly List<IncidentEntry> _entries = new List<IncidentEntry>();
        private float _height;

        private string _nullDictText;
        private int _totalWeights = 1;
        private Vector2 _weightScrollPos = Vector2.zero;

        public GlobalWeightDialog()
        {
            doCloseX = true;
            doCloseButton = false;
            optionalTitle = "TKUtils.Headers.GlobalWeights".TranslateSimple();
        }

        /// <inheritdoc cref="Window.PostOpen"/>
        public override void PostOpen()
        {
            _nullDictText = "TKUtils.GlobalWeights.Null".TranslateSimple();

            float width = windowRect.width - Margin * 2f - 16f;
            float entryWidth = Mathf.FloorToInt(width * 0.7f);

            foreach (VotingIncident incident in DefDatabase<VotingIncident>.AllDefs)
            {
                var entry = new IncidentEntry { Incident = incident, Height = Text.CalcHeight($"{incident.defName} - 1000.00 %", entryWidth) };
                _height += entry.Height;
                _entries.Add(entry);
            }
        }

        /// <inheritdoc cref="Window_GlobalVoteWeights.DoWindowContents"/>
        public override void DoWindowContents(Rect region)
        {
            GUI.BeginGroup(region);

            var weightsRect = new Rect(0f, 0f, region.width, region.height);

            GUI.BeginGroup(weightsRect);

            if (ToolkitSettings.VoteWeights == null)
            {
                UiHelper.Label(weightsRect.AtZero(), _nullDictText, ColorLibrary.Lavender, TextAnchor.MiddleCenter);
                ToolkitSettings.VoteWeights ??= new Dictionary<string, int>();
            }
            else
            {
                DrawIncidentWeights(weightsRect.AtZero());
            }

            GUI.EndGroup();

            GUI.EndGroup();
        }

        private void DrawIncidentWeights(Rect region)
        {
            var total = 0;
            var usedHeight = 0f;
            var viewPort = new Rect(0f, 0f, region.width - 16f, _height);

            GUI.BeginGroup(region);
            Widgets.BeginScrollView(region, ref _weightScrollPos, viewPort);

            foreach (IncidentEntry entry in _entries)
            {
                var relativeWeight = (float)Math.Round(entry.Incident.voteWeight / (double)_totalWeights * 100f, 2);

                var lineRect = new Rect(0f, usedHeight, viewPort.width, entry.Height);

                (Rect labelRect, Rect inputRect) = lineRect.Split(0.7f);
                UiHelper.Label(labelRect, $"{entry.Incident.defName} - {relativeWeight:P}");

                (Rect sliderRect, Rect fieldRect) = inputRect.Split(0.6f);
                var weight = (int)Widgets.HorizontalSlider(sliderRect, entry.Incident.voteWeight, 0f, 100f, true);

                if (weight != entry.Incident.voteWeight)
                {
                    entry.Incident.voteWeight = weight;
                    _bufferCache[entry.Incident.defName] = entry.Incident.voteWeight.ToString();
                }

                string buffer = null;

                if (!_bufferCache.TryGetValue(entry.Incident.defName, out string value))
                {
                    _bufferCache[entry.Incident.defName] = value = buffer = entry.Incident.voteWeight.ToString();
                }

                buffer ??= value;

                Widgets.TextFieldNumeric(fieldRect, ref entry.Incident.voteWeight, ref buffer);

                ToolkitSettings.VoteWeights[entry.Incident.defName] = entry.Incident.voteWeight;

                if (buffer != value)
                {
                    _bufferCache[entry.Incident.defName] = buffer;
                }

                usedHeight += entry.Height;
                total += entry.Incident.voteWeight;
            }

            _totalWeights = total;
            Widgets.EndScrollView();
            GUI.EndGroup();
        }

        private struct IncidentEntry
        {
            public float Height { get; set; }
            public VotingIncident Incident { get; set; }
        }
    }
}
