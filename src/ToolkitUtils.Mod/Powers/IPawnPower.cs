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
using RimWorld;
using Verse;

namespace ToolkitUtils.Mod.Domain.Powers;

/// <summary>Defines a contract for representing powers associated with pawns in the system.</summary>
/// <remarks>
///     This interface is designed to encapsulate the fundamental attributes and characteristics of powers that can be
///     assigned to pawns. It outlines key properties such as the power's definition, level, cost, duration, range, and
///     additional focusing on specifics. The implementation of this interface enables differentiation between various
///     types of pawn powers while ensuring consistency across systems that use such powers.
/// </remarks>
public interface IPawnPower<out TDef> : IIdentifiable where TDef : Def
{
    /// <summary>
    ///     Represents the base definition or identifier associated with a power in the context of the IPawnPower
    ///     interface. This property provides a concrete reference to a specific Def instance, serving as the defining
    ///     characteristic or unique descriptor of the power.
    /// </summary>
    TDef Def { get; }

    /// <summary>
    ///     Defines the type of target that the power can be applied to within the context of pawn powers in the
    ///     ToolkitUtils Mod. This property determines the applicable target categories such as Pawns, Animals, or Locations,
    ///     as governed by the enum PowerTargetType.
    /// </summary>
    PowerTargetType TargetType { get; }

    /// <summary>
    ///     Defines the modifier used to constrain or refine the targeting logic of a pawn power in the context of the
    ///     ToolkitUtils Mod. It specifies additional rules or limitations on applicable targets, complementing the primary
    ///     target type defined by the PowerTargetType enumeration.
    /// </summary>
    PowerTargetModifier TargetModifier { get; }

    /// <summary>
    ///     Represents a collection of factions associated with the pawn power within the context of the IPawnPower
    ///     interface. This property defines the specific factions that are impacted or associated with the power, allowing for
    ///     faction-targeted interactions, restrictions, or effects.
    /// </summary>
    IReadOnlyList<IFaction> Factions { get; }

    /// <summary>
    ///     Represents a collection of hierarchical tiers or levels associated with a pawn power. This property defines
    ///     the progression or variation in functionality, attributes, or characteristics of the power across its different
    ///     stages. Each tier may include specific properties such as cost, duration, range, and other tier-related parameters.
    /// </summary>
    IReadOnlyList<IPawnPowerTier<TDef>> Tiers { get; }
}

public interface IPawnPowerTier<out TDef> : IIdentifiable where TDef : Def
{
    /// <summary>
    ///     Represents the level or rank of the power within the context of the IPawnPower interface. This property
    ///     indicates the minimum or current level required for the power to be active, used, or applicable, effectively
    ///     serving as a measure of its intensity or progression.
    /// </summary>
    int Level { get; }

    /// <summary>
    ///     Represents the cost associated with executing a specific pawn power. This property denotes the resource
    ///     expenditure or requirement needed for using the power, such as energy, mana, or other relevant metrics. It serves
    ///     as a configurable attribute depending on the implementation of a given ability or power.
    /// </summary>
    float Cost { get; }

    /// <summary>
    ///     Represents the duration of a specific power within the context of a pawn's abilities. This value determines
    ///     how long the effects of the power will last once it has been activated. It is expressed as a float to allow for
    ///     flexible time manipulation, such as fractions of seconds or scaled durations based on external factors like
    ///     modifiers or power-specific configurations.
    /// </summary>
    TimeSpan Duration { get; }

    /// <summary>
    ///     Represents the maximum effective distance at which the power can be applied or activated. This property
    ///     defines the operational range of the power, determining its spatial limitations within the context of the game
    ///     environment.
    /// </summary>
    float Range { get; }

    /// <summary>
    ///     Represents the time required to cast the power associated with a pawn. This property indicates the duration,
    ///     in seconds or ticks, taken to complete the casting action for the respective power within the context of the game's
    ///     mechanics or mod framework.
    /// </summary>
    float CastTime { get; }

    /// <summary>
    ///     Represents the base definition or identifier associated with a power in the context of the IPawnPower
    ///     interface. This property provides a concrete reference to a specific Def instance, serving as the defining
    ///     characteristic or unique descriptor of the power.
    /// </summary>
    TDef? Def { get; }
}

