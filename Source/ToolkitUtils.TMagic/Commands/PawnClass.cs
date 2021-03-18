using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Utils;
using TorannMagic;
using TwitchLib.Client.Models.Interfaces;
using Verse;

namespace SirRandoo.ToolkitUtils.TMagic.Commands
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
            bool isMightUser = might?.IsMightUser ?? false;
            bool isMagicUser = magic?.IsMagicUser ?? false;

            if (!isMightUser && !isMagicUser)
            {
                twitchMessage.Reply("TKUtils.PawnClass.None".Localize());
                return;
            }

            var container = new List<string>();

            if (isMightUser)
            {
                container.Add(ExtractMightData(pawn, might).SectionJoin());
            }

            if (isMagicUser)
            {
                container.Add(ExtractMagicData(pawn, magic).SectionJoin());
            }

            string className = ExtractClassName(pawn);

            if (!className.NullOrEmpty())
            {
                container.Insert(0, className.StripTags().CapitalizeFirst());
            }

            twitchMessage.Reply(container.GroupedJoin());
        }

        private IEnumerable<string> ExtractMightData(Pawn pawn, [NotNull] CompAbilityUserMight might)
        {
            yield return "TKUtils.PawnClass.Level".Localize(might.MightUserLevel.ToString("N0"))
               .WithHeader(ResponseHelper.DaggerGlyph);
            yield return "TKUtils.PawnClass.Experience".Localize(
                $"{might.MightUserXP:N0}/{might.MightUserXPTillNextLevel:N0}"
            );
            yield return
                $"{might.Stamina.CurLevel * 100f:N0}/{might.Stamina.MaxLevel * 100f:N0} {(might.Stamina.modifiedStaminaGain < 0 ? "-" : "+")}SP/{might.Stamina.modifiedStaminaGain:N2}s";

            if (might.MightData.MightAbilityPoints > 0)
            {
                yield return "TKUtils.PawnClass.Points".Localize(might.MightData.MightAbilityPoints.ToString("N0"));
            }
        }

        private IEnumerable<string> ExtractMagicData(Pawn pawn, [NotNull] CompAbilityUserMagic magic)
        {
            yield return "TKUtils.PawnClass.Level".Localize(magic.MagicUserLevel.ToString("N0"))
               .WithHeader(ResponseHelper.MagicGlyph);
            yield return "TKUtils.PawnClass.Experience".Localize(
                $"{magic.MagicUserXP:N0}/{magic.MagicUserXPTillNextLevel:N0}"
            );
            yield return
                $"{magic.Mana.CurLevel * 100f:N0}/{magic.Mana.MaxLevel * 100f:N0} {(magic.Mana.modifiedManaGain < 0 ? "-" : "+")}MP/{magic.Mana.modifiedManaGain:N2}s";

            if (magic.MagicData.MagicAbilityPoints > 0)
            {
                yield return "TKUtils.PawnClass.Points".Localize(magic.MagicData.MagicAbilityPoints.ToString("N0"));
            }
        }

        private string ExtractClassName(Pawn pawn)
        {
            foreach (TraitDef trait in TM_Data.AllClassTraits.Where(t => pawn.story.traits.HasTrait(t)))
            {
                return trait.label;
            }

            return pawn.story.traits.HasTrait(TorannMagicDefOf.DeathKnight) ? TorannMagicDefOf.DeathKnight.label : "";
        }
    }
}
