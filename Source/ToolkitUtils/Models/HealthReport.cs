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

namespace SirRandoo.ToolkitUtils.Models
{
    public class HealthReport
    {
        public enum ReportType { Info, Warning, Error, Debug }

        private DateTime occurredAt;

        public string Message { get; set; }
        public ReportType Type { get; set; }
        public string Reporter { get; set; }
        public float Height { get; internal set; }

        public DateTime OccurredAt
        {
            get => occurredAt;
            set
            {
                occurredAt = value;
                OccurredAtString = (DateTime.Now - occurredAt).TotalMinutes.ToString("N2");
            }
        }

        public string OccurredAtString { get; set; }
    }
}
