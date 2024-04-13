// MIT License
//
// Copyright (c) 2024 SirRandoo
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
using RimWorld;
using ToolkitUtils.UX;
using UnityEngine;
using Verse;

namespace SirRandoo.ToolkitUtils.Windows;

public class ProxySettingsWindow : Window
{
    private readonly Verse.Mod _mod;
    private bool _hasSettings;
    private string _lastException = null!;
    private FloatMenu _noSettingsFloatMenu = null!;
    private string _selectModText = null!;
    private bool _settingsCloseAttempted;
    private FloatMenu _settingsFloatMenu = null!;
    private int _totalErrors;

    public ProxySettingsWindow(Mod mod)
    {
        _mod = mod;
        forcePause = true;
        doCloseX = true;
        doCloseButton = true;
        closeOnClickedOutside = true;
        absorbInputAroundWindow = true;
    }

    public override Vector2 InitialSize => new(900f, 700f);

    public override void DoWindowContents(Rect inRect)
    {
        var headerRect = new Rect(0f, 0f, inRect.width, 35f);
        var settingsRect = new Rect(0f, 40f, inRect.width, inRect.height - 40f - CloseButSize.y);

        GUI.BeginGroup(inRect);

        GUI.BeginGroup(headerRect);
        DrawHeader(headerRect);
        GUI.EndGroup();

        GUI.BeginGroup(settingsRect);
        DrawSettings(settingsRect.AtZero());
        GUI.EndGroup();

        GUI.EndGroup();
    }

    private void DrawHeader(Rect region)
    {
        var btnRect = new Rect(0f, 0f, 150f, region.height);
        var labelRect = new Rect(167f, 0f, region.width - 150f - 17f, region.height);

        if (Widgets.ButtonText(btnRect, _selectModText))
        {
            Find.WindowStack.Add(_hasSettings ? _settingsFloatMenu : _noSettingsFloatMenu);
        }

        Text.Font = GameFont.Medium;
        Widgets.Label(labelRect, _mod.SettingsCategory());
        Text.Font = GameFont.Small;
    }

    protected virtual void DrawSettings(Rect region)
    {
        if (_totalErrors >= 20)
        {
            LabelDrawer.Draw(region, _lastException, Color.gray, TextAnchor.UpperLeft);

            return;
        }

        try
        {
            _mod.DoSettingsWindowContents(region);
        }
        catch (Exception e)
        {
            _lastException = StackTraceUtility.ExtractStringFromException(e);
            _totalErrors++;
        }
    }

    protected virtual void GetTranslations()
    {
    }

    public override void PostOpen()
    {
        var modSettings = new List<FloatMenuOption>();

        foreach (Verse.Mod handle in LoadedModManager.ModHandles)
        {
            if (handle.SettingsCategory().NullOrEmpty())
            {
                continue;
            }

            _hasSettings = true;
            modSettings.Add(new FloatMenuOption(handle.SettingsCategory(), () => DisplayMod(handle)));
        }

        _selectModText = "SelectMod".TranslateSimple();

        if (!_hasSettings)
        {
            _noSettingsFloatMenu = new FloatMenu([new FloatMenuOption("NoConfigurableMods".TranslateSimple(), null)]);
        }

        _settingsFloatMenu = new FloatMenu(modSettings);
        GetTranslations();
    }

    public override void PreClose()
    {
        base.PreClose();
        _mod.WriteSettings();
    }

    private void DisplayMod(Mod handle)
    {
        if (handle == TkUtils.Instance)
        {
            Find.WindowStack.Add(TkUtils.Instance.SettingsWindow);
            Find.WindowStack.TryRemove(this, false);

            return;
        }

        var window = new Dialog_ModSettings(handle);

        Find.WindowStack.TryRemove(this, false);
        Find.WindowStack.Add(window);
    }

    internal static void Open(ProxySettingsWindow window)
    {
        if (!window._settingsCloseAttempted)
        {
            Find.WindowStack.TryRemove(typeof(Dialog_ModSettings));
            window._settingsCloseAttempted = true;
        }

        if (!Find.WindowStack.IsOpen(window.GetType()))
        {
            Find.WindowStack.Add(window);
        }
    }
}
