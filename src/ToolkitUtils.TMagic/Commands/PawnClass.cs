using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.CommonLib.Helpers;
using ToolkitUtils.Helpers;
using ToolkitUtils.Utils;
using TorannMagic;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace ToolkitUtils.TMagic.Commands
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
    public class PawnClass : CommandBase
    {
        public override void RunCommand([NotNull] ITwitchMessage twitchMessage)
        {
            if (!PurchaseHelper.TryGetPawn(twitchMessage.Username, out Pawn pawn))
            {
                twitchMessage.Reply("TKUtils.NoPawn".TranslateSimple());

                return;
            }

            var magic = pawn.TryGetComp<CompAbilityUserMagic>();
            var might = pawn.TryGetComp<CompAbilityUserMight>();
            bool isMightUser = might is { IsMightUser: true };
            bool isMagicUser = magic is { IsMagicUser: true };

            if (!isMightUser && !isMagicUser)
            {
                twitchMessage.Reply("TKUtils.PawnClass.None".TranslateSimple());

                return;
            }

            var container = new List<string>();

            if (isMightUser)
            {
                container.Add(ExtractMightData(pawn, might));
            }

            if (isMagicUser)
            {
                container.Add(ExtractMagicData(pawn, magic));
            }

            string className = ExtractClassName(pawn);
            string joined = container.GroupedJoin();

            twitchMessage.Reply(className.NullOrEmpty() ? joined : joined.WithHeader(className));
        }

        [NotNull]
        private string ExtractMightData(Pawn pawn, [NotNull] CompAbilityUserMight might)
        {
            var builder = new StringBuilder();

            builder.Append(TkSettings.Emojis ? ResponseHelper.DaggerGlyph : "TKUtils.PawnClass.Might".TranslateSimple());
            builder.Append(" ");
            builder.Append("TKUtils.PawnClass.Level".Translate(might.MightUserLevel.ToString("N0")));
            builder.Append(", ");
            builder.Append("TKUtils.PawnClass.Experience".Translate(might.MightUserXP.ToString("N0"), might.MightUserXPTillNextLevel.ToString("N0")));
            builder.Append(", ");
            builder.Append("TKUtils.PawnClass.Stamina".TranslateSimple()).Append(" ");
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
            builder.Append("TKUtils.PawnClass.Points".Translate(might.MightData.MightAbilityPoints.ToString("N0")));

            return builder.ToString();
        }

        [NotNull]
        private string ExtractMagicData(Pawn pawn, [NotNull] CompAbilityUserMagic magic)
        {
            var builder = new StringBuilder();

            builder.Append(TkSettings.Emojis ? ResponseHelper.MagicGlyph : "TKUtils.PawnClass.Magic".TranslateSimple());
            builder.Append(" ");
            builder.Append("TKUtils.PawnClass.Level".Translate(magic.MagicUserLevel.ToString("N0")));
            builder.Append(", ");
            builder.Append("TKUtils.PawnClass.Experience".Translate(magic.MagicUserXP.ToString("N0"), magic.MagicUserXPTillNextLevel.ToString("N0")));
            builder.Append(", ");
            builder.Append("TKUtils.PawnClass.Mana".TranslateSimple()).Append(" ");
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
            builder.Append("TKUtils.PawnClass.Points".Translate(magic.MagicData.MagicAbilityPoints.ToString("N0")));

            return builder.ToString();
        }

        [CanBeNull]
        private string ExtractClassName(Pawn pawn)
        {
            TraitDef classTrait = TM_Data.AllClassTraits.Find(t => pawn.story.traits.HasTrait(t));

            if (classTrait != null)
            {
                return GetClassName(classTrait);
            }

            return pawn.story.traits.HasTrait(TorannMagicDefOf.DeathKnight) ? GetClassName(TorannMagicDefOf.DeathKnight) : "";
        }

        [CanBeNull]
        private string GetClassName([NotNull] TraitDef trait) => RichTextHelper.StripTags(trait.degreeDatas.Count > 0 ? trait.degreeDatas.First().label : trait.label);
    }
}
