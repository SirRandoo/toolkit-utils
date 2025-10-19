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
using JetBrains.Annotations;

namespace ToolkitUtils.Api;

/// <summary>Represents a Unicode character with a specified code and codepoint.</summary>
/// <remarks>
///     The <see cref="UnicodeCharacter" /> struct is used to define Unicode characters including emojis with
///     corresponding code identifiers and Unicode codepoints. It provides a range of pre-defined instances representing
///     various symbols and emojis.
/// </remarks>
/// <param name="Code">The unique code identifier for the Unicode character.</param>
/// <param name="Codepoint">The Unicode representation of the character.</param>
[PublicAPI]
public record struct UnicodeCharacter(string Code, string Codepoint)
{
    /// <summary>Represents the infinity emoji.</summary>
    /// <remarks>The Infinity emoji is used to signify the concept of infinity and is represented by the Unicode character '∞'.</remarks>
    public static readonly UnicodeCharacter Infinity = new(Code: "infinity", Codepoint: """∞""");

    /// <summary>Represents the coin bag emoji.</summary>
    /// <remarks>
    ///     The CoinBag emoji is often used to indicate wealth, money, or banking and is represented by the Unicode
    ///     character '💰'.
    /// </remarks>
    public static readonly UnicodeCharacter CoinBag = new(Code: "coin_bag", Codepoint: """💰""");

    /// <summary>Represents the balance scale emoji.</summary>
    /// <remarks>
    ///     The Balance Scale emoji is often used to convey concepts related to justice, balance, and fairness. It is
    ///     represented by the Unicode character '⚖'.
    /// </remarks>
    public static readonly UnicodeCharacter BalanceScale = new(Code: "balance_scale", Codepoint: """⚖""");

    /// <summary>Represents the chart increasing emoji.</summary>
    /// <remarks>
    ///     The Chart Increasing emoji is depicted as a graph with an upward trend and is represented by the Unicode
    ///     character '📈'. It is commonly used to represent growth, improvement, or a rise in statistics.
    /// </remarks>
    public static readonly UnicodeCharacter ChartIncreasing = new(Code: "chart_increasing", Codepoint: """📈""");

    /// <summary>Represents the chart decreasing emoji.</summary>
    /// <remarks>
    ///     The Chart Decreasing emoji is used to depict a declining trend or decreasing values and is represented by the
    ///     Unicode character '📉'.
    /// </remarks>
    public static readonly UnicodeCharacter ChartDecreasing = new(Code: "chart_descending", Codepoint: """📉""");

    /// <summary>Represents the thermometer emoji.</summary>
    /// <remarks>
    ///     The Thermometer emoji is used to signify medical or temperature-related contexts and is represented by the
    ///     Unicode character '🌡'.
    /// </remarks>
    public static readonly UnicodeCharacter Thermometer = new(Code: "thermometer", Codepoint: """🌡""");

    /// <summary>Represents the blood droplet emoji.</summary>
    /// <remarks>
    ///     The Blood emoji is used to symbolize blood-related themes, such as health, medical scenarios, or to indicate a
    ///     drop of liquid. It is represented by the Unicode character '🩸'.
    /// </remarks>
    public static readonly UnicodeCharacter Blood = new(Code: "blood", Codepoint: """🩸""");

    /// <summary>Represents the bandage emoji.</summary>
    /// <remarks>
    ///     The Bandage emoji is used to symbolize healing, medical care, or recovery, and it is represented by the
    ///     Unicode character '🩹'.
    /// </remarks>
    public static readonly UnicodeCharacter Bandage = new(Code: "bandage", Codepoint: """🩹""");

    /// <summary>Represents the dagger emoji.</summary>
    /// <remarks>The Dagger emoji is used to signify a dagger and is represented by the Unicode character '🗡'.</remarks>
    public static readonly UnicodeCharacter Dagger = new(Code: "dagger", Codepoint: """🗡""");

