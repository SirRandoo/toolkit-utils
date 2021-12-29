// ToolkitUtils
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

using JetBrains.Annotations;

namespace SirRandoo.ToolkitUtils.Utils
{
    public class ObservableProperty<T>
    {
        public delegate void ChangeEvent(T data);

        private T value;

        public ObservableProperty(T initialValue)
        {
            value = initialValue;
        }

        public event ChangeEvent Changed;

        public void Set([NotNull] T val)
        {
            if (val.Equals(value))
            {
                return;
            }

            value = val;

            Changed?.Invoke(value);
        }

        public T Get() => value;

        public static implicit operator T([NotNull] ObservableProperty<T> prop) => prop.value;
    }
}
