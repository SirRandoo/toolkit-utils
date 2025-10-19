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
// with ToolkitUtils.TMagic. If not, see <https://www.gnu.org/licenses/>.
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using RimWorld;
using ToolkitUtils.Api;
using ToolkitUtils.Api.Wrappers;
using ToolkitUtils.Core;
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Extensions;
using ToolkitUtils.Mod.Localization;
using ToolkitUtils.Mod.Presentation;
using ToolkitUtils.TMagic.Extensions;
using TorannMagic;
using Verse;

namespace ToolkitUtils.TMagic.Commands;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public sealed class PawnClass(ExecutionContext context, TranslationService service) : CommandGroup
{
    /// <summary>
    ///     Asynchronously retrieves information about a pawn's class for the current invoker and sends a formatted reply
    ///     to the context. This includes data about the pawn's magic or might abilities, or both, if applicable. If the
    ///     invoker is not associated with a pawn, an appropriate error message is sent to the context, and the operation
    ///     fails. If neither a magic nor might class is found, a message indicating no class is sent to the context.
    /// </summary>
    /// <return>Returns a result indicating the success or failure of the operation.</return>
    [Command("mypawnclass")]
    public async Task<Result> GetPawnClassInformationAsync()
    {
        Pawn? pawn = ViewerPawnRegistry.Get(context.Invoker.Id);

        if (pawn == null) return Result.Fail(service.GetPawnRequiredError(context.Invoker));

        var mightComp = pawn.TryGetComp<CompAbilityUserMight>();
        var magicComp = pawn.TryGetComp<CompAbilityUserMagic>();

        bool isMightUser = mightComp is not { IsMightUser: true, };
        bool isMagicUser = magicComp is not { IsMagicUser: true, };

        if (isMightUser && isMagicUser) return Result.Fail(service.GetNoClassError());

        var container = new string[2];

        if (isMagicUser) container[0] = await MainThreadExtensions.OnMainAsync(ExtractMagicData, pawn, magicComp);
        if (isMightUser) container[1] = await MainThreadExtensions.OnMainAsync(ExtractMightData, pawn, mightComp);

        string className = ExtractClassName(pawn);
        string joined = string.Join(separator: " | ", container);

        return await context.SendReplyAsync(string.IsNullOrEmpty(className) ? joined : $"[{className}] {joined}");
    }

    private string ExtractMightData(Pawn pawn, CompAbilityUserMight might)
    {
        var builder = new StringBuilder();

        builder.Append(UnicodeCharacter.Dagger.Codepoint);
        builder.Append(" ");
        builder.Append(string.Format(format: "{0}{1:N0}", service.GetTranslation("TM_MCU_CurrentLevel"), might.MightUserLevel));
        builder.Append(", ");
        builder.Append(
            service.GetTranslation("TKUtils.Responses.TorannClassExperience").Format(
                new
                {
                    CurrentLevelExperience = might.MightUserXP.ToString("N0"), LevelExperienceCap = might.MightUserXPTillNextLevel.ToString("N0"),
                }
            )
        );
        builder.Append(", ");
        builder.Append(pawn.needs.TryGetNeed<Need_Stamina>().LabelCap).Append(" ");
        builder.Append((might.Stamina.CurLevel * 100f).ToString("N0"));
        builder.Append("/");
        builder.Append((might.Stamina.MaxLevel * 100f).ToString("N0"));

        if (might.Stamina.lastGainPct != 0)
        {
            builder.Append(" ");
            builder.Append(might.Stamina.lastGainPct < 0 ? "-" : "+");
            builder.Append(might.Stamina.lastGainPct.ToString("N3"));
            builder.Append("/SP");
        }

        if (might.MightData.MightAbilityPoints <= 0) return builder.ToString();

        builder.Append(", ");
        builder.Append(string.Format(format: "{0:N0} {1}", might.MightData.MightAbilityPoints, service.GetTranslation("TM_PointsAvail")));

        return builder.ToString();
    }

    private string ExtractMagicData(Pawn pawn, CompAbilityUserMagic magic)
    {
        var builder = new StringBuilder();

        builder.Append(UnicodeCharacter.CrystalBall.Codepoint);
        builder.Append(" ");
        builder.Append(string.Format(format: "{0}{1:N0}", service.GetTranslation("TM_MCU_CurrentLevel"), magic.MagicUserLevel));
        builder.Append(", ");

        builder.Append(
            service.GetTranslation("TKUtils.Responses.TorannClassExperience").Format(
                new
                {
                    CurrentLevelExperience = magic.MagicUserXP.ToString("N0"), LevelExperienceCap = magic.MagicUserXPTillNextLevel.ToString("N0"),
                }
            )
        );

        builder.Append(", ");
        builder.Append(pawn.needs.TryGetNeed<Need_Mana>().LabelCap).Append(" ");
        builder.Append((magic.Mana.CurLevel * 100f).ToString("N0"));
        builder.Append("/");
        builder.Append((magic.Mana.MaxLevel * 100f).ToString("N0"));

        if (magic.Mana.lastGainPct != 0)
        {
            builder.Append(" ");
            builder.Append(magic.Mana.lastGainPct < 0 ? "-" : "+");
            builder.Append(magic.Mana.lastGainPct.ToString("N3"));
            builder.Append("/MP");
        }


        if (magic.MagicData.MagicAbilityPoints <= 0) return builder.ToString();

        builder.Append(", ");
        builder.Append(string.Format(format: "{0:N0} {1}", magic.MagicData.MagicAbilityPoints, service.GetTranslation("TM_PointsAvail")));

        return builder.ToString();
    }

    private static string ExtractClassName(Pawn pawn)
    {
        TraitDef classTrait = TM_Data.AllClassTraits.Find(t => pawn.story.traits.HasTrait(t));

        if (classTrait != null) return GetClassName(classTrait);

        return pawn.story.traits.HasTrait(TorannMagicDefOf.DeathKnight) ? GetClassName(TorannMagicDefOf.DeathKnight) : "";
    }

    private static string GetClassName(TraitDef trait) => RichTextHelper.StripTags(trait.degreeDatas.Count > 0 ? trait.degreeDatas[0].label : trait.label);
}
