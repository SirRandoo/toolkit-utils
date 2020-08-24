using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Windows;
using TwitchToolkit.Settings;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    [UsedImplicitly]
    public class TkUtils : Mod
    {
        public const string Id = "ToolkitUtils";

        public TkUtils(ModContentPack content) : base(content)
        {
            GetSettings<TkSettings>();
            Settings_ToolkitExtensions.RegisterExtension(new ToolkitExtension(this, typeof(TkUtilsWindow)));
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            TkSettings.DoWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return Id;
        }
    }
}
