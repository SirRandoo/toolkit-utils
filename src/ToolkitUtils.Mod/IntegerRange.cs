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
namespace ToolkitUtils.Mod;

/// <summary>Represents a range of integers defined by a minimum and maximum value.</summary>
/// <remarks>This type is immutable and provides several pre-defined common ranges for convenience.</remarks>
public readonly record struct IntegerRange(int Minimum, int Maximum)
{
    /// <summary>Represents an integer range with both minimum and maximum values set to zero.</summary>
    /// <remarks>Commonly used as a placeholder or default value for scenarios involving ranges where no span is required.</remarks>
    public static readonly IntegerRange Zero = new(Minimum: 0, Maximum: 0);

    /// <summary>Represents a predefined integer range spanning from 0 to 1, inclusive.</summary>
    /// <remarks>
    ///     This range can be used where a small span of integers, starting from zero, is needed. It is part of a
    ///     collection of predefined ranges for convenience.
    /// </remarks>
    public static readonly IntegerRange ZeroToOne = new(Minimum: 0, Maximum: 1);

    /// <summary>Represents an integer range from 0 to 10, inclusive.</summary>
    /// <remarks>Provides a pre-defined range commonly used in scenarios requiring a small, bounded set of values.</remarks>
    public static readonly IntegerRange ZeroToTen = new(Minimum: 0, Maximum: 10);

    /// <summary>Represents an integer range starting from 0 and ending at 100.</summary>
    /// <remarks>
    ///     This range is a predefined instance of <see cref="IntegerRange" /> designed for use cases where values are
    ///     restricted between 0 and 100 inclusive.
    /// </remarks>
    public static readonly IntegerRange ZeroToHundred = new(Minimum: 0, Maximum: 100);

    /// <summary>Represents an integer range spanning from zero to one thousand, inclusively.</summary>
    /// <remarks>
    ///     This range is often utilized in scenarios where a predefined scope of values between 0 and 1000 is required.
    ///     It is a static, readonly instance of the <see cref="IntegerRange" /> type.
    /// </remarks>
    public static readonly IntegerRange ZeroToThousand = new(Minimum: 0, Maximum: 1000);

    /// <summary>Represents an integer range starting from 0 and ending at 10,000, inclusive.</summary>
    /// <remarks>This pre-defined range is often used for scenarios involving bounds or indexing within a defined limit.</remarks>
    public static readonly IntegerRange ZeroToTenThousand = new(Minimum: 0, Maximum: 10_000);

    /// <summary>Represents a predefined range of integers starting from 0 and ending at 100,000 (inclusive).</summary>
    /// <remarks>
    ///     This range is commonly used in scenarios where values are expected to fall within the bounds of 0 to 100,000.
    ///     It provides clarity and consistency in defining such value constraints.
    /// </remarks>
    public static readonly IntegerRange ZeroToHundredThousand = new(Minimum: 0, Maximum: 100_000);
}
