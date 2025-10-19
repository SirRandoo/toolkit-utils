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
// with ToolkitUtils.TMagic. If not, see <https://www.gnu.org/licenses/>.
using ToolkitUtils.Mod;
using ToolkitUtils.Mod.Domain.Products;
using ToolkitUtils.Mod.Presentation;
using ToolkitUtils.Mod.Presentation.Drawers;
using TorannMagic;
using UnityEngine;
using Verse;

namespace ToolkitUtils.TMagic;

public class ClassSelector : ISelector
{
    private string _classText = null!;
    private bool _state = true;

    public string Id { get; init; } = "sirrandoo.tku:selectors.class";
    public string Name { get; init; } = "TKUtils.Fields.Class".TranslateSimple();

    public bool Filter(IIdentifiable product)
    {
        if (product is not TraitProduct traitProduct) return false;
        if (traitProduct.Def.Equals(TorannMagicDefOf.DeathKnight)) return _state;

        return TM_Data.AllClassTraits.Contains(traitProduct.Def) && _state;
    }

    public void Prepare()
    {
        _classText = "TKUtils.Fields.Class".TranslateSimple();
    }

    public void Draw(Rect region)
    {
        CheckboxDrawer.DrawCheckbox(region, _classText, ref _state);
    }
}
