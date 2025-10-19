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
using JetBrains.Annotations;
using ToolkitUtils.Mod.Data;
using Verse;

namespace ToolkitUtils.Mod.Defs;

/// <summary>
///     A component that can be attached to a Thing in the RimWorld game to represent an assignment to a specific
///     viewer.
/// </summary>
/// <remarks>This component tracks whether a Thing is assigned to someone and manages data persistence for assignments.</remarks>
[PublicAPI]
[UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
public sealed class AssignedThingComp : ThingComp
{
    /// <summary>Gets a value indicating whether the associated object is currently assigned to a viewer.</summary>
    /// <value><c>true</c> if the object is assigned; otherwise, <c>false</c>.</value>
    /// <remarks>This property is determined by checking if the <see cref="AssignedTo" /> field is non-null.</remarks>
    public bool IsAssigned => AssignedTo != null;

    /// <summary>Gets the viewer to whom the associated object is assigned, if any.</summary>
    /// <value>An instance of <see cref="Viewer" /> representing the assigned viewer, or <c>null</c> if no viewer is assigned.</value>
    /// <remarks>
    ///     This property indicates the current viewer assigned to the object and is primarily used to determine ownership
    ///     or control within the system.
    /// </remarks>
    public Viewer? AssignedTo { get; private set; }

    /// <summary>Gets the component properties specific to the <see cref="AssignedThingComp" /> instance.</summary>
    /// <value>The <see cref="AssignedCompProperties" /> instance that defines custom behavior and data for the component.</value>
    /// <remarks>
    ///     This property casts the generic <see cref="ThingComp.props" /> to <see cref="AssignedCompProperties" />. It
    ///     provides a way to access the specific configuration and properties defined for this component.
    /// </remarks>
    public AssignedCompProperties Props => (AssignedCompProperties)props;

    /// <inheritdoc />
    public override void PostExposeData()
    {
        base.PostExposeData();

        const string viewerIdKey = "viewerId";
        string? viewerId = AssignedTo?.Id;

        switch (Scribe.mode)
        {
            case LoadSaveMode.Saving:
            {
                Scribe_Values.Look(ref viewerId!, viewerIdKey);

                break;
            }
            case LoadSaveMode.LoadingVars:
            {
                Scribe_Values.Look(ref viewerId, viewerIdKey);

                AssignedTo = null; // FIXME: This doesn't properly lookup the viewer a pawn is assigned to.

                break;
            }
        }
    }

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);

        // TODO: Notify the mod if the pawn is assigned to a viewer.
    }
}

/// <summary>
///     Defines the properties for the <see cref="AssignedThingComp" /> component, enabling its configuration and
///     behavior.
/// </summary>
/// <remarks>
///     This class specifies shared data and settings for the <see cref="AssignedThingComp" /> component. It is
///     responsible for establishing the association between the properties and the component type, ensuring that the
///     component operates within the defined parameters.
/// </remarks>
[UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
public sealed class AssignedCompProperties : CompProperties
{
    public AssignedCompProperties()
    {
        compClass = typeof(AssignedThingComp);
    }
}
