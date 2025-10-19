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
// with ToolkitUtils.Api. If not, see <https://www.gnu.org/licenses/>.
using System;
using JetBrains.Annotations;

namespace ToolkitUtils.Api;

/// <summary>A small helper class for performing arithmetic.</summary>
[PublicAPI]
public static class Calculator
{
    /// <summary>Multiplies two integers.</summary>
    /// <param name="left">The left number.</param>
    /// <param name="right">The right number.</param>
    /// <returns>The result of the multiplication, or the maximum/minimum integer depending on the overflow that took place.</returns>
    public static int MultiplyUnchecked(int left, int right)
    {
        var leftLong = (long)left;
        var rightLong = (long)right;
        long result = leftLong * rightLong;

        if (result is > int.MaxValue or < int.MinValue) return (int)Math.Min(Math.Max(result, int.MinValue), int.MaxValue);

        return (int)result;
    }

    /// <summary>Multiplies two integers.</summary>
    /// <param name="left">The left number.</param>
    /// <param name="right">The right number.</param>
    /// <returns>The result of the multiplication, or the maximum/minimum integer depending on the overflow that took place.</returns>
    /// <remarks>This method uses a checked region to multiply the two numbers.</remarks>
    public static int MultiplyChecked(int left, int right)
    {
        try { return checked(left * right); }
        catch (OverflowException)
        {
            // An alternative way of clamping the result into
            // the bounds of int.
            if (left < 0 || right < 0) return int.MinValue;

            return int.MaxValue;
        }
    }
}
