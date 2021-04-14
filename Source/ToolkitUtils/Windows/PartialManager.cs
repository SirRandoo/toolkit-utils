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
using System.Threading.Tasks;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Models;
using UnityEngine;
using Utf8Json;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows
{
    public class PartialManager<T> : Window where T : class, IShopItemBase
    {
        private Action<FileData<T>> callback;
        private List<FileData<T>> files = new List<FileData<T>>();

        public PartialManager(Action<FileData<T>> callback)
        {
            doCloseButton = false;
            this.callback = callback;
            layer = WindowLayer.Dialog;
        }

        public override Vector2 InitialSize => new Vector2(500, 500);

        protected override float Margin => 0f;

        public override void DoWindowContents(Rect canvas) { }

        public override void PostOpen()
        {
            base.PostOpen();
            Task.Run(async () => { files = await IndexPartialFiles(); });
        }

        private static async Task<List<FileData<T>>> IndexPartialFiles()
        {
            var container = new List<FileData<T>>();

            foreach (string file in Directory.EnumerateFileSystemEntries(
                Paths.PartialPath,
                "*.json",
                SearchOption.TopDirectoryOnly
            ))
            {
                string path = Path.Combine(Paths.PartialPath, file);

                if (Directory.Exists(path))
                {
                    container.Add(
                        new FileData<T> {Description = path, IsDirectory = true, Extension = "", Name = file}
                    );
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
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    container.Add(data);
                }
            }

            return container;
        }
    }
}
