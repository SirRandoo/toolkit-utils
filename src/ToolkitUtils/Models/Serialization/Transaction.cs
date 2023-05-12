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

namespace ToolkitUtils.Models.Serialization
{
    public class Transaction
    {
        public enum ActionType { Purchase, Transfer, Refund, Payment }

        public int Amount { get; set; }
        public ActionType Type { get; set; }
        public string Recipient { get; set; }
        public DateTime OccurredAt { get; set; }
    }
}
