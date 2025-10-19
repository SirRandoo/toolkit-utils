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
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using RimWorld;
using ToolkitUtils.Mod.Localization;
using Verse;

namespace ToolkitUtils.Mod.Services;

/// <summary>Interface representing the service responsible for pawn-related functionalities in the application.</summary>
public interface IPawnService
{
    /// <summary>Asynchronously generates a pawn based on the provided generation request.</summary>
    /// <param name="request">
    ///     The request containing the parameters for pawn generation, including kind, faction, traits, and
    ///     specific properties.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains an <see cref="IResult{Pawn}" />,
    ///     which includes the status and outcome of the pawn generation process.
    /// </returns>
    Task<Result<Pawn>> GeneratePawnAsync(PawnGenerationRequest request);
}

/// <summary>
///     Service responsible for managing pawn-related operations, including pawn generation. Implements the
///     <see cref="IPawnService" /> interface. Utilizes routing capabilities through <see cref="RouterService" /> for
///     operations requiring external processing.
/// </summary>
public sealed class PawnService(RouterService router) : IPawnService
{
    /// <inheritdoc />
    public async Task<Result<Pawn>> GeneratePawnAsync(PawnGenerationRequest request)
    {
        Pawn? pawn = await router.RouteToMainAsync(PawnGenerator.GeneratePawn, (Verse.PawnGenerationRequest)request);

        return pawn != null ? Result.Ok(pawn) : Result.Fail<Pawn>(Translation.None);
    }
}