/// <summary>Represents a faction with its associated definition.</summary>
/// <remarks>
///     This interface is used to define the behavior and properties of a faction within the mod, including its
///     associated definition for in-game behavior and identification.
/// </remarks>
public interface IFaction : IIdentifiable
{
    /// <summary>
    ///     Represents a definition that acts as a unique identifier or descriptor within the defined context. Commonly
    ///     associated with properties, abilities, or configurations conforming to logical or domain-specific constraints.
    /// </summary>
    FactionDef Def { get; }
}

/// <summary>Represents the target type of corpses for a power in the system.</summary>
/// <remarks>
///     This enum member is used to specify that a power can interact specifically with corpses as its target.
///     Incorporating this target type allows for defining powers that affect or utilize corpses, enabling diverse
///     functionalities related to corpse manipulation within the system.
/// </remarks>
[Flags]
public enum PowerTargetType
{
    /// <summary>Represents the absence of any specific target type for a pawn power.</summary>
    /// <remarks>
    ///     This enum member is used when a pawn power does not apply to any particular target category, effectively
    ///     signifying that no targeting restrictions or requirements are imposed. It serves as a default or null value within
    ///     the <see cref="PowerTargetType" /> enumeration.
    /// </remarks>
    None,

    /// <summary>Represents a location as a potential target for a pawn power.</summary>
    /// <remarks>
    ///     This enum member specifies that the power can be applied to a specific position or area in the game world. It
    ///     allows for focusing on functionalities that are spatially focused, enabling effects such as environment alterations
    ///     or area-based influences.
    /// </remarks>
    Locations,

    /// <summary>Represents the targeting of the entity that uses or holds the pawn power.</summary>
    /// <remarks>
    ///     This enum member indicates that the pawn power exclusively affects the user itself, allowing self-application
    ///     or self-beneficial effects without influencing external entities or objects in the game environment.
    /// </remarks>
    Self,

    /// <summary>Represents a power target type that applies specifically to pawns within the game.</summary>
    /// <remarks>
    ///     This member defines powers that focus on individual or groups of pawns, enabling abilities or effects to be
    ///     directed at these game entities. It is used when a power's scope is restricted to pawns, providing clarity and
    ///     precision in determining applicable targets.
    /// </remarks>
    Pawns,

    /// <summary>Indicates that the target of the pawn power is fires or elements related to fire within the game world.</summary>
    /// <remarks>
    ///     This enum member is used to specify that a pawn power applies to fire-based entities or conditions, enabling
    ///     interactions such as controlling, extinguishing, or amplifying fires. It helps to restrict or define the effect of
    ///     a pawn power to scenarios involving fire elements.
    /// </remarks>
    Fires,

    /// <summary>Represents a pawn power that can focus on buildings within the game world.</summary>
    /// <remarks>
    ///     This enum member specifies that the power is applicable to structures or constructs categorized as buildings.
    ///     It allows developers to create powers designed to interact with or influence these fixed game-world elements.
    /// </remarks>
    Buildings,

    /// <summary>Represents a target type referring to items that a power can influence or interact with.</summary>
    /// <remarks>
    ///     This enum member indicates that the pawn power is applicable to in-game items, enabling the ability to
    ///     manipulate, affect, or otherwise interact with objects classified as items within the game world.
    /// </remarks>
    Items,

    /// <summary>Specifies that the power can focus on animals within the effective range.</summary>
    /// <remarks>
    ///     The Animals target type is used for abilities, spells, or actions that are specifically applicable to
    ///     creatures classified as animals. It allows the targeting of entities that fall under the animal category, enabling
    ///     selective interactions with them in gameplay mechanics.
    /// </remarks>
    Animals,

    /// <summary>Represents the category of targets consisting of bloodfeeding creatures for a pawn power.</summary>
    /// <remarks>
    ///     This enum member is used to specify that a pawn power specifically targets entities classified as
    ///     bloodfeeders. It helps to define precise targeting mechanics within the <see cref="PowerTargetType" /> enumeration,
    ///     ensuring the power is applied exclusively to this particular group of entities.
    /// </remarks>
    Bloodfeeders,

