using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TorannMagic;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.Commands
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature, ImplicitUseTargetFlags.WithMembers)]
    public class PawnClass : CommandBase
    {
        public override void RunCommand([NotNull] ITwitchMessage twitchMessage)
        {
            if (!PurchaseHelper.TryGetPawn(twitchMessage.Username, out Pawn pawn))
            {
                twitchMessage.Reply("TKUtils.NoPawn".Localize());
                return;
            }

            var magic = pawn.TryGetComp<CompAbilityUserMagic>();
            var might = pawn.TryGetComp<CompAbilityUserMight>();
            bool isMightUser = might?.IsMightUser == true;
            bool isMagicUser = magic?.IsMagicUser == true;

            if (!isMightUser && !isMagicUser)
            {
                twitchMessage.Reply("TKUtils.PawnClass.None".Localize());
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

            builder.Append(TkSettings.Emojis ? ResponseHelper.DaggerGlyph : "TKUtils.PawnClass.Might".Localize());
            builder.Append(" ");
            builder.Append("TKUtils.PawnClass.Level".LocalizeKeyed(might.MightUserLevel.ToString("N0")));
            builder.Append(", ");
            builder.Append(
                "TKUtils.PawnClass.Experience".LocalizeKeyed(
                    might.MightUserXP.ToString("N0"),
                    might.MightUserXPTillNextLevel.ToString("N0")
                )
            );
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

        [NotNull]
        private string ExtractMagicData(Pawn pawn, [NotNull] CompAbilityUserMagic magic)
        {
            var builder = new StringBuilder();

            builder.Append(TkSettings.Emojis ? ResponseHelper.MagicGlyph : "TKUtils.PawnClass.Magic".Localize());
            builder.Append(" ");
            builder.Append("TKUtils.PawnClass.Level".LocalizeKeyed(magic.MagicUserLevel.ToString("N0")));
            builder.Append(", ");
            builder.Append(
                "TKUtils.PawnClass.Experience".LocalizeKeyed(
                    magic.MagicUserXP.ToString("N0"),
                    magic.MagicUserXPTillNextLevel.ToString("N0")
                )
            );
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

        [CanBeNull]
        private string ExtractClassName(Pawn pawn)
        {
            foreach (TraitDef trait in TM_Data.AllClassTraits.Where(trait => pawn.story.traits.HasTrait(trait)))
            {
                return GetClassName(trait);
            }

            return pawn.story.traits.HasTrait(TorannMagicDefOf.DeathKnight)
                ? GetClassName(TorannMagicDefOf.DeathKnight)
                : "";
        }

        [CanBeNull]
        private string GetClassName([NotNull] TraitDef trait)
        {
            return Unrichify.StripTags(trait.degreeDatas.Count > 0 ? trait.degreeDatas.First().label : trait.label);
        }
    }
}
