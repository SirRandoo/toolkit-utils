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

namespace ToolkitUtils.Mod.Presentation;

/// <summary>Used to mark fields as a container for a given translation string.</summary>
/// <param name="key">
///     The translation string's key. This is typically the name of an XML element within a mod's translation
///     file, but may be broken in pieces should <see cref="TranslationNamespaceAttribute" /> is also used.
/// </param>
/// <param name="color"></param>
[AttributeUsage(AttributeTargets.Field)]
public sealed class TranslationAttribute(string key, string? color = null) : Attribute
{
    /// <inheritdoc cref="TranslationAttribute" path="/param[@name='key']" />
    public string Key { get; } = key;

    /// <summary>The optional color of the translation string.</summary>
    public string? Color { get; } = color;
}

/// <summary>
///     Used to mark classes, that contain one or more <see cref="TranslationAttribute" />d fields, with a translation
///     namespace. Translation namespaces are simply strings that are prefixed to all
///     <see cref="TranslationAttribute.Key" /> within the class.
/// </summary>
/// <param name="namespace">The namespace of translations within the class.</param>
/// <remarks>
///     This is typically used to reduce the amount of horizontal scrolling required to see class fields that have a
///     <see cref="TranslationAttribute" />.
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public sealed class TranslationNamespaceAttribute(string @namespace) : Attribute
{
    /// <inheritdoc cref="TranslationNamespaceAttribute" path="/param[@name='namespace']" />
    public string Namespace { get; } = @namespace;
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class TranslationRecacheListenerAttribute : Attribute {}
