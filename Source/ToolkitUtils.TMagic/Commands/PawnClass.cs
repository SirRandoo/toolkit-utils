using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.CommonLib.Helpers;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TorannMagic;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.TMagic.Commands;

[UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
public class PawnClass : CommandBase
{
    public override void RunCommand(ITwitchMessage twitchMessage)
    {
        if (!PurchaseHelper.TryGetPawn(twitchMessage.Username, out Pawn pawn))
        {
            twitchMessage.Reply("TKUtils.NoPawn".Localize());

            return;
        }

        var magic = pawn.TryGetComp<CompAbilityUserMagic>();
        var might = pawn.TryGetComp<CompAbilityUserMight>();
        bool isMightUser = might is { IsMightUser: true };
        bool isMagicUser = magic is { IsMagicUser: true };

        if (!isMightUser && !isMagicUser)
        {
            twitchMessage.Reply("TKUtils.PawnClass.None".Localize());

            return;
        }

        var container = new List<string>();

        if (isMightUser)
        {
            container.Add(ExtractMightData(might));
        }

        if (isMagicUser)
        {
            container.Add(ExtractMagicData(magic));
        }

        string className = ExtractClassName(pawn);
        string joined = container.GroupedJoin();

        twitchMessage.Reply(className.NullOrEmpty() ? joined : joined.WithHeader(className));
    }

    private string ExtractMightData(CompAbilityUserMight might)
    {
        var builder = new StringBuilder();

        builder.Append(TkSettings.Emojis ? ResponseHelper.DaggerGlyph : "TKUtils.PawnClass.Might".Localize());
        builder.Append(" ");
        builder.Append("TKUtils.PawnClass.Level".LocalizeKeyed(might.MightUserLevel.ToString("N0")));
        builder.Append(", ");
        builder.Append("TKUtils.PawnClass.Experience".LocalizeKeyed(might.MightUserXP.ToString("N0"), might.MightUserXPTillNextLevel.ToString("N0")));
        builder.Append(", ");
        builder.Append("TKUtils.PawnClass.Stamina".Localize()).Append(" ");
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

        if (might.MightData.MightAbilityPoints <= 0)
        {
            return builder.ToString();
        }

        builder.Append(", ");
        builder.Append("TKUtils.PawnClass.Points".LocalizeKeyed(might.MightData.MightAbilityPoints.ToString("N0")));

        return builder.ToString();
    }

    private string ExtractMagicData(CompAbilityUserMagic magic)
    {
        var builder = new StringBuilder();

        builder.Append(TkSettings.Emojis ? ResponseHelper.MagicGlyph : "TKUtils.PawnClass.Magic".Localize());
        builder.Append(" ");
        builder.Append("TKUtils.PawnClass.Level".LocalizeKeyed(magic.MagicUserLevel.ToString("N0")));
        builder.Append(", ");
        builder.Append("TKUtils.PawnClass.Experience".LocalizeKeyed(magic.MagicUserXP.ToString("N0"), magic.MagicUserXPTillNextLevel.ToString("N0")));
        builder.Append(", ");
        builder.Append("TKUtils.PawnClass.Mana".Localize()).Append(" ");
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


        if (magic.MagicData.MagicAbilityPoints <= 0)
        {
            return builder.ToString();
        }

        builder.Append(", ");
        builder.Append("TKUtils.PawnClass.Points".LocalizeKeyed(magic.MagicData.MagicAbilityPoints.ToString("N0")));

        return builder.ToString();
    }

    private string ExtractClassName(Pawn pawn)
    {
        TraitDef classTrait = TM_Data.AllClassTraits.Find(t => pawn.story.traits.HasTrait(t));

        if (classTrait is not null)
        {
            return GetClassName(classTrait);
        }

        return pawn.story.traits.HasTrait(TorannMagicDefOf.DeathKnight) ? GetClassName(TorannMagicDefOf.DeathKnight) : "";
    }

    private string GetClassName(TraitDef trait) => RichTextHelper.StripTags(trait.degreeDatas.Count > 0 ? trait.degreeDatas[0].label : trait.label);
}
