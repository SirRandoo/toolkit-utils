﻿// ToolkitUtils
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
using System.Runtime.Serialization;

namespace SirRandoo.ToolkitUtils.Models
{
    public class TokenValidateResponse
    {
        [DataMember(Name = "client_id")] public string ClientId { get; set; }
        [DataMember(Name = "login")] public string Login { get; set; }
        [DataMember(Name = "scopes")] public List<string> Scopes { get; set; }
        [DataMember(Name = "user_id")] public string UserId { get; set; }
        [DataMember(Name = "expires_in")] public int ExpiresIn { get; set; }
    }
}