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

using System.Text;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Models;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    public class PersonaDef : ThingComp
    {
        private string lastUsername;
        private string linkedId;
        private UserData userData;

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref linkedId, "linkedUserId");
            Scribe_Values.Look(ref lastUsername, "lastUsername");
        }

        [NotNull]
        public override string GetDescriptionPart()
        {
            userData ??= UserRegistry.GetData(linkedId);
            lastUsername = userData?.DisplayName ?? userData?.Username;

            var builder = new StringBuilder();
            builder.Append(base.GetDescriptionPart());

            builder.Append("\n\n");
            builder.Append("TKUtils.Persona.Linked".Translate(lastUsername));

            return builder.ToString();
        }
    }
}