    /// <summary>Represents a target type comprised of corpses for a pawn power.</summary>
    /// <remarks>
    ///     This enum member is used when a pawn power specifically applies to corpses, enabling the selection or
    ///     interaction with deceased entities within the <see cref="PowerTargetType" /> enumeration.
    /// </remarks>
    Corpses,

    /// <summary>Represents a power target type specifically applicable to human pawns.</summary>
    /// <remarks>
    ///     This enum member is used when a pawn power is intended to focus on humans exclusively. It indicates that the
    ///     power's effects or functionality are restricted to the human category within the <see cref="PowerTargetType" />
    ///     enumeration.
    /// </remarks>
    Humans,

    /// <summary>Represents a power target type that applies to mechanoids within the game.</summary>
    /// <remarks>
    ///     This enum member specifies that the power or ability can be targeted or applied specifically to mechanoids,
    ///     which are mechanical entities distinct from organic characters.
    /// </remarks>
    Mechanoids,

    /// <summary>Represents plants as a specific target type for a pawn power.</summary>
    /// <remarks>
    ///     This enum member is used when a pawn power is designed to interact specifically with plants, enabling actions
    ///     or effects that directly relate to flora within the game environment. It defines a target category for powers that
    ///     might grow, damage, modify, or otherwise influence plants.
    /// </remarks>
    Plants,

    /// <summary>Represents a target type for powers that are specifically associated with mutants.</summary>
    /// <remarks>
    ///     This enum member allows powers to explicitly focus on entities categorized as mutants. It enables specialized
    ///     interactions or effects applicable only to this unique entity type within the system.
    /// </remarks>
    Mutants,
}

/// <summary>Specifies various modifiers that can restrict or refine the applicability of a power's target.</summary>
/// <remarks>
///     This enumeration defines discrete conditions or constraints that can be applied to filter or modify the
///     targets that a particular power is allowed to interact with. Each value represents a specific rule or set of
///     characteristics that further narrows down or qualifies the target pool.
/// </remarks>
[Flags]
public enum PowerTargetModifier
{
    /// <summary>
    ///     Represents the absence of any specific modification or restriction to the target selection criteria for a
    ///     power.
    /// </summary>
    /// <remarks>
    ///     When this modifier is used, no special constraints or alterations are applied to the default target selection
    ///     logic of the power. It signifies a neutral or unrestricted behavior in terms of choosing applicable targets.
    /// </remarks>
    None,

    /// <summary>Restricts the power target to entities that belong to specific factions.</summary>
    /// <remarks>
    ///     This enum member is used to ensure that the power effect only applies to targets associated with predefined or
    ///     selected factions. It is typically used to limit the range of valid targets based on their factional affiliation,
    ///     enabling more precise control over power application.
    /// </remarks>
    OnlyFactions,

    /// <summary>Specifies that the power can only focus on objects or entities classified as flammable.</summary>
    /// <remarks>
    ///     This modifier limits the power's applicability to targets capable of catching fire, ensuring selective and
    ///     context-specific functionality. Ideal for abilities or effects that involve fire-based interactions or combustion.
    /// </remarks>
    OnlyFlammables,

    /// <summary>Restricts the power target to a specific, predefined object within the game world.</summary>
    /// <remarks>
    ///     This enum member is used to designate that the pawn power can only affect a particular, explicitly identified
    ///     object or thing. The target must match the specified criteria, ensuring precise application of the power to the
    ///     desired object. This modifier enforces strict limitations on the valid target for the power effect.
    /// </remarks>
    SpecificThing,

    /// <summary>Indicates that the target must be selectable as a prerequisite for the power's applicability.</summary>
    /// <remarks>
    ///     This modifier enforces a constraint where the target of the power must fulfill requirements that make it
    ///     explicitly selectable. It serves as a behavioral filter for determining valid interactions involving powers applied
    ///     to specific entities.
    /// </remarks>
    MustBeSelectable,

