// MIT License
// 
// Copyright (c) 2023 SirRandoo
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

using UnityEngine;
using Verse;

namespace ToolkitUtils.UX
{
    public abstract class GuiElementBase<T> : IGuiElement<T>
    {
        private string _description;
        private float _height;
        private string _label;
        private bool _recalculateHeight = true;

        /// <inheritdoc/>
        public abstract void ExposeData();

        /// <inheritdoc/>
        public virtual T Value { get; set; }

        /// <inheritdoc/>
        public string Label
        {
            get => _label;
            set
            {
                _label = value;
                _recalculateHeight = true;
            }
        }

        /// <inheritdoc/>
        public string Tooltip { get; set; }

        /// <inheritdoc/>
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                _recalculateHeight = true;
            }
        }

        /// <inheritdoc/>
        public abstract void Draw(Rect region);

        /// <inheritdoc/>
        public float GetHeight(float width)
        {
            if (!_recalculateHeight)
            {
                return _height;
            }

            GameFont cache = Text.Font;
            _height = 0f;

            if (!string.IsNullOrEmpty(Label))
            {
                Text.Font = GameFont.Small;
                _height += Text.CalcHeight(Label, width);
                Text.Font = cache;
            }

            if (string.IsNullOrEmpty(Description))
            {
                return _height;
            }

            Text.Font = GameFont.Tiny;
            _height += Text.CalcHeight(Description, width) + Text.SpaceBetweenLines;
            Text.Font = cache;

            return _height;
        }
    }
}
