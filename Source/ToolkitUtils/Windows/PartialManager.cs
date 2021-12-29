// MIT License
// 
// Copyright (c) 2021 SirRandoo
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Models;
using UnityEngine;
using Utf8Json;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public class PartialManager<T> : Window where T : class, IShopItemBase
    {
        private readonly PartialType filter;
        private readonly Action<PartialData<T>> loadCallback;
        private readonly Action<PartialUgc> saveCallback;
        private readonly InstanceType type;
        private string cancelLabel;
        private bool cancelled = true;
        private string confirmLabel;
        private string deletePartialTooltip;
        private string descriptionLabel;
        private string fileDescription;
        private string fileName;
        private string fileNameLabel;
        private List<FileData<T>> files = new List<FileData<T>>();
        private string indexingLabel;
        private bool isIndexing;
        private string loadPartialTooltip;
        private Vector2 scrollPos = Vector2.zero;
        private FileData<T> selectedFile;

        private PartialManager(Action<PartialData<T>> loadCallback)
        {
            type = InstanceType.Load;
            this.loadCallback = loadCallback;

            filter = GetFilter();
            SetWindowParams();
        }

        private PartialManager(Action<PartialUgc> saveCallback)
        {
            type = InstanceType.Save;
            this.saveCallback = saveCallback;

            filter = GetFilter();
            SetWindowParams();
        }

        public override Vector2 InitialSize
        {
            get
            {
                switch (type)
                {
                    case InstanceType.Load:
                        return new Vector2(350, 500);
                    case InstanceType.Save:
                        return new Vector2(350, 150);
                    default:
                        return new Vector2(100, 100);
                }
            }
        }

        private void SetWindowParams()
        {
            doCloseX = true;
            doCloseButton = false;
            layer = WindowLayer.Dialog;
            closeOnClickedOutside = true;
        }

        private static PartialType GetFilter()
        {
            Type clazz = typeof(T);

            if (clazz == typeof(ItemPartial))
            {
                return PartialType.Items;
            }

            if (clazz == typeof(EventPartial))
            {
                return PartialType.Events;
            }

            if (clazz == typeof(PawnKindItem))
            {
                return PartialType.Pawns;
            }

            return clazz == typeof(TraitItem) ? PartialType.Traits : PartialType.Items;
        }

        [NotNull]
        public static PartialManager<T> CreateLoadInstance(Action<PartialData<T>> callback) => new PartialManager<T>(callback);

        [NotNull]
        public static PartialManager<T> CreateSaveInstance(Action<PartialUgc> callback) => new PartialManager<T>(callback);

        public override void DoWindowContents(Rect canvas)
        {
            GUI.BeginGroup(canvas);

            switch (type)
            {
                case InstanceType.Load:
                    DrawLoadScreen(canvas);

                    break;
                case InstanceType.Save:
                    DrawSaveScreen(canvas);

                    break;
            }

            GUI.EndGroup();
        }

        private void DrawLoadScreen(Rect canvas)
        {
            if (isIndexing)
            {
                SettingsHelper.DrawLabel(canvas, indexingLabel, TextAnchor.MiddleCenter, GameFont.Medium);

                return;
            }

            var listing = new Listing_Standard();
            var viewport = new Rect(0f, 0f, canvas.width - 16f, Text.SmallFontHeight * files.Count);

            GUI.BeginGroup(canvas);
            listing.Begin(canvas);
            Widgets.BeginScrollView(canvas, ref scrollPos, viewport);
            FileData<T> toDelete = null;

            foreach (FileData<T> file in files)
            {
                Rect lineRect = listing.GetRect(Text.SmallFontHeight);

                if (!lineRect.IsRegionVisible(canvas, scrollPos))
                {
                    continue;
                }

                var nameRect = new Rect(lineRect.x, lineRect.y, lineRect.width - lineRect.height * 2f, lineRect.height);
                var loadRect = new Rect(nameRect.x + nameRect.width, nameRect.y, lineRect.height, lineRect.height);
                var deleteRect = new Rect(loadRect.x + loadRect.width, nameRect.y, loadRect.width, loadRect.height);

                SettingsHelper.DrawLabel(nameRect, file.Name);

                if (Widgets.ButtonImage(loadRect, TexCommand.Install))
                {
                    selectedFile = file;
                    cancelled = false;
                    Close();
                }

                if (Widgets.ButtonImage(deleteRect, Widgets.CheckboxOffTex))
                {
                    toDelete = file;
                }

                nameRect.TipRegion(file.Description);
                loadRect.TipRegion(loadPartialTooltip);
                deleteRect.TipRegion(deletePartialTooltip);
            }

            Widgets.EndScrollView();
            listing.End();
            GUI.EndGroup();

            if (toDelete == null)
            {
                return;
            }

            files.Remove(toDelete);

            try
            {
                File.Delete(toDelete.Path);
            }
            catch (Exception e)
            {
                LogHelper.Error($"Couldn't remove the partial file @ {toDelete.Path}", e);
            }
        }

        private void DrawSaveScreen(Rect canvas)
        {
            var listing = new Listing_Standard();

            listing.Begin(canvas);

            (Rect nameLabel, Rect nameField) = listing.GetRectAsForm(0.55f);
            SettingsHelper.DrawLabel(nameLabel, fileNameLabel);

            if (SettingsHelper.DrawTextField(nameField, fileName, out string newFileName))
            {
                fileName = newFileName;
            }

            (Rect descLabel, Rect descField) = listing.GetRectAsForm(0.55f);
            SettingsHelper.DrawLabel(descLabel, descriptionLabel);

            if (SettingsHelper.DrawTextField(descField, fileDescription, out string newFileDesc))
            {
                fileDescription = newFileDesc;
            }

            listing.End();

            (Rect cancel, Rect confirm) = new Rect(0f, canvas.height - Text.SmallFontHeight, canvas.width, Text.SmallFontHeight).ToForm(0.5f);

            if (Widgets.ButtonText(confirm, confirmLabel))
            {
                cancelled = false;
                Close();
            }

            if (Widgets.ButtonText(cancel, cancelLabel))
            {
                Close();
            }
        }

        private static string SanitizeFileName(string name)
        {
            return Path.GetInvalidPathChars().Aggregate(name, (current, c) => current.Replace(c, '_'));
        }

        public override void OnCancelKeyPressed()
        {
            cancelled = true;
            base.OnCancelKeyPressed();
        }

        public override void PostOpen()
        {
            base.PostOpen();
            FetchTranslations();

            Task.Run(
                async () =>
                {
                    isIndexing = true;
                    files = await IndexPartialFiles();
                    files.RemoveAll(i => i.PartialData.PartialType != filter);
                    isIndexing = false;
                }
            );
        }

        private void FetchTranslations()
        {
            fileNameLabel = "TKUtils.Fields.Name".Localize();
            descriptionLabel = "TKUtils.Fields.Description".Localize();
            confirmLabel = "TKUtils.Buttons.Confirm".Localize();
            cancelLabel = "TKUtils.Buttons.Cancel".Localize();
            indexingLabel = "TKUtils.PartialManager.Indexing".Localize();
            loadPartialTooltip = "TKUtils.PartialTooltips.LoadPartial".Localize();
            deletePartialTooltip = "TKUtils.PartialTooltips.DeletePartial".Localize();
        }

        private static async Task<List<FileData<T>>> IndexPartialFiles()
        {
            var container = new List<FileData<T>>();

            foreach (string file in Directory.EnumerateFileSystemEntries(Paths.PartialPath, "*.json", SearchOption.TopDirectoryOnly))
            {
                string path = Path.Combine(Paths.PartialPath, file);

                if (Directory.Exists(path))
                {
                    container.Add(new FileData<T> { Description = path, IsDirectory = true, Extension = "", Name = file });

                    continue;
                }

                using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var data = new FileData<T>
                    {
                        Description = path,
                        IsFile = true,
                        Extension = Path.GetExtension(file),
                        Name = Path.GetFileNameWithoutExtension(file)
                    };

                    try
                    {
                        var partial = await JsonSerializer.DeserializeAsync<PartialData<T>>(stream);

                        data.IsPartial = true;
                        data.Description = partial.Description;
                        data.PartialData = partial;
                    }
                    catch (Exception e)
                    {
                        LogHelper.Error("Could not deserialize partial", e);
                    }

                    container.Add(data);
                }
            }

            return container;
        }

        public override void PostClose()
        {
            if (cancelled)
            {
                base.PostClose();

                return;
            }

            switch (type)
            {
                case InstanceType.Load when loadCallback != null && selectedFile != null:
                    loadCallback(selectedFile.PartialData);

                    break;
                case InstanceType.Save when saveCallback != null:
                    saveCallback(new PartialUgc { Description = fileDescription, Name = SanitizeFileName(fileName) });

                    break;
            }

            base.PostClose();
        }

        private enum InstanceType { Load, Save }

        public class PartialUgc
        {
            private string description;
            private string name;

            [NotNull]
            public string Name
            {
                get => name.NullOrEmpty() ? DefaultFileName : $"{name}.json";
                set => name = value.NullOrEmpty() ? DefaultFileName : value;
            }

            public PartialType Type { get; set; }

            public string Description
            {
                get => description.NullOrEmpty() ? DefaultDescription : description;
                set => description = value.NullOrEmpty() ? DefaultDescription : value;
            }

            [NotNull] private static string DefaultDescription => "No description provided";

            [NotNull]
            private string DefaultFileName =>
                $"{Enum.GetName(typeof(PartialType), Type)}.{DateTime.Now.ToFileTime()}.json";
        }
    }
}