    /// <summary>Represents a restriction indicating that the power cannot focus on doors.</summary>
    /// <remarks>
    ///     This modifier ensures that the power explicitly excludes door entities from being valid targets. It is useful
    ///     for defining powers that interact specifically with other types of objects or entities in the environment while
    ///     deliberately avoiding doors.
    /// </remarks>
    NeverDoors,

    /// <summary>Specifies that the target cannot be incapacitated, ensuring only active, capable entities are eligible.</summary>
    /// <remarks>
    ///     This modifier restricts the applicability of a power to targets that are never in an incapacitated state. It
    ///     is particularly useful in contexts where powers or effects should not be applied to incapacitated entities, such as
    ///     unconscious or otherwise disabled pawns.
    /// </remarks>
    NeverIncapacitated,

    /// <summary>
    ///     Specifies modifiers that define constraints or conditions under which a power can focus on entities in the
    ///     game environment.
    /// </summary>
    /// <remarks>
    ///     This enumeration delineates a variety of targeting restrictions and filters used to determine the
    ///     applicability of certain powers. It includes specific conditions such as focusing on factions, damaged items,
    ///     incapacitated pawns, or other specialized criteria. These modifiers are essential for refining the scope and
    ///     applicability of powers, ensuring they align with gameplay mechanics and circumstances.
    /// </remarks>
    NeverHostileFaction,

    /// <summary>Restricts the target selection to entities or factions that share the same ideology.</summary>
    /// <remarks>
    ///     This modifier ensures that the applicable targets belong to a matching ideology, aligning power effects with
    ///     ideological boundaries. It is particularly useful in scenarios where ideological conformity is a fundamental
    ///     requirement for the power's applicability.
    /// </remarks>
    OnlySameIdeology,

    /// <summary>Restricts targeting to objects or elements that influence specific regions on the map.</summary>
    /// <remarks>
    ///     This modifier is used to ensure that powers only apply to entities or objects that have an impact on defined
    ///     map regions, such as areas affected by terrain, structures, or other spatial elements. It serves to refine the
    ///     scope of a pawn power to focus on region-related interactions, excluding global or non-regional effects.
    /// </remarks>
    OnlyThingsAffectingRegions,

    /// <summary>Restricts the power's target to only include things that are damaged.</summary>
    /// <remarks>
    ///     This modifier ensures that the power can only be applied to entities or objects that are not in optimal
    ///     condition and have sustained damage. It is typically used in scenarios where repairing, weakening, or specifically
    ///     interacting with damaged items or entities is required.
    /// </remarks>
    OnlyDamagedThings,

    /// <summary>Specifies that the target must be an object on the map that can be auto-attacked.</summary>
    /// <remarks>
    ///     This enum member enforces the condition where the target must fulfill the criteria of being an entity on the
    ///     map that is eligible for automatic attacks. It is typically used in situations where powers or actions are
    ///     restricted to objects that are valid auto-attack candidates.
    /// </remarks>
    MapObjectMustBeAutoAttackable,

    /// <summary>Restricts the power's applicability to only pawns that are incapacitated.</summary>
    /// <remarks>
    ///     This modifier ensures that the power is only applicable to targets that are unable to perform actions due to
    ///     incapacitation. Typically used in scenarios where the power is intended for aiding, attacking, or interacting with
    ///     incapacitated pawns.
    /// </remarks>
    OnlyIncapacitatedPawns,

    /// <summary>Restricts the applicability of a pawn power to colonist pawns only.</summary>
    /// <remarks>
    ///     This enum member ensures that the pawn power targets only pawns classified as colonists. It is used to create
    ///     gameplay conditions or effects specifically tailored to colonists, excluding other pawn types such as prisoners,
    ///     visitors, or allies.
    /// </remarks>
    OnlyColonists,

    /// <summary>Restricts the power's target selection to only prisoners.</summary>
    /// <remarks>
    ///     This modifier ensures that the power can only be applied to entities classified as prisoners, typically
    ///     individuals in captivity under specific conditions. It supports gameplay mechanics involving detainees and
    ///     restricts targeting to align with such scenarios.
    /// </remarks>
    OnlyPrisoners,

