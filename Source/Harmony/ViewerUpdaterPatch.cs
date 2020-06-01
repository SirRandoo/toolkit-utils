using HarmonyLib;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;
using TwitchToolkit.PawnQueue;
using TwitchToolkit.Twitch;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(ViewerUpdater), "ParseMessage")]
    public class ViewerUpdaterPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(ITwitchMessage twitchMessage)
        {
            if (twitchMessage?.ChatMessage == null)
            {
                return false;
            }

            Viewer viewer = Viewers.GetViewer(twitchMessage.Username);
            var component = Current.Game.GetComponent<GameComponentPawns>();

            ToolkitSettings.ViewerColorCodes[twitchMessage.Username.ToLowerInvariant()] =
                twitchMessage.ChatMessage.ColorHex;

            if (component.HasUserBeenNamed(twitchMessage.Username) && TkSettings.HairColor)
            {
                if (ColorUtility.TryParseHtmlString(twitchMessage.ChatMessage.ColorHex, out Color hairColor))
                {
                    component.PawnAssignedToUser(twitchMessage.Username).story.hairColor = hairColor;
                }
            }

            if (twitchMessage.ChatMessage.IsModerator && !viewer.mod)
            {
                viewer.SetAsModerator();
            }

            if (twitchMessage.ChatMessage.IsSubscriber && !viewer.IsSub)
            {
                viewer.subscriber = true;
            }

            if (!twitchMessage.ChatMessage.IsVip || viewer.IsVIP)
            {
                return false;
            }

            viewer.vip = true;

            return false;
        }
    }
}
