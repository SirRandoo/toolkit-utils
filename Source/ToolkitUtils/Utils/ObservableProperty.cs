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

using System.Diagnostics.CodeAnalysis;

namespace SirRandoo.ToolkitUtils.Utils;

public class ObservableProperty<T>
{
    public delegate void ChangeEvent(T data);

    private T _value;

    public ObservableProperty(T initialValue)
    {
        _value = initialValue;
    }

    public event ChangeEvent? Changed;

    public void Set([DisallowNull] T val)
    {
        if (val.Equals(_value))
        {
            return;
        }

        _value = val;

        Changed?.Invoke(_value);
    }

    public T Get() => _value;

    public static implicit operator T(ObservableProperty<T> prop) => prop._value;
}
