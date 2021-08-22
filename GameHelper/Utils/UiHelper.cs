﻿// <copyright file="UiHelper.cs" company="None">
// Copyright (c) None. All rights reserved.
// </copyright>

namespace GameHelper.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using System.Reflection;
    using GameHelper.RemoteObjects;
    using GameOffsets.Natives;
    using ImGuiNET;

    /// <summary>
    /// Has helper functions to DRY out the Ui creation.
    /// </summary>
    public static class UiHelper
    {
        /// <summary>
        /// Flags associated with transparent ImGui window.
        /// </summary>
        public const ImGuiWindowFlags TransparentWindowFlags = ImGuiWindowFlags.NoInputs |
            ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoCollapse |
            ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoScrollbar |
            ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.AlwaysAutoResize |
            ImGuiWindowFlags.NoTitleBar;

        /// <summary>
        /// Converts rgba color information to uint32 color format.
        /// </summary>
        /// <param name="r">red color number between 0 - 255.</param>
        /// <param name="g">green color number between 0 - 255.</param>
        /// <param name="b">blue color number between 0 - 255.</param>
        /// <param name="a">alpha number between 0 - 255.</param>
        /// <returns>color in uint32 format.</returns>
        public static uint Color(uint r, uint g, uint b, uint a) => a << 24 | b << 16 | g << 8 | r;

        /// <summary>
        /// Draws the Rectangle on the screen.
        /// </summary>
        /// <param name="pos">Postion of the rectange.</param>
        /// <param name="size">Size of the rectange.</param>
        /// <param name="r">color selector red 0 - 255.</param>
        /// <param name="g">color selector green 0 - 255.</param>
        /// <param name="b">color selector blue 0 - 255.</param>
        public static void DrawRect(Vector2 pos, Vector2 size, byte r, byte g, byte b) =>
            ImGui.GetForegroundDrawList().AddRect(pos, pos + size, UiHelper.Color(r, g, b, 255), 0f, ImDrawCornerFlags.None, 2f);

        /// <summary>
        /// Draws the text on the screen.
        /// </summary>
        /// <param name="pos">world location to draw the text.</param>
        /// <param name="text">text to draw.</param>
        public static void DrawText(StdTuple3D<float> pos, string text)
        {
            var colBg = Color(0, 0, 0, 255);
            var colFg = Color(255, 255, 255, 255);
            var textSizeHalf = ImGui.CalcTextSize(text) / 2;
            var location = Core.States.InGameStateObject.WorldToScreen(pos);
            var max = location + textSizeHalf;
            location = location - textSizeHalf;
            ImGui.GetBackgroundDrawList().AddRectFilled(location, max, colBg);
            ImGui.GetForegroundDrawList().AddText(location, colFg, text);
        }

        /// <summary>
        /// Draws the disabled button on the ImGui.
        /// </summary>
        /// <param name="buttonLabel">text to write on the button.</param>
        public static void DrawDisabledButton(string buttonLabel)
        {
            uint col = UiHelper.Color(204, 204, 204, 128);
            ImGui.PushStyleColor(ImGuiCol.Button, col);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, col);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, col);
            ImGui.Button(buttonLabel);
            ImGui.PopStyleColor(3);
        }

        /// <summary>
        /// Helps convert address to ImGui Widget.
        /// </summary>
        /// <param name="name">name of the object whos address it is.</param>
        /// <param name="address">address of the object in the game.</param>
        public static void IntPtrToImGui(string name, IntPtr address)
        {
            var addr = address.ToInt64().ToString("X");
            ImGui.Text(name);
            ImGui.SameLine();
            ImGui.PushStyleColor(ImGuiCol.Button, Color(0, 0, 0, 0));
            if (ImGui.SmallButton(addr))
            {
                ImGui.SetClipboardText(addr);
            }

            ImGui.PopStyleColor();
        }

        /// <summary>
        /// Helps convert the text into ImGui widget that display the text
        /// and copy it if user click on it.
        /// </summary>
        /// <param name="displayText">text to display on the ImGui.</param>
        /// <param name="copyText">text to copy when user click.</param>
        public static void DisplayTextAndCopyOnClick(string displayText, string copyText)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, Color(0, 0, 0, 0));
            if (ImGui.SmallButton(displayText))
            {
                ImGui.SetClipboardText(copyText);
            }

            ImGui.PopStyleColor();
        }

        /// <summary>
        /// Creates a ImGui ComboBox for C# Enums.
        /// </summary>
        /// <typeparam name="T">Enum type to display in the ComboBox.</typeparam>
        /// <param name="displayText">Text to display along the ComboBox.</param>
        /// <param name="selected">Selected enum value in the ComboBox.</param>
        public static void EnumComboBox<T>(string displayText, ref T selected)
            where T : Enum
        {
            Type enumType = typeof(T);
            string[] enumNames = Enum.GetNames(enumType);
            int selectedIndex = (int)Convert.ChangeType(selected, typeof(int));
            if (ImGui.Combo(displayText, ref selectedIndex, enumNames, enumNames.Length))
            {
                selected = (T)Enum.Parse(enumType, enumNames[selectedIndex]);
            }
        }

        /// <summary>
        /// Iterates over properties of the given class via reflection
        /// and yields the <see cref="RemoteObjectBase"/> property name and its
        /// <see cref="RemoteObjectBase.ToImGui"/> method. Any property
        /// that doesn't have both the getter and setter method are ignored.
        /// </summary>
        /// <param name="classType">Type of the class to traverse.</param>
        /// <param name="propertyFlags">flags to filter the class properties.</param>
        /// <param name="classObject">Class object, or null for static class.</param>
        /// <returns>Yield the <see cref="RemoteObjectBase.ToImGui"/> method.</returns>
        internal static IEnumerable<RemoteObjectPropertyDetail> GetToImGuiMethods(
            Type classType,
            BindingFlags propertyFlags,
            object classObject)
        {
            var methodFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            var properties = classType.GetProperties(propertyFlags).ToList();
            for (int i = 0; i < properties.Count; i++)
            {
                PropertyInfo property = properties[i];
                if (property.SetMethod == null)
                {
                    continue;
                }

                object propertyValue = property.GetValue(classObject);
                if (propertyValue == null)
                {
                    continue;
                }

                Type propertyType = propertyValue.GetType();

                if (!typeof(RemoteObjectBase).IsAssignableFrom(propertyType))
                {
                    continue;
                }

                yield return new RemoteObjectPropertyDetail()
                {
                    Name = property.Name,
                    Value = propertyValue,
                    ToImGui = propertyType.GetMethod("ToImGui", methodFlags),
                };
            }
        }
    }
}