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
using SirRandoo.ToolkitUtils.Helpers;
using TwitchToolkit;
using TwitchToolkit.Storytellers.StorytellerPackWindows;
using TwitchToolkit.Votes;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public class GlobalWeightDialog : Window_GlobalVoteWeights
    {
        private readonly List<VotingIncident> incidents;
        private int totalWeights = 1;
        private Vector2 weightScrollPos = Vector2.zero;

        public GlobalWeightDialog()
        {
            incidents = DefDatabase<VotingIncident>.AllDefsListForReading;
        }

        public override void DoWindowContents(Rect region)
        {
            GUI.BeginGroup(region);

            var noticeRect = new Rect(0f, 0f, region.width, Text.SmallFontHeight);
            Rect weightsRect = new Rect(
                0f,
                noticeRect.height + 5f,
                region.width,
                region.height - noticeRect.height - 5f
            ).ContractedBy(4f);
            SettingsHelper.DrawLabel(noticeRect, "Change the weights given to votes. Setting to 0% disables it.");

            GUI.BeginGroup(weightsRect);
            if (ToolkitSettings.VoteWeights == null)
            {
                SettingsHelper.DrawColoredLabel(
                    weightsRect.AtZero(),
                    "Vote weight dictionary is null.",
                    ColorLibrary.Lavender
                );
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
            var viewPort = new Rect(0f, 0f, region.width - 16f, Text.SmallFontHeight * incidents.Count);
            var total = 0;

            Widgets.BeginScrollView(region, ref weightScrollPos, viewPort);
            for (var index = 0; index < incidents.Count; index++)
            {
                VotingIncident incident = incidents[index];
                var relativeWeight = (float)Math.Round(incident.voteWeight / (double)totalWeights * 100f, 2);

                var lineRect = new Rect(0f, index * Text.SmallFontHeight, viewPort.width, Text.SmallFontHeight);
                (Rect labelRect, Rect inputRect) = lineRect.ToForm(0.7f);
                SettingsHelper.DrawFittedLabel(labelRect, $"{incident.defName} - {relativeWeight:P}");

                (Rect sliderRect, Rect fieldRect) = inputRect.ToForm(0.6f);
                incident.voteWeight = (int)Widgets.HorizontalSlider(sliderRect, incident.voteWeight, 0f, 100f, true);

                var buffer = incident.voteWeight.ToString();
                Widgets.TextFieldNumeric(fieldRect, ref incident.voteWeight, ref buffer);

                ToolkitSettings.VoteWeights[incident.defName] = incident.voteWeight;
                total += incident.voteWeight;
            }

            totalWeights = total;
            Widgets.EndScrollView();
        }
    }
}