    /// <summary>Restricts the targeting of a power specifically to entities classified as slaves.</summary>
    /// <remarks>
    ///     This member of the <c>PowerTargetModifier</c> enumeration ensures that the power can only affect slaves. It is
    ///     used to enforce target constraints in scenarios where the interaction is limited to this specific group.
    /// </remarks>
    OnlySlaves,

    /// <summary>Allows targeting of pawns experiencing minor mental breaks.</summary>
    /// <remarks>
    ///     This modifier permits powers to include pawns undergoing minor mental breaks as valid targets. It expands the
    ///     potential target range of a power by including these specific mental states, which may otherwise be excluded by
    ///     default focusing on rules within the <see cref="PowerTargetModifier" /> enumeration.
    /// </remarks>
    AllowMinorMentalBreaks,

    /// <summary>Restricts the target to pawns that are under the direct control of the player or user.</summary>
    /// <remarks>
    ///     This enum member ensures that the pawn power is applied exclusively to controlled pawns. It is typically used
    ///     in scenarios where the effect or ability is intended to operate only on pawns actively commanded or managed by the
    ///     player, excluding NPCs or independent entities.
    /// </remarks>
    OnlyControlledPawns,

    /// <summary>Restricts the power's target to only prisoners belonging to the colony.</summary>
    /// <remarks>
    ///     This enum member is used to ensure that a pawn power is applied exclusively to prisoners that are under the
    ///     control or ownership of the colony. It establishes a targeting limitation specifically for individuals classified
    ///     as prisoners in the context of the colony's management system.
    /// </remarks>
    OnlyPrisonersOfColony,

    /// <summary>Restricts the power's applicability to targets that are psychically sensitive.</summary>
    /// <remarks>
    ///     This modifier ensures that only entities with psychic sensitivity are eligible as valid targets for the
    ///     associated power. It is commonly used in scenarios where psychic capabilities or receptiveness are a prerequisite
    ///     for interaction or effect.
    /// </remarks>
    OnlyPsychicSensitive,

    /// <summary>Restricts the power target to Anima Trees specifically.</summary>
    /// <remarks>
    ///     This enum member is used when a power explicitly applies only to Anima Trees. It ensures that the effect or
    ///     interaction defined by the power is constrained to this particular target type, excluding all other map objects or
    ///     entities.
    /// </remarks>
    OnlyAnimaTrees,

    /// <summary>Restricts the effect of a pawn power to target classified as bloodfeeders.</summary>
    /// <remarks>
    ///     This enum member is used in scenarios where a pawn power or ability is designed to apply specifically to
    ///     entities or characters with the trait or characteristic of being bloodfeeders. It ensures that the targeted effect
    ///     is limited to this particular category, excluding all other target types.
    /// </remarks>
    OnlyBloodfeeders,

    /// <summary>Restricts the power target to repairable mechanoids, identifying mechanical entities that can undergo repairs.</summary>
    /// <remarks>
    ///     This modifier ensures that the power can only be applied to mechanoid entities capable of being repaired. It
    ///     is used to filter or limit the scope of eligible targets for specific powers, providing more precise targeting
    ///     criteria.
    /// </remarks>
    OnlyRepairableMechanoids,

    /// <summary>Represents a modifier that restricts power targeting based on the category a thing belongs to.</summary>
    /// <remarks>
    ///     This modifier is used to fine-tune power applicability by narrowing the scope of valid targets to specific
    ///     categories of objects or entities. It ensures that the power interacts exclusively with entities classified under a
    ///     defined thing category.
    /// </remarks>
    ThingCategory,

    /// <summary>Specifies that the pawn power applies exclusively to doors as a target type.</summary>
    /// <remarks>
    ///     This enum member is used to restrict the application of a pawn power to objects classified as doors. It
    ///     ensures that the power only interacts with such entities, excluding all other target categories.
    /// </remarks>
    OnlyDoors,

    /// <summary>Restricts the pawn power's target application to corpses only.</summary>
    /// <remarks>
    ///     This modifier ensures that the pawn power can only interact with or affect targets classified as corpses. It
    ///     is used to specify corpse-targeting behavior within the <see cref="PowerTargetModifier" /> enumeration.
    /// </remarks>
    OnlyCorpses,
}
