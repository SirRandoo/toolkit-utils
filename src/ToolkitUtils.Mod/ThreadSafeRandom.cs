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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using JetBrains.Annotations;
using ArgumentException = System.ArgumentException;
using ArgumentNullException = System.ArgumentNullException;
using Rand = Verse.Rand;
using Random = System.Random;

namespace ToolkitUtils.Mod;

/// <summary>Provides thread-safe random number generation and utility methods for randomization.</summary>
[PublicAPI]
public static class ThreadSafeRandom
{
    private static readonly Lock Lock = new();

    /// <summary>
    ///     Represents a thread-local, lazily initialized instance of <see cref="Random" /> with a thread-specific seed
    ///     for thread-safe random number generation.
    /// </summary>
    /// <remarks>
    ///     The <see cref="Instance" /> property ensures that each thread has its own unique instance of
    ///     <see cref="Random" />, reducing contention and allowing for efficient, concurrent random number generation.
    /// </remarks>
    [field: ThreadStatic]
    [field: AllowNull]
    [field: MaybeNull]
    public static Random Instance
    {
        get
        {
            if (field is not null) return field;
            int seed;

            lock (Lock)
            {
                seed = Rand.Int;
            }

            field = new Random(seed);

            return field;
        }
    }

    /// <summary>Returns a random number between 0 and 100.</summary>
    public static double Chance() => Instance.NextDouble() * 100;

    /// <summary>Returns whether the number generator rolled a number less than the chance specified.</summary>
    /// <param name="chance">A number indicating the chance an event could occur.</param>
    /// <remarks>
    ///     This method automatically converts chances greater than or equal to 1 into a double that fits within the range
    ///     returned by the random number generator, i.e. 75.25% would be converted to 0.7225.
    /// </remarks>
    public static bool Chance(double chance)
    {
        return chance switch
        {
            < 0           => false,
            > 100         => true,
            > 0 and < 100 => Instance.NextDouble() < chance / 100d,
            var _         => Instance.NextDouble() < chance,
        };
    }

    /// <summary>Returns a non-negative random integer.</summary>
    /// <returns>A 32-bit signed integer that is greater than or equal to 0 and less than <see cref="int.MaxValue" /></returns>
    public static int Next() => Instance.Next();

    /// <summary>Returns a non-negative integer that is less than the specified maximum.</summary>
    /// <param name="maximum">
    ///     The exclusive upper bound of the random number to be generated. `maximum` must be greater than or
    ///     equal to 0.
    /// </param>
    /// <returns>
    ///     A 32-bit signed integer that is greater than or equal to 0, and less than `maximum`; that is, the range of
    ///     return values ordinarily includes 0 but not `maximum`. However, if `maximum` equals 0, 0 is returned.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">`maximum` is less than 0.</exception>
    public static int Next(int maximum)
    {
        if (maximum < 0) throw new ArgumentOutOfRangeException(nameof(maximum));

        return maximum == 0 ? 0 : Instance.Next(maximum);
    }

    /// <summary>Returns a random integer within a specified range.</summary>
    /// <param name="minimum">The inclusive lower bound of the random number returned.</param>
    /// <param name="maximum">The exclusive upper bound of the random number returned.</param>
    /// <returns>
    ///     A 32-bit signed integer greater than or equal to `minimum` and less than `maximum`; that is, the range of
    ///     returned values includes `minimum` but not `maximum`. If `minimum` equals `maximum`, `minimum` is returned.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">`minimum` is greater than `maximum`.</exception>
    public static int Next(int minimum, int maximum)
    {
        if (minimum > maximum)
        {
            throw new ArgumentOutOfRangeException(nameof(minimum), minimum, message: "The minimum number is greater than the maximum.");
        }

        return minimum == maximum ? minimum : Instance.Next(minimum, maximum);
    }

    /// <summary>Selects a random element from a collection.</summary>
    /// <param name="collection">The collection of elements that indicate the possible choices that can be chosen.</param>
    /// <typeparam name="T">The type of the elements contain within the collection.</typeparam>
    /// <returns>A randomly chosen element from the collection.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the collection passed was null.</exception>
    /// <exception cref="ArgumentException">Thrown when the collection is empty.</exception>
    public static T Choice<T>(IReadOnlyList<T> collection)
    {
        if (collection is null) throw new ArgumentNullException(nameof(collection));

        switch (collection.Count)
        {
            case 0: throw new ArgumentException(message: "Collection is empty", nameof(collection));
            case 1: return collection[0];
            default:
                int total = collection.Count;
                int random = Instance.Next(total);

                return collection[random];
        }
    }

    /// <summary>Selects a random element from a collection.</summary>
    /// <param name="collection">The collection of elements that indicate the possible choices that can be chosen.</param>
    /// <typeparam name="T">The type of the elements contain within the collection.</typeparam>
    /// <returns>A randomly chosen element from the collection.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the collection passed was null.</exception>
    /// <exception cref="ArgumentException">Thrown when the collection is empty.</exception>
    public static T Choice<T>(T[] collection)
    {
        if (collection is null) throw new ArgumentNullException(nameof(collection));

        switch (collection.Length)
        {
            case 0: throw new ArgumentException(message: "Collection is empty", nameof(collection));
            case 1: return collection[0];
            default:
                int total = collection.Length;
                int random = Instance.Next(total);

                return collection[random];
        }
    }

    /// <summary>Shuffles a collection in-place.</summary>
    /// <param name="collection">The collection being shuffled.</param>
    /// <typeparam name="T">The type of the objects contained within the collection.</typeparam>
    /// <exception cref="ArgumentNullException">Thrown when the collection passed is null.</exception>
    /// <remarks>This method will modify the contents of the collection passed, not return a shuffled copy of the collection.</remarks>
    public static void Shuffle<T>(this IList<T> collection)
    {
        if (collection is null) throw new ArgumentNullException(nameof(collection));

        for (int i = collection.Count - 1; i >= 0; i--)
        {
            int newPosition = Instance.Next(collection.Count);
            collection[i] = collection[newPosition];
        }
    }

    /// <summary>Shuffles a collection in-place.</summary>
    /// <param name="collection">The collection being shuffled.</param>
    /// <typeparam name="T">The type of the objects contained within the collection.</typeparam>
    /// <exception cref="ArgumentNullException">Thrown when the collection passed is null.</exception>
    /// <remarks>This method will modify the contents of the collection passed, not return a shuffled copy of the collection.</remarks>
    public static void Shuffle<T>(this T[] collection)
    {
        if (collection is null) throw new ArgumentNullException(nameof(collection));

        for (int i = collection.Length - 1; i >= 0; i--)
        {
            int newPosition = Instance.Next(collection.Length);
            collection[i] = collection[newPosition];
        }
    }
}
