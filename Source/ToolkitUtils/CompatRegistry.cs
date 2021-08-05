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

using System.Collections.Generic;
using SirRandoo.ToolkitUtils.Compat;
using SirRandoo.ToolkitUtils.Interfaces;
using SirRandoo.ToolkitUtils.Models;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    [StaticConstructorOnStartup]
    public static class CompatRegistry
    {
        static CompatRegistry()
        {
            HealHandlers = new List<IHealHandler> { new DefaultHealHandler() };
            SurgeryHandlers = new List<ISurgeryHandler> { new DefaultSurgeryHandler(), new AndroidSurgeryHandler() };
            UsabilityHandlers =
                new List<IUsabilityHandler> { new DefaultUsabilityHandler(), new IngestabilityUsabilityHandler() };
        }

        public static MagicCompat Magic { get; set; }
        public static List<IUsabilityHandler> UsabilityHandlers { get; }
        public static List<ISurgeryHandler> SurgeryHandlers { get; }
        public static List<IHealHandler> HealHandlers { get; }
    }
}
