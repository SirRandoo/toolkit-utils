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

namespace ToolkitUtils.Mod;

/// <summary>
///     The <c>SemanticVersion</c> class represents a version following the semantic versioning specification, which
///     consists of major, minor, and patch version components, with optional prerelease and build metadata identifiers.
/// </summary>
/// <remarks>
///     Instances of <c>SemanticVersion</c> encapsulate version information as defined by the semantic versioning
///     guidelines. The class provides comparison functionality based on the semantic precedence rules and offers parsing
///     capabilities for constructing instances from string representations. Comparisons consider major, minor, and patch
///     numbers, followed by prerelease identifiers when applicable. Build metadata is excluded from precedence
///     comparisons.
/// </remarks>
[PublicAPI]
public sealed record SemanticVersion(short Major, short Minor, short Patch, string Prerelease = "", string BuildMetadata = "")
{
    /// <summary>Represents the semantic version "0.0.0" with no prerelease or build metadata.</summary>
    /// <remarks>
    ///     This is a predefined instance of the <see cref="SemanticVersion" /> class signifying an initial or default
    ///     version in semantic versioning.
    /// </remarks>
    public static readonly SemanticVersion Zero = new(Major: 0, Minor: 0, Patch: 0);

    /// <summary>
    ///     Compares the current instance to another <see cref="SemanticVersion" /> instance and returns an integer
    ///     indicating their relative precedence.
    /// </summary>
    /// <param name="other">The <see cref="SemanticVersion" /> instance to compare with the current instance.</param>
    /// <returns>
    ///     A signed integer that indicates the relative precedence of the two instances:
    ///     <list type="bullet">
    ///         <item>Less than zero: The current instance is less than <paramref name="other" />.</item>
    ///         <item>Zero: The current instance is equal to <paramref name="other" />.</item>
    ///         <item>Greater than zero: The current instance is greater than <paramref name="other" />.</item>
    ///     </list>
    /// </returns>
    public int CompareTo(SemanticVersion? other)
    {
        if (other == null) return 1;

        int majorComparison = Major.CompareTo(other.Major);

        if (majorComparison != 0) return majorComparison;

        int minorComparison = Minor.CompareTo(other.Minor);

        if (minorComparison != 0) return minorComparison;

        int patchComparison = Patch.CompareTo(other.Patch);

        if (patchComparison != 0) return patchComparison;

        if (string.IsNullOrEmpty(Prerelease) && !string.IsNullOrEmpty(other.Prerelease)) return 1;
        if (!string.IsNullOrEmpty(Prerelease) && string.IsNullOrEmpty(other.Prerelease)) return -1;

        return string.Compare(Prerelease, other.Prerelease, StringComparison.Ordinal);
    }

    /// <summary>Parses a semantic version string and returns an instance of <see cref="SemanticVersion" />.</summary>
    /// <param name="version">The string representation of the semantic version to parse.</param>
    /// <returns>A <see cref="SemanticVersion" /> instance representing the parsed version.</returns>
    /// <exception cref="ArgumentException">
    ///     Thrown when the input version string is null, empty, or consists only of
    ///     whitespace.
    /// </exception>
    /// <exception cref="FormatException">
    ///     Thrown when the input string does not conform to the expected semantic version
    ///     format.
    /// </exception>
    public static unsafe SemanticVersion Parse(string version)
    {
        if (string.IsNullOrWhiteSpace(version)) throw new ArgumentException("Version string cannot be null or empty.");

        char* versionPtr = stackalloc char[version.Length];
        for (var i = 0; i < version.Length; i++) versionPtr[i] = version[i];

        short parsedMajor = 0;
        short parsedMinor = 0;
        short parsedPatch = 0;
        var parsedPrerelease = "";
        var parsedBuildMetadata = "";
        var parsePosition = ParsePosition.Major;

        var segmentStart = 0;

        for (var i = 0; i < version.Length; i++)
        {
            char c = version[i];

            switch (c)
            {
                case '.' when parsePosition == ParsePosition.Major:
                    parsedMajor = short.Parse(new string(versionPtr, segmentStart, i - segmentStart));
                    segmentStart = i + 1;
                    parsePosition = ParsePosition.Minor;

                    break;
                case '.' when parsePosition == ParsePosition.Minor:
                    parsedMinor = short.Parse(new string(versionPtr, segmentStart, i - segmentStart));
                    segmentStart = i + 1;
                    parsePosition = ParsePosition.Patch;

                    break;
                case '-' when parsePosition == ParsePosition.Patch:
                    parsedPatch = short.Parse(new string(versionPtr, segmentStart, i - segmentStart));
                    segmentStart = i + 1;
                    parsePosition = ParsePosition.Prerelease;

                    break;
                case '+' when parsePosition == ParsePosition.Prerelease:
                    parsedPrerelease = new string(versionPtr, segmentStart, i - segmentStart);
                    segmentStart = i + 1;
                    parsePosition = ParsePosition.BuildMetadata;

                    break;
            }
        }

        switch (parsePosition)
        {
            case ParsePosition.Patch:         parsedPatch = short.Parse(new string(versionPtr, segmentStart, version.Length - segmentStart)); break;
            case ParsePosition.BuildMetadata: parsedBuildMetadata = new string(versionPtr, segmentStart, version.Length - segmentStart); break;
            case ParsePosition.Prerelease:    parsedPrerelease = new string(versionPtr, segmentStart, version.Length - segmentStart); break;
        }

        return new SemanticVersion(parsedMajor, parsedMinor, parsedPatch, parsedPrerelease, parsedBuildMetadata);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var version = $"{Major}.{Minor}.{Patch}";
        if (!string.IsNullOrEmpty(Prerelease)) version += $"-{Prerelease}";
        if (!string.IsNullOrEmpty(BuildMetadata)) version += $"+{BuildMetadata}";

        return version;
    }

    private enum ParsePosition : byte
    {
        Major,
        Minor,
        Patch,
        Prerelease,
        BuildMetadata,
    }
}
