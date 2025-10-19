// Copyright (C) 2025 sirrandoo
// 
// This file is part of ToolkitUtils.
// 
// ToolkitUtils is free software: you can redistribute it and/or modify it under
// the terms of the GNU Lesser General Public License version 3 as published by the
// Free Software Foundation.
// 
// ToolkitUtils is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License
// for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along
// with ToolkitUtils.Mod. If not, see <https://www.gnu.org/licenses/>.
using System;

namespace ToolkitUtils.Mod.Messaging;

/// <summary>An event indicating the necessity to invalidate the translation cache.</summary>
/// <remarks>
///     This event is usually triggered when translation data is modified, necessitating the cache to be cleared and
///     refreshed.
/// </remarks>
public sealed class InvalidateTranslationCacheEventArgs : EventArgs {}
