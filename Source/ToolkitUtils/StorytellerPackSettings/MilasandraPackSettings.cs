// MIT License
// 
// Copyright (c) 2022 SirRandoo
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using JetBrains.Annotations;
using TwitchToolkit;

namespace SirRandoo.ToolkitUtils.StorytellerPackSettings
{
    [UsedImplicitly]
    public class MilasandraPackSettings : PackSettingsBase
    {
        /// <inheritdoc/>
        public override bool Enabled
        {
            get => ToolkitSettings.MilasandraEnabled;
            set => ToolkitSettings.MilasandraEnabled = value;
        }

        /// <inheritdoc />
        [NotNull]
        public override string Tooltip =>
            "Threats on/off cycle. Milasandra uses an on/off cycle to bring votes in waves, similar to Cassandra. This pack is the closest to the base game's forced raid cycle. You will experience lag generated these votes.";
    }
}
