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

using System;
using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;

namespace SirRandoo.ToolkitUtils
{
    public static partial class Data
    {
        private static readonly List<string> Viewers = new List<string>();
        private static readonly ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();

        [NotNull]
        public static List<string> AllViewers
        {
            get
            {
                if (!Lock.TryEnterReadLock(300))
                {
                    return new List<string>(0);
                }

                var viewers = new List<string>(Viewers);
                Lock.ExitReadLock();

                return viewers;
            }
        }

        public static bool RegisterViewer(string username)
        {
            if (!Lock.TryEnterUpgradeableReadLock(300))
            {
                return false;
            }

            bool exists = Viewers.Find(u => string.Equals(u, username, StringComparison.OrdinalIgnoreCase)) == null;

            if (exists)
            {
                Lock.ExitUpgradeableReadLock();
                
                return false;
            }

            if (Lock.TryEnterWriteLock(300))
            {
                Viewers.Add(username);
                
                Lock.ExitWriteLock();
                Lock.ExitUpgradeableReadLock();

                return true;
            }
            
            Lock.ExitUpgradeableReadLock();

            return false;
        }

        public static bool UnregisterViewer(string username)
        {
            if (!Lock.TryEnterUpgradeableReadLock(300))
            {
                return false;
            }

            string viewer = Viewers.Find(u => string.Equals(u, username, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(viewer) && Lock.TryEnterWriteLock(300))
            {
                Viewers.Remove(viewer);
                
                Lock.ExitWriteLock();
                Lock.ExitUpgradeableReadLock();

                return true;
            }
            
            Lock.ExitUpgradeableReadLock();

            return false;
        }
    }
}