/// <summary>
///     Represents a request for generating a pawn with specified attributes and constraints. This provides extensive
///     customization options, such as specifying pawn kind, faction, generation context, abilities, traits, xenotype,
///     developmental stage, and other parameters.
/// </summary>
[PublicAPI]
public sealed record PawnGenerationRequest(
    PawnKindDef Kind,
    Faction? Faction = null,
    PawnGenerationContext Context = PawnGenerationContext.NonPlayer,
    int Tile = -1,
    bool ForceGenerateNewPawn = false,
    bool AllowDead = false,
    bool AllowDowned = false,
    bool CanGeneratePawnRelations = true,
    bool MustBeCapableOfViolence = false,
    float ColonistRelationChanceFactor = 1f,
    bool ForceAddFreeWarmLayerIfNeeded = false,
    bool AllowGay = true,
    bool AllowPregnant = false,
    bool AllowFood = true,
    bool AllowAddictions = true,
    bool Inhabitant = false,
    bool CertainlyBeenInCryptosleep = false,
    bool ForceRedressWorldPawnIfFormerColonist = false,
    bool WorldPawnFactionDoesntMatter = false,
    float BiocodeWeaponChance = 0f,
    float BiocodeApparelChance = 0f,
    Pawn? ExtraPawnForExtraRelationChance = null,
    float RelationWithExtraPawnChanceFactor = 1f,
    Predicate<Pawn>? ValidatorPreGear = null,
    Predicate<Pawn>? ValidatorPostGear = null,
    IReadOnlyCollection<TraitDef>? ForcedTraits = null,
    IReadOnlyCollection<TraitDef>? ProhibitedTraits = null,
    float? MinChanceToRedressWorldPawn = null,
    float? FixedBiologicalAge = null,
    float? FixedChronologicalAge = null,
    Gender? FixedGender = null,
    string? FixedLastName = null,
    string? FixedBirthName = null,
    RoyalTitleDef? FixedTitle = null,
    Ideo? FixedIdeo = null,
    bool ForceNoIdeo = false,
    bool ForceNoBackstory = false,
    bool ForbidAnyTitle = false,
    bool ForceDead = false,
    IReadOnlyCollection<GeneDef>? ForcedGenes = null,
    IReadOnlyCollection<GeneDef>? ForcedEndogenes = null,
    XenotypeDef? ForcedXenotype = null,
    CustomXenotype? ForcedCustomXenotype = null,
    IReadOnlyCollection<XenotypeDef>? AllowedXenotypes = null,
    float ForceBaselinerChance = 0f,
    DevelopmentalStage DevelopmentalStage = DevelopmentalStage.Adult,
    Func<XenotypeDef, PawnKindDef>? PawnKindDefGetter = null,
    FloatRange? ExcludeBiologicalAgeRange = null,
    FloatRange? BiologicalAgeRange = null,
    bool ForceRecruitable = false,
    bool DontGiveWeapon = false,
    bool OnlyUseForcedBackstories = false,
    int MaximumAgeTraits = -1,
    int MinimumAgeTraits = 0,
    bool ForceNoGear = false,
    bool IsCreepJoiner = false,
    Predicate<Pawn>? RedressValidator = null
)
{
    /// <summary>
    ///     Defines a specific type or category of pawn that can be used in the game's context, determining its behavior,
    ///     appearance, abilities, and other characteristics.
    /// </summary>
    public PawnKindDef PawnKindDef => PawnKindDefGetter?.Invoke(ForcedXenotype!) ?? Kind;

    public static implicit operator PawnGenerationRequest(Verse.PawnGenerationRequest request) =>
        new(
            request.KindDef,
            request.Faction,
            request.Context,
            request.Tile,
            request.ForceGenerateNewPawn,
            request.AllowDead,
            request.AllowDowned,
            request.CanGeneratePawnRelations,
            request.MustBeCapableOfViolence,
            request.ColonistRelationChanceFactor,
            request.ForceAddFreeWarmLayerIfNeeded,
            request.AllowGay,
            request.AllowPregnant,
            request.AllowFood,
            request.AllowAddictions,
            request.Inhabitant,
            request.CertainlyBeenInCryptosleep,
            request.ForceRedressWorldPawnIfFormerColonist,
            request.WorldPawnFactionDoesntMatter,
            request.BiocodeWeaponChance,
            request.BiocodeApparelChance,
            request.ExtraPawnForExtraRelationChance,
            request.RelationWithExtraPawnChanceFactor,
            request.ValidatorPreGear,
            request.ValidatorPostGear,
            request.ForcedTraits.ToArray(),
            request.ProhibitedTraits.ToArray(),
            request.MinChanceToRedressWorldPawn,
            request.FixedBiologicalAge,
            request.FixedChronologicalAge,
            request.FixedGender,
            request.FixedLastName,
            request.FixedBirthName,
            request.FixedTitle,
            request.FixedIdeo,
            request.ForceNoIdeo,
            request.ForceNoBackstory,
            request.ForbidAnyTitle,
            request.ForceDead,
            ForcedGenes: null,
            request.ForcedEndogenes,
            request.ForcedXenotype,
            request.ForcedCustomXenotype,
            request.AllowedXenotypes,
            request.ForceBaselinerChance,
            request.AllowedDevelopmentalStages,
            request.PawnKindDefGetter,
            request.ExcludeBiologicalAgeRange,
            request.BiologicalAgeRange,
            request.ForceRecruitable,
            request.DontGiveWeapon,
            request.OnlyUseForcedBackstories,
            request.MaximumAgeTraits,
            request.MinimumAgeTraits,
            request.ForceNoGear,
            request.IsCreepJoiner,
            request.RedressValidator
        );

    public static implicit operator Verse.PawnGenerationRequest(PawnGenerationRequest request) =>
        new(
            request.PawnKindDef,
            request.Faction,
            request.Context,
            request.Tile,
            request.ForceGenerateNewPawn,
            request.AllowDead,
            request.AllowDowned,
            request.CanGeneratePawnRelations,
            request.MustBeCapableOfViolence,
            request.ColonistRelationChanceFactor,
            request.ForceAddFreeWarmLayerIfNeeded,
            request.AllowGay,
            request.AllowPregnant,
            request.AllowFood,
            request.AllowAddictions,
            request.Inhabitant,
            request.CertainlyBeenInCryptosleep,
            request.ForceRedressWorldPawnIfFormerColonist,
            request.WorldPawnFactionDoesntMatter,
            request.BiocodeWeaponChance,
            request.BiocodeApparelChance,
            request.ExtraPawnForExtraRelationChance,
            request.RelationWithExtraPawnChanceFactor,
            request.ValidatorPreGear,
            request.ValidatorPostGear,
            request.ForcedTraits,
            request.ProhibitedTraits,
            request.MinChanceToRedressWorldPawn,
            request.FixedBiologicalAge,
            request.FixedChronologicalAge,
            request.FixedGender,
            request.FixedLastName,
            request.FixedBirthName,
            request.FixedTitle,
            request.FixedIdeo,
            request.ForceNoIdeo,
            request.ForceNoBackstory,
            request.ForbidAnyTitle,
            request.ForceDead,
            request.ForcedGenes?.ToList(),
            request.ForcedEndogenes?.ToList(),
            request.ForcedXenotype,
            request.ForcedCustomXenotype,
            request.AllowedXenotypes?.ToList(),
            request.ForceBaselinerChance,
            request.DevelopmentalStage,
            request.PawnKindDefGetter,
            request.ExcludeBiologicalAgeRange,
            request.BiologicalAgeRange,
            request.ForceRecruitable,
            request.DontGiveWeapon,
            request.OnlyUseForcedBackstories,
            request.MaximumAgeTraits,
            request.MinimumAgeTraits,
            request.ForceNoGear
        )
        {
            RedressValidator = request.RedressValidator,
        };

    /// <summary>
    ///     Provides a fluent interface for constructing instances of the PawnGenerationRequest. The <c>Builder</c> class
    ///     allows fine-grained configuration of various parameters relevant to the pawn generation process, ensuring
    ///     flexibility and clarity during object creation.
    /// </summary>
    [PublicAPI]
    public class Builder(PawnKindDef kind)
    {
        private bool _allowAddictions = true;
        private bool _allowDead;
        private bool _allowDowned;
        private IReadOnlyCollection<XenotypeDef>? _allowedXenotypes;
        private bool _allowFood = true;
        private bool _allowGay = true;
        private bool _allowPregnant;
        private float _biocodeApparelChance;
        private float _biocodeWeaponChance;
        private FloatRange? _biologicalAgeRange;
        private bool _canGeneratePawnRelations = true;
        private bool _certainlyBeenInCryptosleep;
        private float _colonistRelationChanceFactor = 1f;
        private PawnGenerationContext _context = PawnGenerationContext.NonPlayer;
        private DevelopmentalStage _developmentalStage = DevelopmentalStage.Adult;
        private bool _dontGiveWeapon;
        private FloatRange? _excludeBiologicalAgeRange;
        private Pawn? _extraPawnForExtraRelationChance;
        private Faction? _faction;
        private float? _fixedBiologicalAge;
        private string? _fixedBirthName;
        private float? _fixedChronologicalAge;
        private Gender? _fixedGender;
        private Ideo? _fixedIdeo;
        private string? _fixedLastName;
        private RoyalTitleDef? _fixedTitle;
        private bool _forbidAnyTitle;
        private bool _forceAddFreeWarmLayerIfNeeded;
        private float _forceBaselinerChance;
        private CustomXenotype? _forcedCustomXenotype;
        private bool _forceDead;
        private IReadOnlyCollection<GeneDef>? _forcedEndogenes;
        private IReadOnlyCollection<GeneDef>? _forcedGenes;
        private IReadOnlyCollection<TraitDef>? _forcedTraits;
        private XenotypeDef? _forcedXenotype;
        private bool _forceGenerateNewPawn;
        private bool _forceNoBackstory;
        private bool _forceNoGear;
        private bool _forceNoIdeo;
        private bool _forceRecruitable;
        private bool _forceRedressWorldPawnIfFormerColonist;
        private bool _inhabitant;
        private int _maximumAgeTraits = -1;
        private float? _minChanceToRedressWorldPawn;
        private int _minimumAgeTraits;
        private bool _mustBeCapableOfViolence;
        private bool _onlyUseForcedBackstories;
        private Func<XenotypeDef, PawnKindDef>? _pawnKindDefGetter;
        private IReadOnlyCollection<TraitDef>? _prohibitedTraits;
        private Predicate<Pawn>? _redressValidator;
        private float _relationWithExtraPawnChanceFactor = 1f;
        private int _tile = -1;
        private Predicate<Pawn>? _validatorPostGear;
        private Predicate<Pawn>? _validatorPreGear;
        private bool _worldPawnFactionDoesntMatter;

        [field: MaybeNull] private PawnKindDef KindDef => _pawnKindDefGetter != null ? _pawnKindDefGetter(_forcedXenotype!) : field!;

        /// <summary>
        ///     Sets a redress validator predicate to be used for validating the pawn after redress during the generation
        ///     process.
        /// </summary>
        /// <param name="validator">A predicate that defines the validation logic for the pawn after being redressed.</param>
        /// <returns>
        ///     The current <see cref="PawnGenerationRequest.Builder" /> instance with the specified redress validator
        ///     configured.
        /// </returns>
        public Builder WithRedressValidator(Predicate<Pawn> validator)
        {
            _redressValidator = validator;

            return this;
        }

        /// <summary>Specifies whether to force the generation of a new pawn even if an existing pawn meets the criteria.</summary>
        /// <param name="forceGenerateNewPawn">
        ///     A boolean value indicating whether to force the creation of a new pawn. Defaults to
        ///     true.
        /// </param>
        /// <returns>The current instance of the <see cref="Builder" /> for method chaining.</returns>
        public Builder ForceGenerateNewPawn(bool forceGenerateNewPawn = true)
        {
            _forceGenerateNewPawn = forceGenerateNewPawn;

            return this;
        }

        /// <summary>Specifies whether pawn relations can be generated during the pawn generation process.</summary>
        /// <param name="canGeneratePawnRelations">
        ///     A boolean value indicating if the generation of pawn relations should be
        ///     enabled. Defaults to true.
        /// </param>
        /// <returns>The current <see cref="Builder" /> instance with the updated configuration for generating pawn relations.</returns>
        public Builder CanGeneratePawnRelations(bool canGeneratePawnRelations = true)
        {
            _canGeneratePawnRelations = canGeneratePawnRelations;

            return this;
        }

        /// <summary>Ensures that the generated pawn is capable of performing violent actions.</summary>
        /// <param name="capableOfViolence">
        ///     A boolean value indicating whether the pawn must be capable of violence. Default is
        ///     true.
        /// </param>
        /// <returns>An instance of <see cref="Builder" /> with the updated capability settings for violent actions.</returns>
        public Builder EnsureCapableOfViolence(bool capableOfViolence = true)
        {
            _mustBeCapableOfViolence = capableOfViolence;

            return this;
        }

        /// <summary>Sets the factor that modifies the chance of generating colonist relations for the pawn being created.</summary>
        /// <param name="factor">
        ///     The factor to adjust the chance of pawn relations. A value of 1 maintains the default behavior,
        ///     values greater than 1 increase the chance, and values less than 1 decrease the chance.
        /// </param>
        /// <returns>Returns the builder instance, allowing for method chaining during pawn generation configuration.</returns>
        public Builder WithColonistRelationChanceFactor(float factor)
        {
            _colonistRelationChanceFactor = factor;

            return this;
        }

        /// <summary>Forces the addition of a free warm layer to the generated pawn if necessary.</summary>
        /// <param name="forceAddFreeWarmLayerIfNeeded">
        ///     A boolean indicating whether to force the addition of a free warm clothing
        ///     layer to the pawn.
        /// </param>
        /// <returns>The current builder instance with the specified behavior for adding a free warm layer.</returns>
        public Builder ForceAddFreeWarmLayerIfNeeded(bool forceAddFreeWarmLayerIfNeeded = true)
        {
            _forceAddFreeWarmLayerIfNeeded = forceAddFreeWarmLayerIfNeeded;

            return this;
        }

        /// <summary>Configures whether the generated pawn is allowed to have the "gay" trait.</summary>
        /// <param name="allowGay">
        ///     A boolean value indicating whether pawns with the "gay" trait are allowed. When set to true, the
        ///     "gay" trait is permitted; otherwise, it is prohibited.
        /// </param>
        /// <returns>The current instance of the <see cref="Builder" />, enabling method chaining.</returns>
        public Builder AllowGay(bool allowGay = true)
        {
            _allowGay = allowGay;

            return this;
        }

        /// <summary>Specifies whether to allow generated pawns to be pregnant during the generation process.</summary>
        /// <param name="allowPregnant">A boolean indicating if pregnancy is allowed for the generated pawn. Default is true.</param>
        /// <returns>An instance of the <see cref="Builder" /> class with the pregnancy allowance setting applied.</returns>
        public Builder AllowPregnant(bool allowPregnant = true)
        {
            _allowPregnant = allowPregnant;

            return this;
        }

        /// <summary>Specifies whether the generated pawn is allowed to consume food during its lifecycle.</summary>
        /// <param name="allowFood">A boolean indicating if the pawn generation process allows the pawn to eat. Default is true.</param>
        /// <returns>The <see cref="PawnGenerationRequest.Builder" /> instance with the updated food allowance setting.</returns>
        public Builder AllowFood(bool allowFood = true)
        {
            _allowFood = allowFood;

            return this;
        }

        /// <summary>Sets whether addictions are allowed during the pawn generation process.</summary>
        /// <param name="allowAddictions">A boolean value indicating if addictions should be allowed. Default is true.</param>
        /// <returns>The builder instance modified with the specified addiction allowance setting.</returns>
        public Builder AllowAddictions(bool allowAddictions = true)
        {
            _allowAddictions = allowAddictions;

            return this;
        }

        /// <summary>Sets whether the generated pawn has certainly been in cryptosleep or not in the generation request.</summary>
        /// <param name="certainlyBeenInCryptosleep">
        ///     A boolean value indicating if the pawn is guaranteed to have been in
        ///     cryptosleep.
        /// </param>
        /// <returns>Returns the current <see cref="Builder" /> instance with the specified value for the cryptosleep property.</returns>
        public Builder CertainlyBeenInCryptosleep(bool certainlyBeenInCryptosleep = true)
        {
            _certainlyBeenInCryptosleep = certainlyBeenInCryptosleep;

            return this;
        }

        /// <summary>
        ///     Forces the system to redress a world pawn if it was previously a colonist. This ensures the pawn's state is
        ///     updated to reflect any changes required.
        /// </summary>
        /// <param name="forceRedressWorldPawnIfFormerColonist">
        ///     Indicates whether to enforce redress for world pawns that were
        ///     former colonists.
        /// </param>
        /// <returns>An updated <see cref="Builder" /> instance with the force redress setting applied.</returns>
        public Builder ForceRedressWorldPawnIfFormerColonist(bool forceRedressWorldPawnIfFormerColonist = true)
        {
            _forceRedressWorldPawnIfFormerColonist = forceRedressWorldPawnIfFormerColonist;

            return this;
        }

        /// <summary>Configures whether the faction of the world pawn is considered irrelevant during pawn creation.</summary>
        /// <param name="worldPawnFactionDoesntMatter">
        ///     A boolean value indicating whether the faction of the world pawn should be
        ///     ignored. If set to true, the faction will not be factored into the pawn generation process.
        /// </param>
        /// <returns>
        ///     Returns the <see cref="Builder" /> instance for method chaining, with the specified world pawn faction
        ///     configuration.
        /// </returns>
        public Builder WorldPawnFactionDoesntMatter(bool worldPawnFactionDoesntMatter = true)
        {
            _worldPawnFactionDoesntMatter = worldPawnFactionDoesntMatter;

            return this;
        }

        /// <summary>Specifies a fixed chance for the generated pawn to have a biocoded weapon.</summary>
        /// <param name="chance">
        ///     The probability (as a float value) that the generated pawn will possess a biocoded weapon, where 0
        ///     represents 0% chance and 1 represents 100% chance.
        /// </param>
        /// <returns>The updated <see cref="Builder" /> instance with the specified biocode weapon chance set.</returns>
        public Builder WithFixedBiocodeWeaponChance(float chance)
        {
            _biocodeWeaponChance = chance;

            return this;
        }

        /// <summary>Sets a fixed chance for apparel to have a biocode attached during pawn generation.</summary>
        /// <param name="chance">The fixed chance value for biocode apparel, ranging from 0 to 1.</param>
        /// <returns>The <see cref="Builder" /> instance with the updated biocode apparel chance, allowing for method chaining.</returns>
        public Builder WithFixedBiocodeApparelChance(float chance)
        {
            _biocodeApparelChance = chance;

            return this;
        }

        /// <summary>Sets an additional pawn to be factored into the calculation of extra pawn relationship chances.</summary>
        /// <param name="pawn">The pawn to include in the relationship calculation as the extra relation chance factor.</param>
        /// <returns>The <see cref="Builder" /> instance with the specified extra pawn applied for chaining further configuration.</returns>
        public Builder WithExtraPawnForExtraRelationChance(Pawn pawn)
        {
            _extraPawnForExtraRelationChance = pawn;

            return this;
        }

        /// <summary>Sets a fixed factor that determines the chance of generating a relationship with an extra pawn.</summary>
        /// <param name="factor">
        ///     The factor to apply to the chance of generating a relationship with an extra pawn. A higher value
        ///     increases the likelihood.
        /// </param>
        /// <returns>The current <see cref="Builder" /> instance, allowing for method chaining.</returns>
        public Builder WithFixedRelationWithExtraPawnChanceFactor(float factor)
        {
            _relationWithExtraPawnChanceFactor = factor;

            return this;
        }

        /// <summary>Specifies a validation predicate that will be applied to the pawn after gear generation.</summary>
        /// <param name="validator">
        ///     A predicate function that takes a <see cref="Pawn" /> and returns true if the pawn passes the
        ///     validation.
        /// </param>
        /// <returns>
        ///     The <see cref="PawnGenerationRequest.Builder" /> instance, allowing further configuration of the pawn
        ///     generation request.
        /// </returns>
        public Builder WithValidatorPostGear(Predicate<Pawn> validator)
        {
            _validatorPostGear = validator;

            return this;
        }

        /// <summary>Sets a pre-gear validation predicate for the pawn generation process.</summary>
        /// <param name="validator">A predicate function that will be applied to validate the pawn before gear is assigned.</param>
        /// <returns>The current <see cref="PawnGenerationRequest.Builder" /> instance, enabling method chaining.</returns>
        public Builder WithValidatorPreGear(Predicate<Pawn> validator)
        {
            _validatorPreGear = validator;

            return this;
        }

        /// <summary>Specifies a collection of traits that the generated pawn is prohibited from having.</summary>
        /// <param name="traits">The traits that are to be prohibited for the generated pawn.</param>
        /// <returns>The current instance of the builder with the prohibited traits applied.</returns>
        public Builder WithProhibitedTraits(params TraitDef[] traits)
        {
            _prohibitedTraits = traits;

            return this;
        }

        /// <summary>Specifies a set of traits that must be assigned to the generated pawn.</summary>
        /// <param name="traits">
        ///     An array of <see cref="TraitDef" /> objects representing the traits to forcibly assign to the
        ///     pawn.
        /// </param>
        /// <returns>The current instance of <see cref="Builder" /> to allow method chaining.</returns>
        public Builder WithForcedTraits(params TraitDef[] traits)
        {
            _forcedTraits = traits;

            return this;
        }

        /// <summary>Sets the faction for the pawn being generated within the builder.</summary>
        /// <param name="faction">The faction to assign to the pawn.</param>
        /// <returns>The current builder instance with the specified faction applied.</returns>
        public Builder WithFaction(Faction faction)
        {
            _faction = faction;

            return this;
        }

        /// <summary>Sets a fixed minimum chance for redressing a world pawn during generation.</summary>
        /// <param name="chance">The fixed minimum chance value to apply when redressing a world pawn.</param>
        /// <returns>
        ///     Returns the current <see cref="PawnGenerationRequest.Builder" /> instance with the specified fixed minimum
        ///     chance to redress a world pawn applied to the configuration.
        /// </returns>
        public Builder WithFixedMinimumChanceToRedressWorldPawn(float chance)
        {
            _minChanceToRedressWorldPawn = chance;

            return this;
        }

        /// <summary>Sets the fixed gender for the pawn being generated in the request builder.</summary>
        /// <param name="gender">The specific gender to assign to the generated pawn.</param>
        /// <returns>The <see cref="Builder" /> instance, allowing for further chaining of builder methods.</returns>
        public Builder WithFixedGender(Gender gender)
        {
            _fixedGender = gender;

            return this;
        }

        /// <summary>Specifies a fixed last name for the pawn being generated.</summary>
        /// <param name="name">The fixed last name to assign to the generated pawn.</param>
        /// <returns>A <see cref="PawnGenerationRequest.Builder" /> instance with the fixed last name applied.</returns>
        public Builder WithFixedLastName(string name)
        {
            _fixedLastName = name;

            return this;
        }

        /// <summary>Specifies a fixed birth name for the pawn being created.</summary>
        /// <param name="name">The birth name to assign to the pawn.</param>
        /// <returns>The modified <see cref="Builder" /> instance with the fixed birth name set.</returns>
        public Builder WithFixedBirthName(string name)
        {
            _fixedBirthName = name;

            return this;
        }

        /// <summary>Sets a fixed royal title for the pawn being generated in the request.</summary>
        /// <param name="title">The fixed royal title to assign to the generated pawn.</param>
        /// <returns>A <see cref="Builder" /> instance with the specified fixed royal title applied.</returns>
        public Builder WithFixedTitle(RoyalTitleDef title)
        {
            _fixedTitle = title;

            return this;
        }

        /// <summary>Specifies a custom xenotype to be forcibly applied to the generated pawn.</summary>
        /// <param name="customXenotype">The custom xenotype definition to assign to the generated pawn.</param>
        /// <returns>The current <see cref="PawnGenerationRequest.Builder" /> instance with the specified custom xenotype applied.</returns>
        public Builder WithForcedCustomXenotype(CustomXenotype customXenotype)
        {
            _forcedCustomXenotype = customXenotype;

            return this;
        }

        /// <summary>Specifies whether the pawn generation process should strictly adhere to using only forced backstories.</summary>
        /// <param name="onlyUseForcedBackstories">
        ///     A boolean value indicating whether to enforce the usage of only forced
        ///     backstories during pawn generation.
        /// </param>
        /// <returns>The builder instance with the updated configuration for forced backstories.</returns>
        public Builder OnlyUseForcedBackstories(bool onlyUseForcedBackstories = true)
        {
            _onlyUseForcedBackstories = onlyUseForcedBackstories;

            return this;
        }

        /// <summary>Sets the minimum age required for traits to be applied to the generated pawn.</summary>
        /// <param name="minimumAgeTraits">The minimum age at which traits can be assigned to the pawn.</param>
        /// <returns>The current instance of the builder with the specified minimum age for traits.</returns>
        public Builder WithMinimumAgeTraits(int minimumAgeTraits)
        {
            _minimumAgeTraits = minimumAgeTraits;

            return this;
        }

        /// <summary>Sets the maximum allowable age for traits when generating a pawn.</summary>
        /// <param name="maximumAgeTraits">
        ///     The maximum age of traits to be considered during pawn generation. A value of -1
        ///     indicates no maximum age restriction.
        /// </param>
        /// <returns>The current <see cref="Builder" /> instance for method chaining.</returns>
        public Builder WithMaximumAgeTraits(int maximumAgeTraits)
        {
            _maximumAgeTraits = maximumAgeTraits;

            return this;
        }

        /// <summary>Forces the generated pawn to have only the specified genes.</summary>
        /// <param name="genes">The genes that the generated pawn will exclusively have.</param>
        /// <returns>The current builder instance with the specified forced genes applied.</returns>
        public Builder ForceOnlyGenes(params GeneDef[] genes)
        {
            _forcedGenes = genes;

            return this;
        }

        /// <summary>Sets whether the generated pawn should be forced to spawn dead.</summary>
        /// <param name="forceDead">A boolean value indicating if the pawn should be forcibly marked as dead. Defaults to true.</param>
        /// <returns>The current instance of the <see cref="Builder" />, allowing for method chaining.</returns>
        public Builder ForceDead(bool forceDead = true)
        {
            _forceDead = forceDead;

            return this;
        }

        /// <summary>Sets whether any title is forbidden for the pawn being generated in the request.</summary>
        /// <param name="forbidAnyTitle">
        ///     A boolean value indicating whether to forbid any title for the generated pawn. When set to
        ///     true, no title will be assigned to the pawn.
        /// </param>
        /// <returns>The current instance of the <see cref="PawnGenerationRequest.Builder" />, allowing for method chaining.</returns>
        public Builder ForbidAnyTitle(bool forbidAnyTitle = true)
        {
            _forbidAnyTitle = forbidAnyTitle;

            return this;
        }

        /// <summary>Forces the generated pawn to not have a backstory assigned.</summary>
        /// <param name="noBackstory">Indicates whether the generated pawn should have no backstory. Default is true.</param>
        /// <returns>An updated instance of the builder with the specified backstory constraint applied.</returns>
        public Builder ForceNoBackstory(bool noBackstory = true)
        {
            _forceNoBackstory = noBackstory;

            return this;
        }

        /// <summary>Specifies a set of forced endogenes to apply exclusively to the pawn being generated.</summary>
        /// <param name="genes">The collection of gene definitions that will be exclusively forced as endogenes for the pawn.</param>
        /// <returns>The builder instance with the specified forced endogenes applied, allowing for further configuration.</returns>
        public Builder WithOnlyForcedEndogenes(params GeneDef[] genes)
        {
            _forcedEndogenes = genes;

            return this;
        }

        /// <summary>Restricts the pawn generation request to include only the specified xenotypes.</summary>
        /// <param name="xenotypes">The list of xenotypes that are allowed for the pawn generation request.</param>
        /// <returns>A <see cref="PawnGenerationRequest.Builder" /> instance with the allowed xenotypes restriction applied.</returns>
        public Builder WithOnlyAllowedXenotypes(params XenotypeDef[] xenotypes)
        {
            _allowedXenotypes = xenotypes;

            return this;
        }

        /// <summary>Sets the forced xenotype for the pawn generation request.</summary>
        /// <param name="xenotype">The specific <see cref="XenotypeDef" /> to assign as the forced xenotype for the generated pawn.</param>
        /// <returns>
        ///     This <see cref="PawnGenerationRequest.Builder" /> instance, allowing for method chaining and further
        ///     configuration.
        /// </returns>
        public Builder WithForcedXenotype(XenotypeDef xenotype)
        {
            _forcedXenotype = xenotype;

            return this;
        }

        /// <summary>Specifies a function to determine the <see cref="PawnKindDef" /> based on a given <see cref="XenotypeDef" />.</summary>
        /// <param name="getter">
        ///     A function that takes a <see cref="XenotypeDef" /> as input and returns the corresponding
        ///     <see cref="PawnKindDef" />.
        /// </param>
        /// <returns>The current instance of the builder, with the specified <paramref name="getter" /> assigned.</returns>
        public Builder WithPawnKindDefGetter(Func<XenotypeDef, PawnKindDef> getter)
        {
            _pawnKindDefGetter = getter;

            return this;
        }

        /// <summary>Sets the chance to force the pawn to be a baseliner.</summary>
        /// <param name="chance">
        ///     A <see cref="float" /> representing the probability (from 0 to 1) of forcing the pawn to be a
        ///     baseliner. A value of 0 means no chance, while 1 means certainty.
        /// </param>
        /// <returns>The updated <see cref="Builder" /> instance with the specified force baseliner chance applied.</returns>
        public Builder WithForcedBaselinerChance(float chance)
        {
            _forceBaselinerChance = chance;

            return this;
        }

        /// <summary>Forces the generated pawn to be recruitable, overriding default behavior.</summary>
        /// <param name="recruitable">If set to true, ensures the pawn is forcibly marked as recruitable. Defaults to true.</param>
        /// <returns>The current <see cref="Builder" /> instance with the force recruitable setting applied.</returns>
        public Builder ForceRecruitable(bool recruitable = true)
        {
            _forceRecruitable = recruitable;

            return this;
        }

        /// <summary>Sets whether the generated pawn should have no gear assigned.</summary>
        /// <param name="noGear">A boolean value indicating whether to forcibly generate the pawn without gear. Defaults to true.</param>
        /// <returns>The current <see cref="Builder" /> instance with the updated gear setting.</returns>
        public Builder ForceNoGear(bool noGear = true)
        {
            _forceNoGear = noGear;

            return this;
        }

        /// <summary>Specifies whether or not to prevent a pawn from being assigned a weapon during generation.</summary>
        /// <param name="dontGiveWeapon">
        ///     A boolean value indicating whether the pawn should not be given a weapon (true) or allow
        ///     weapon assignment (false).
        /// </param>
        /// <returns>The current instance of the <see cref="Builder" /> with the updated setting for weapon assignment.</returns>
        public Builder DontGiveWeapon(bool dontGiveWeapon = true)
        {
            _dontGiveWeapon = dontGiveWeapon;

            return this;
        }

        /// <summary>Configures the pawn generation context for the current builder instance.</summary>
        /// <param name="context">
        ///     The context to set for pawn generation. This cannot be <see cref="PawnGenerationContext.All" />,
        ///     and an exception will be thrown if such context is provided.
        /// </param>
        /// <returns>The current instance of the builder with the specified pawn generation context applied.</returns>
        /// <exception cref="ArgumentException">
        ///     Thrown when the specified <paramref name="context" /> is
        ///     <see cref="PawnGenerationContext.All" />.
        /// </exception>
        public Builder WithGenerationContext(PawnGenerationContext context)
        {
            if (context is PawnGenerationContext.All) throw new ArgumentException($"Cannot use {nameof(PawnGenerationContext.All)} context for generation.", nameof(context));

            _context = context;

            return this;
        }

        /// <summary>Configures the pawn generation request to create an inhabitant associated with a specified world tile.</summary>
        /// <param name="tile">The tile ID corresponding to a valid map where the pawn will be considered an inhabitant.</param>
        /// <returns>The same <see cref="Builder" /> instance, configured to generate an inhabitant for the specified tile.</returns>
        /// <exception cref="ArgumentException">Thrown when the provided tile ID is -1 or does not correspond to a valid map.</exception>
        public Builder AsInhabitant(int tile)
        {
            if (tile == -1 || Current.Game.FindMap(tile) == null)
                throw new ArgumentException(message: "Cannot generate an inhabitant with a tile that doesn't correspond to a map.", nameof(tile));

            _tile = tile;
            _inhabitant = true;

            return this;
        }

        /// <summary>Specifies an ideology for the pawn being generated.</summary>
        /// <param name="ideology">The ideology to assign to the generated pawn.</param>
        /// <returns>The current <see cref="Builder" /> instance with the specified ideology set.</returns>
        public Builder WithIdeology(Ideo ideology)
        {
            _fixedIdeo = ideology;
            _forceNoIdeo = false;

            return this;
        }

        /// <summary>
        ///     Removes any ideology constraints from the pawn generation request and ensures no ideology is assigned to the
        ///     pawn.
        /// </summary>
        /// <returns>
        ///     The <see cref="PawnGenerationRequest.Builder" /> instance with ideology settings removed, allowing chaining of
        ///     further configurations.
        /// </returns>
        public Builder WithNoIdeology()
        {
            _fixedIdeo = null;
            _forceNoIdeo = true;

            return this;
        }

        /// <summary>Sets the developmental stage for the pawn being generated.</summary>
        /// <param name="stage">The developmental stage to assign to the generated pawn.</param>
        /// <returns>The builder instance with the updated developmental stage.</returns>
        public Builder AtDevelopmentStage(DevelopmentalStage stage)
        {
            _developmentalStage = stage;
            _fixedIdeo = stage is DevelopmentalStage.Newborn or DevelopmentalStage.Baby ? null : _fixedIdeo;

            return this;
        }

        /// <summary>Allows the generation of a downed pawn during the pawn creation process.</summary>
        /// <returns>The current <see cref="PawnGenerationRequest.Builder" /> instance, enabling method chaining.</returns>
        public Builder AllowDowned()
        {
            _allowDowned = true;

            return this;
        }

        /// <summary>Prevents the generated pawn from being in a downed state by disabling the allowance of downed pawns.</summary>
        /// <returns>The current instance of the <see cref="Builder" />, with modifications applied to disallow downed pawns.</returns>
        public Builder PreventDowned()
        {
            _allowDowned = false;

            if ((_developmentalStage & DevelopmentalStage.Newborn) != 0) _developmentalStage = DevelopmentalStage.Baby;

            if (KindDef.RaceProps.lifeStageAges.Any(s => s.def.alwaysDowned))
            {
                // FIXME: Either log this reversion, or throw an exception.
                _allowDowned = true;
            }

            return this;
        }

        /// <summary>Specifies whether dead pawns are allowed to be generated in the pawn generation request.</summary>
        /// <param name="allowDead">
        ///     A boolean value indicating whether dead pawns are allowed. Pass true to allow dead pawns, or
        ///     false to disallow them.
        /// </param>
        /// <returns>
        ///     The builder instance with the updated state, allowing method chaining to configure additional options for the
        ///     pawn generation request.
        /// </returns>
        public Builder AllowDead(bool allowDead = true)
        {
            _allowDead = allowDead;

            return this;
        }

        /// <summary>Sets the fixed biological age for the pawn being generated, overriding any existing age ranges or exclusions.</summary>
        /// <param name="age">The exact biological age to set for the pawn.</param>
        /// <returns>
        ///     The builder instance with the specified biological age applied, allowing further customization of the pawn
        ///     generation request.
        /// </returns>
        public Builder WithBiologicalAge(float age)
        {
            _fixedBiologicalAge = age;
            _biologicalAgeRange = null;
            _excludeBiologicalAgeRange = null;

            return this;
        }

        /// <summary>Specifies the range of biological age for pawns during generation.</summary>
        /// <param name="range">
        ///     A <see cref="FloatRange" /> defining the minimum and maximum biological age allowed for generated
        ///     pawns.
        /// </param>
        /// <returns>The <see cref="Builder" /> instance with the specified biological age range applied.</returns>
        public Builder WithBiologicalAgeRange(FloatRange range)
        {
            _biologicalAgeRange = range;
            _fixedBiologicalAge = null;
            _excludeBiologicalAgeRange = null;

            return this;
        }

        /// <summary>Sets the range of biological age to exclude when generating a pawn.</summary>
        /// <param name="range">The range of biological ages to exclude.</param>
        /// <returns>The builder instance with the updated excluded biological age range.</returns>
        public Builder ExcludeBiologicalAgeRange(FloatRange range)
        {
            _excludeBiologicalAgeRange = range;
            _biologicalAgeRange = null;
            _fixedBiologicalAge = null;

            return this;
        }

        /// <summary>Sets a fixed chronological age for the pawn being generated.</summary>
        /// <param name="age">The specific fixed chronological age to assign to the pawn.</param>
        /// <returns>The current <see cref="Builder" /> instance, allowing for method chaining.</returns>
        public Builder WithChronologicalAge(float age)
        {
            _fixedChronologicalAge = age;

            return this;
        }

        /// <summary>Constructs a configured <see cref="PawnGenerationRequest" /> instance using the specified parameters.</summary>
        /// <returns>A fully built <see cref="PawnGenerationRequest" /> instance containing the configurations set in the builder.</returns>
        public PawnGenerationRequest Build()
        {
            if ((_developmentalStage & DevelopmentalStage.Newborn) != 0 && (_developmentalStage & DevelopmentalStage.Newborn) == 0)
                throw new InvalidOperationException("Cannot generate a pawn with multiple developmental stages simultaneously.");

            return new PawnGenerationRequest(
                kind,
                _faction,
                _context,
                _tile,
                _forceGenerateNewPawn,
                _allowDead,
                _allowDowned,
                _canGeneratePawnRelations,
                _mustBeCapableOfViolence,
                _colonistRelationChanceFactor,
                _forceAddFreeWarmLayerIfNeeded,
                _allowGay,
                _allowPregnant,
                _allowFood,
                _allowAddictions,
                _inhabitant,
                _certainlyBeenInCryptosleep,
                _forceRedressWorldPawnIfFormerColonist,
                _worldPawnFactionDoesntMatter,
                _biocodeWeaponChance,
                _biocodeApparelChance,
                _extraPawnForExtraRelationChance,
                _relationWithExtraPawnChanceFactor,
                _validatorPreGear,
                _validatorPostGear,
                _forcedTraits,
                _prohibitedTraits,
                _minChanceToRedressWorldPawn,
                _fixedBiologicalAge,
                _fixedChronologicalAge,
                _fixedGender,
                _fixedLastName,
                _fixedBirthName,
                _fixedTitle,
                _fixedIdeo,
                _forceNoIdeo,
                _forceNoBackstory,
                _forbidAnyTitle,
                _forceDead,
                _forcedGenes,
                _forcedEndogenes,
                _forcedXenotype,
                _forcedCustomXenotype,
                _allowedXenotypes,
                _forceBaselinerChance,
                _developmentalStage,
                _pawnKindDefGetter,
                _excludeBiologicalAgeRange,
                _biologicalAgeRange,
                _forceRecruitable,
                _dontGiveWeapon,
                _onlyUseForcedBackstories,
                _maximumAgeTraits,
                _minimumAgeTraits,
                _forceNoGear,
                KindDef is CreepJoinerFormKindDef,
                _redressValidator
            );
        }
    }
}