    /// <summary>Represents the pan emoji.</summary>
    /// <remarks>
    ///     The Pan emoji is used to depict a frying pan, often utilized in contexts related to cooking or kitchens, and
    ///     is represented by the Unicode character '🍳'.
    /// </remarks>
    public static readonly UnicodeCharacter Pan = new(Code: "pan", Codepoint: """🍳""");

    /// <summary>Represents the flame emoji.</summary>
    /// <remarks>The Flame emoji is typically used to depict fire and is symbolized by the Unicode character '🔥'.</remarks>
    public static readonly UnicodeCharacter Flame = new(Code: "flame", Codepoint: """🔥""");

    /// <summary>Represents the swirling star emoji.</summary>
    /// <remarks>
    ///     The Swirling Star emoji is often used to convey dizziness, creativity, or a magical effect and is represented
    ///     by the Unicode character '💫'.
    /// </remarks>
    public static readonly UnicodeCharacter SwirlingStar = new(Code: "swirling_star", Codepoint: """💫""");

    /// <summary>Represents the ghost emoji.</summary>
    /// <remarks>
    ///     The Ghost emoji is often used to convey a sense of spookiness, playfulness, or to represent the supernatural.
    ///     It is represented by the Unicode character '👻'.
    /// </remarks>
    public static readonly UnicodeCharacter Ghost = new(Code: "ghost", Codepoint: """👻""");

    /// <summary>Represents the lightning emoji.</summary>
    /// <remarks>
    ///     The Lightning emoji is used to convey excitement, electric energy, or to signify a sudden event. It is
    ///     represented by the Unicode character '⚡'.
    /// </remarks>
    public static readonly UnicodeCharacter Lighting = new(Code: "lightning", Codepoint: """⚡""");

    /// <summary>Represents the face with symbols on mouth emoji.</summary>
    /// <remarks>
    ///     The Face With Symbols On Mouth emoji is used to convey anger, frustration, or profanity and is represented by
    ///     the Unicode character '🤬'.
    /// </remarks>
    public static readonly UnicodeCharacter FaceWithSymbolsOnMouth = new(Code: "face_with_symbols_on_mouth", Codepoint: """🤬""");

    /// <summary>Represents the angry face emoji.</summary>
    /// <remarks>
    ///     The Angry Face emoji is used to express anger, frustration, or annoyance and is represented by the Unicode
    ///     character '😠'.
    /// </remarks>
    public static readonly UnicodeCharacter AngryFace = new(Code: "angry_face", Codepoint: """😠""");

    /// <summary>Represents the crystal ball emoji.</summary>
    /// <remarks>
    ///     The CrystalBall emoji is commonly used to symbolize predictions or fortune-telling and is represented by the
    ///     Unicode character '🔮'.
    /// </remarks>
    public static readonly UnicodeCharacter CrystalBall = new(Code: "crystal_ball", Codepoint: """🔮""");

    /// <summary>Represents the persevering face emoji.</summary>
    /// <remarks>
    ///     The Persevering Face emoji is used to convey feelings of determination or the effort of trying to overcome a
    ///     difficult situation, represented by the Unicode character '😣'.
    /// </remarks>
    public static readonly UnicodeCharacter PerseveringFace = new(Code: "persevering_face", Codepoint: """😣""");

    /// <summary>Represents the neutral face emoji.</summary>
    /// <remarks>
    ///     The Neutral Face emoji is used to express a lack of specific expression or emotion and is represented by the
    ///     Unicode character '😐'.
    /// </remarks>
    public static readonly UnicodeCharacter NeutralFace = new(Code: "neutral_face", Codepoint: """😐""");

    /// <summary>Represents the slightly smiling face emoji.</summary>
    /// <remarks>
    ///     The Slightly Smiling Face emoji is used to convey a gentle smile and a positive or neutral sentiment,
    ///     represented by the Unicode character '🙂'.
    /// </remarks>
    public static readonly UnicodeCharacter SlightlySmilingFace = new(Code: "slightly_smiling_face", Codepoint: """🙂""");

    /// <summary>Represents the smiling face emoji.</summary>
    /// <remarks>
    ///     The Smiling Face emoji is used to convey happiness, warmth, and friendliness, and is represented by the
    ///     Unicode character '😊'.
    /// </remarks>
    public static readonly UnicodeCharacter SmilingFace = new(Code: "smiling_face", Codepoint: """😊""");

