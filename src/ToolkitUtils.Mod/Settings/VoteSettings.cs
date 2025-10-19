// Copyright (C) 2025 sirrandoo
// 
// This file is part of ToolkitUtils.
// 
// ToolkitUtils is free software: you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License version 3 as published by the
// Free Software Foundation.
// 
// ToolkitUtils is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
// for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along
// with ToolkitUtils.Mod. If not, see <https://www.gnu.org/licenses/>.
using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace ToolkitUtils.Mod.Domain.Settings;

[PublicAPI]
public sealed record VoteSettings(
    [property: JsonProperty("vote_time")] TimeSpan VoteTime,
    [property: JsonProperty("maximum_options")] int MaximumOptions = 4,
    [property: JsonProperty("show_voting_window")] bool ShowVotingWindow = true,
    [property: JsonProperty("send_votes_as_chat_messages")] bool SendVotesAsChatMessages = true,
    [property: JsonProperty("version")] byte Version = 1
);
