﻿// ToolkitUtils
// Copyright (C) 2021  SirRandoo
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using RimWorld;
using SirRandoo.CommonLib.Helpers;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Models;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    /// <summary>
    ///     A dialog for saving and loading partials -- snapshots of the
    ///     user's store data.
    /// </summary>
    /// <typeparam name="T">The type the partial is storing/loading</typeparam>
    public class PartialManager<T> : Window where T : class, IShopItemBase
    {
        private readonly PartialType _filter;
        private readonly Action<PartialData<T>> _loadCallback;
        private readonly Action<PartialUgc> _saveCallback;
        private readonly InstanceType _type;
        private string _cancelLabel;
        private bool _cancelled = true;
        private string _confirmLabel;
        private string _emptyLabel;
        private string _erroredLabel;
        private bool _errored;
        private string _deletePartialTooltip;
        private string _descriptionLabel;
        private string _fileDescription;
        private int _fileCount;
        private string _fileName;
        private string _fileNameLabel;
        private List<FileData<T>> _files = new List<FileData<T>>();
        private string _indexingLabel;
        private bool _isIndexing;
        private string _loadPartialTooltip;
        private Vector2 _scrollPos = Vector2.zero;
        private FileData<T> _selectedFile;

        private PartialManager(Action<PartialData<T>> loadCallback)
        {
            _type = InstanceType.Load;
            _loadCallback = loadCallback;

            _filter = GetFilter();
            SetWindowParams();
        }

        private PartialManager(Action<PartialUgc> saveCallback)
        {
            _type = InstanceType.Save;
            _saveCallback = saveCallback;

            _filter = GetFilter();
            SetWindowParams();
        }

        /// <inheritdoc cref="Window.InitialSize"/>
        public override Vector2 InitialSize
        {
            get
            {
                switch (_type)
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

        /// <summary>
        ///     Creates a new instance for loading partial data.
        /// </summary>
        /// <param name="callback">An action to call when partial data is loaded</param>
        [NotNull]
        public static PartialManager<T> CreateLoadInstance(Action<PartialData<T>> callback) => new PartialManager<T>(callback);

        /// <summary>
        ///     Creates a new instance for saving partial data.
        /// </summary>
        /// <param name="callback">An action to call when partial data is saved</param>
        [NotNull]
        public static PartialManager<T> CreateSaveInstance(Action<PartialUgc> callback) => new PartialManager<T>(callback);

        /// <inheritdoc cref="Window.DoWindowContents"/>
        public override void DoWindowContents(Rect canvas)
        {
            GUI.BeginGroup(canvas);

            switch (_type)
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
            if (_isIndexing)
            {
                UiHelper.Label(canvas, _indexingLabel, TextAnchor.MiddleCenter, GameFont.Medium);

                return;
            }

            if (_errored)
            {
                UiHelper.Label(canvas, _erroredLabel, TextAnchor.MiddleCenter, GameFont.Medium);

                return;
            }

            if (_fileCount <= 0)
            {
                UiHelper.Label(canvas, _emptyLabel, TextAnchor.MiddleCenter, GameFont.Medium);

                return;
            }

            var listing = new Listing_Standard();
            var viewport = new Rect(0f, 0f, canvas.width - 16f, Text.SmallFontHeight * _files.Count);

            GUI.BeginGroup(canvas);
            listing.Begin(canvas);
            Widgets.BeginScrollView(canvas, ref _scrollPos, viewport);
            FileData<T> toDelete = null;

            foreach (FileData<T> file in _files)
            {
                Rect lineRect = listing.GetRect(Text.SmallFontHeight);

                if (!lineRect.IsVisible(canvas, _scrollPos))
                {
                    continue;
                }

                var nameRect = new Rect(lineRect.x, lineRect.y, lineRect.width - lineRect.height * 2f, lineRect.height);
                var loadRect = new Rect(nameRect.x + nameRect.width, nameRect.y, lineRect.height, lineRect.height);
                var deleteRect = new Rect(loadRect.x + loadRect.width, nameRect.y, loadRect.width, loadRect.height);

                UiHelper.Label(nameRect, file.Name);

                if (Widgets.ButtonImage(loadRect, TexCommand.Install))
                {
                    _selectedFile = file;
                    _cancelled = false;
                    Close();
                }

                if (Widgets.ButtonImage(deleteRect, Widgets.CheckboxOffTex))
                {
                    toDelete = file;
                }

                nameRect.TipRegion(file.Description);
                loadRect.TipRegion(_loadPartialTooltip);
                deleteRect.TipRegion(_deletePartialTooltip);
            }

            Widgets.EndScrollView();
            listing.End();
            GUI.EndGroup();

            if (toDelete == null)
            {
                return;
            }

            _files.Remove(toDelete);

            try
            {
                File.Delete(toDelete.Path);
            }
            catch (Exception e)
            {
                TkUtils.Logger.Error($"Couldn't remove the partial file @ {toDelete.Path}", e);
            }
        }

        private void DrawSaveScreen(Rect canvas)
        {
            var listing = new Listing_Standard();

            listing.Begin(canvas);

            (Rect nameLabel, Rect nameField) = listing.Split(0.55f);
            UiHelper.Label(nameLabel, _fileNameLabel);

            if (UiHelper.TextField(nameField, _fileName, out string newFileName))
            {
                _fileName = newFileName;
            }

            (Rect descLabel, Rect descField) = listing.Split(0.55f);
            UiHelper.Label(descLabel, _descriptionLabel);

            if (UiHelper.TextField(descField, _fileDescription, out string newFileDesc))
            {
                _fileDescription = newFileDesc;
            }

            listing.End();

            (Rect cancel, Rect confirm) = new Rect(0f, canvas.height - Text.SmallFontHeight, canvas.width, Text.SmallFontHeight).Split(0.5f);

            if (Widgets.ButtonText(confirm, _confirmLabel))
            {
                _cancelled = false;
                Close();
            }

            if (Widgets.ButtonText(cancel, _cancelLabel))
            {
                Close();
            }
        }

        private static string SanitizeFileName(string name)
        {
            return Path.GetInvalidPathChars().Aggregate(name, (current, c) => current.Replace(c, '_'));
        }

        /// <inheritdoc cref="Window.OnCancelKeyPressed"/>
        public override void OnCancelKeyPressed()
        {
            _cancelled = true;
            base.OnCancelKeyPressed();
        }

        /// <inheritdoc cref="Window.PostOpen"/>
        public override void PostOpen()
        {
            base.PostOpen();
            FetchTranslations();

            Task.Run(
                async () =>
                {
                    _isIndexing = true;

                    try
                    {
                        _files = await IndexPartialFiles();
                    }
                    catch (Exception e)
                    {
                        TkUtils.Logger.Error($"Could not index partials for {_filter.ToString()}", e);
                        
                        _errored = true;
                    }
                    
                    if (_files != null)
                    {
                        _files.RemoveAll(i => i.PartialData.PartialType != _filter);
                        _fileCount = _files.Count;
                    }
                    
                    _isIndexing = false;
                }
            );
        }

        private void FetchTranslations()
        {
            _fileNameLabel = "TKUtils.Fields.Name".TranslateSimple();
            _descriptionLabel = "TKUtils.Fields.Description".TranslateSimple();
            _confirmLabel = "TKUtils.Buttons.Confirm".TranslateSimple();
            _cancelLabel = "TKUtils.Buttons.Cancel".TranslateSimple();
            _emptyLabel = "TKUtils.PartialManager.Empty".TranslateSimple();
            _erroredLabel = "TKUtils.PartialManager.Errored".TranslateSimple();
            _indexingLabel = "TKUtils.PartialManager.Indexing".TranslateSimple();
            _loadPartialTooltip = "TKUtils.PartialTooltips.LoadPartial".TranslateSimple();
            _deletePartialTooltip = "TKUtils.PartialTooltips.DeletePartial".TranslateSimple();
        }

        [ItemNotNull]
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
                    var data = new FileData<T> { Description = path, IsFile = true, Extension = Path.GetExtension(file), Name = Path.GetFileNameWithoutExtension(file) };

                    try
                    {
                        var partial = await Json.DeserializeAsync<PartialData<T>>(stream);

                        if (partial == null)
                        {
                            TkUtils.Logger.Error(@$"Could not deserialize partial at ""{path}""; is it malformed?");

                            continue;
                        }

                        data.IsPartial = true;
                        data.Description = partial.Description;
                        data.PartialData = partial;
                    }
                    catch (Exception e)
                    {
                        TkUtils.Logger.Error("Could not deserialize partial", e);
                    }

                    container.Add(data);
                }
            }

            return container;
        }

        /// <inheritdoc cref="Window.PostClose"/>
        public override void PostClose()
        {
            if (_cancelled)
            {
                base.PostClose();

                return;
            }

            switch (_type)
            {
                case InstanceType.Load when _loadCallback != null && _selectedFile != null:
                    _loadCallback(_selectedFile.PartialData);

                    break;
                case InstanceType.Save when _saveCallback != null:
                    _saveCallback(new PartialUgc { Description = _fileDescription, Name = SanitizeFileName(_fileName) });

                    break;
            }

            base.PostClose();
        }

        private enum InstanceType { Load, Save }

        /// <summary>
        ///     A manifest-like dataclass representing the data stored within a
        ///     partial.
        /// </summary>
        public class PartialUgc
        {
            private string _description;
            private string _name;

            /// <summary>
            ///     The name the user gave the partial.
            /// </summary>
            [NotNull]
            public string Name
            {
                get => _name.NullOrEmpty() ? DefaultFileName : $"{_name}.json";
                set => _name = value.NullOrEmpty() ? DefaultFileName : value;
            }

            /// <summary>
            ///     The type of data this manifest describes.
            /// </summary>
            public PartialType Type { get; set; }

            /// <summary>
            ///     The description the user gave the partial.
            /// </summary>
            public string Description
            {
                get => _description.NullOrEmpty() ? DefaultDescription : _description;
                set => _description = value.NullOrEmpty() ? DefaultDescription : value;
            }

            [NotNull] private static string DefaultDescription => "No description provided";

            [NotNull] private string DefaultFileName => $"{Enum.GetName(typeof(PartialType), Type)}.{DateTime.Now.ToFileTime()}.json";
        }
    }
}