    /// <summary>Represents the combination of a blood drop and a completed hourglass emoji.</summary>
    /// <remarks>
    ///     The BloodWithHourglassDone emoji is indicative of the completion of time or a task associated with blood and
    ///     is represented by the Unicode characters '🩸⌛'.
    /// </remarks>
    public static readonly UnicodeCharacter BloodWithHourglassDone = new(Code: "blood_with_hourglass_done", Codepoint: """🩸⌛""");

    /// <summary>Represents the blood with hourglass not done emoji.</summary>
    /// <remarks>
    ///     The Blood with Hourglass Not Done emoji combines the symbol of blood with an hourglass that indicates ongoing
    ///     or unfinished timing. This is represented by the Unicode characters '🩸⏳'.
    /// </remarks>
    public static readonly UnicodeCharacter BloodWithHourglassNotDone = new(Code: "blood_with_hourglass_not_done", Codepoint: """🩸⏳""");

    /// <summary>Represents the prohibited emoji.</summary>
    /// <remarks>
    ///     The Prohibited emoji is used to signify a restriction or a prohibition and is represented by the Unicode
    ///     character '🚫'.
    /// </remarks>
    public static readonly UnicodeCharacter Prohibited = new(Code: "prohibited", Codepoint: """🚫""");

    /// <summary>Represents the princess emoji.</summary>
    /// <remarks>
    ///     The princess emoji is often used to convey themes of royalty, elegance, or fancifulness and is represented by
    ///     the Unicode character '👸'.
    /// </remarks>
    public static readonly UnicodeCharacter Princess = new(Code: "princess", Codepoint: """👸""");

    /// <summary>Represents the prince emoji.</summary>
    /// <remarks>
    ///     The Prince emoji is used to symbolize royalty or a male prince figure and is represented by the Unicode
    ///     character '🤴'.
    /// </remarks>
    public static readonly UnicodeCharacter Prince = new(Code: "prince", Codepoint: """🤴""");

    /// <summary>Represents the crown emoji.</summary>
    /// <remarks>
    ///     The Crown emoji is often used to signify royalty, power, or leadership and is represented by the Unicode
    ///     character '👑'.
    /// </remarks>
    public static readonly UnicodeCharacter Crown = new(Code: "crown", Codepoint: """👑""");

    /// <summary>Represents the male symbol emoji.</summary>
    /// <remarks>
    ///     The Male symbol emoji is commonly used to represent male gender and is represented by the Unicode character
    ///     '♂'.
    /// </remarks>
    public static readonly UnicodeCharacter Male = new(Code: "male", Codepoint: """♂""");

    /// <summary>Represents the female gender symbol emoji.</summary>
    /// <remarks>The Female emoji symbolizes femininity and is represented by the Unicode character '♀'.</remarks>
    public static readonly UnicodeCharacter Female = new(Code: "female", Codepoint: """♀""");

    /// <summary>Represents the medium white circle emoji.</summary>
    /// <remarks>
    ///     The Medium White Circle emoji is used to depict a medium-sized white circle and is represented by the Unicode
    ///     character '⚪︎'.
    /// </remarks>
    public static readonly UnicodeCharacter MediumWhiteCircle = new(Code: "medium_white_circle", Codepoint: """⚪︎""");

    /// <summary>Represents the not equal sign emoji.</summary>
    /// <remarks>The NotEqual emoji is utilized to express inequality and is represented by the Unicode character '≠'.</remarks>
    public static readonly UnicodeCharacter NotEqual = new(Code: "not_equal", Codepoint: """≠""");

    /// <summary>Represents the right arrow emoji.</summary>
    /// <remarks>
    ///     The Right Arrow emoji is used to signify direction or movement towards the right and is represented by the
    ///     Unicode character '→'.
    /// </remarks>
    public static readonly UnicodeCharacter RightArrow = new(Code: "arrow_right", Codepoint: """→""");
}
