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

using System;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using Verse;

namespace SirRandoo.ToolkitUtils.Utils.ModComp
{
    [UsedImplicitly]
    [StaticConstructorOnStartup]
    public static class Puppeteer
    {
        public static readonly bool Active;
        private static readonly Type Controller;
        private static readonly FieldInfo ControllerInstance;
        private static readonly MethodInfo SendChatMessage;
        private static readonly Type State;
        private static readonly PropertyInfo StateInstance;
        private static readonly MethodInfo PuppeteerForViewerName;
        private static readonly FieldInfo ViewerId;
        private static readonly PropertyInfo IsConnected;

        static Puppeteer()
        {
            if (ModLister.GetActiveModWithIdentifier("brrainz.puppeteer") == null)
            {
                return;
            }

            try
            {
                Controller = AccessTools.TypeByName("Puppeteer.Controller");
                ControllerInstance = AccessTools.Field(Controller, "instance");
                SendChatMessage = AccessTools.Method("Puppeteer.Controller:SendChatMessage");

                State = AccessTools.TypeByName("Puppeteer.State");
                StateInstance = AccessTools.Property(State, "Instance");
                PuppeteerForViewerName = AccessTools.Method("Puppeteer.State:PuppeteerForViewerName");

                Type puppeteer = AccessTools.TypeByName("Puppeteer.State.Puppeteer");
                ViewerId = AccessTools.Field(puppeteer, "vID");
                IsConnected = AccessTools.Property(puppeteer, "IsConnected");
                Active = true;
            }
            catch (Exception e)
            {
                LogHelper.Error("Puppeteer compatibility could not be loaded", e);
            }
        }

        public static void SendMessage(string viewer, string message)
        {
            if (!Active)
            {
                return;
            }

            object stateInstance = StateInstance.GetValue(State);
            object puppet = PuppeteerForViewerName.Invoke(stateInstance, new object[] {viewer});

            if (puppet == null || IsConnected.GetValue(puppet) is bool state && !state)
            {
                return;
            }

            object viewerId = ViewerId.GetValue(puppet);
            object controllerInstance = ControllerInstance.GetValue(Controller);
            SendChatMessage.Invoke(controllerInstance, new[] {viewerId, message});
        }

        public static bool ShouldRedirect(string viewer)
        {
            return false;
            if (!Active)
            {
                return false;
            }

            object stateInstance = StateInstance.GetValue(State);
            object puppet = PuppeteerForViewerName.Invoke(stateInstance, new object[] {viewer});

            return puppet != null && IsConnected.GetValue(puppet) is bool state && state;
        }
    }
}
