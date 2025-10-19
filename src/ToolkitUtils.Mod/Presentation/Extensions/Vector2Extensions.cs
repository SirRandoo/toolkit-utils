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
using JetBrains.Annotations;
using UnityEngine;

namespace ToolkitUtils.Mod.Presentation.Extensions;

[PublicAPI]
public static class Vector2Extensions
{
    public static ref Vector2 SetX(this ref Vector2 vector, float value)
    {
        vector.x = value;

        return ref vector;
    }

    public static ref Vector2 IncrementX(this ref Vector2 vector, float value)
    {
        vector.x += value;

        return ref vector;
    }

    public static ref Vector2 SetY(this ref Vector2 vector, float value)
    {
        vector.y = value;

        return ref vector;
    }

    public static ref Vector2 IncrementY(this ref Vector2 vector, float value)
    {
        vector.y += value;

        return ref vector;
    }

    public static ref Vector2 AtZero(this ref Vector2 vector)
    {
        vector.x = 0;
        vector.y = 0;

        return ref vector;
    }
}
