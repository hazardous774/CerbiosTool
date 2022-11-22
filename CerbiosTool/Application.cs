﻿using CerbiosTool.Shared;
using ImGuiNET;
using Repackinator.Shared;
using SharpDX.DXGI;
using System.Numerics;
using System.Text;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace RepackinatorUI
{
    public class Application
    {
        private Sdl2Window? m_window;
        private GraphicsDevice? m_graphicsDevice;
        private CommandList? m_commandList;
        private ImGuiController? m_controller;
        private PathPicker? m_biosFilePicker;
        private OkDialog? m_okDialog;
        private Config m_config = new();
        private bool m_biosLoaded = false;
        private byte[] m_biosData = Array.Empty<byte>();
        private string? m_biosPath;

        private readonly string m_version;

        public Application(string version)
        {
            m_version = version;
        }

        private static void SetXboxTheme()
        {
            ImGui.StyleColorsDark();
            var style = ImGui.GetStyle();
            var colors = style.Colors;
            colors[(int)ImGuiCol.Text] = new Vector4(0.94f, 0.94f, 0.94f, 1.00f);
            colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.86f, 0.93f, 0.89f, 0.28f);
            colors[(int)ImGuiCol.WindowBg] = new Vector4(0.10f, 0.10f, 0.10f, 1.00f);
            colors[(int)ImGuiCol.ChildBg] = new Vector4(0.06f, 0.06f, 0.06f, 0.98f);
            colors[(int)ImGuiCol.PopupBg] = new Vector4(0.10f, 0.10f, 0.10f, 1.00f);
            colors[(int)ImGuiCol.Border] = new Vector4(0.11f, 0.11f, 0.11f, 0.60f);
            colors[(int)ImGuiCol.BorderShadow] = new Vector4(0.16f, 0.16f, 0.16f, 0.00f);
            colors[(int)ImGuiCol.FrameBg] = new Vector4(0.18f, 0.18f, 0.18f, 1.00f);
            colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.30f, 0.30f, 0.30f, 1.00f);
            colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.28f, 0.71f, 0.25f, 1.00f);
            colors[(int)ImGuiCol.TitleBg] = new Vector4(0.20f, 0.51f, 0.18f, 1.00f);
            colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.26f, 0.66f, 0.23f, 1.00f);
            colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.16f, 0.16f, 0.16f, 0.75f);
            colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0.14f, 0.14f, 0.14f, 0.00f);
            colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.16f, 0.16f, 0.16f, 0.00f);
            colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.30f, 0.30f, 0.30f, 1.00f);
            colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.24f, 0.60f, 0.00f, 1.00f);
            colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.24f, 0.60f, 0.00f, 1.00f);
            colors[(int)ImGuiCol.CheckMark] = new Vector4(0.26f, 0.66f, 0.23f, 1.00f);
            colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.90f, 0.90f, 0.90f, 1.00f);
            colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(1.00f, 1.00f, 1.00f, 1.00f);
            colors[(int)ImGuiCol.Button] = new Vector4(0.17f, 0.17f, 0.17f, 1.00f);
            colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.24f, 0.60f, 0.00f, 1.00f);
            colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.26f, 0.66f, 0.23f, 1.00f);
            colors[(int)ImGuiCol.Header] = new Vector4(0.24f, 0.60f, 0.00f, 1.00f);
            colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.24f, 0.60f, 0.00f, 1.00f);
            colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.24f, 0.60f, 0.00f, 1.00f);
            colors[(int)ImGuiCol.Separator] = new Vector4(1.00f, 1.00f, 1.00f, 0.25f);
            colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(0.13f, 0.87f, 0.16f, 0.78f);
            colors[(int)ImGuiCol.SeparatorActive] = new Vector4(0.25f, 0.75f, 0.10f, 1.00f);
            colors[(int)ImGuiCol.ResizeGrip] = new Vector4(0.47f, 0.83f, 0.49f, 0.04f);
            colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.28f, 0.71f, 0.25f, 0.78f);
            colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(0.28f, 0.71f, 0.25f, 1.00f);
            colors[(int)ImGuiCol.Tab] = new Vector4(0.26f, 0.67f, 0.23f, 0.95f);
            colors[(int)ImGuiCol.TabHovered] = new Vector4(0.24f, 0.60f, 0.00f, 1.00f);
            colors[(int)ImGuiCol.TabActive] = new Vector4(0.24f, 0.60f, 0.00f, 1.00f);
            colors[(int)ImGuiCol.TabUnfocused] = new Vector4(0.21f, 0.54f, 0.19f, 0.99f);
            colors[(int)ImGuiCol.TabUnfocusedActive] = new Vector4(0.24f, 0.60f, 0.21f, 1.00f);
            colors[(int)ImGuiCol.PlotLines] = new Vector4(0.86f, 0.93f, 0.89f, 0.63f);
            colors[(int)ImGuiCol.PlotLinesHovered] = new Vector4(0.28f, 0.71f, 0.25f, 1.00f);
            colors[(int)ImGuiCol.PlotHistogram] = new Vector4(0.86f, 0.93f, 0.89f, 0.63f);
            colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(0.28f, 0.71f, 0.25f, 1.00f);
            colors[(int)ImGuiCol.TextSelectedBg] = new Vector4(0.26f, 0.66f, 0.23f, 1.00f);
            colors[(int)ImGuiCol.DragDropTarget] = new Vector4(1.00f, 1.00f, 0.00f, 0.90f);
            colors[(int)ImGuiCol.NavHighlight] = new Vector4(0.28f, 0.71f, 0.25f, 1.00f);
            colors[(int)ImGuiCol.NavWindowingHighlight] = new Vector4(1.00f, 1.00f, 1.00f, 0.70f);
            colors[(int)ImGuiCol.NavWindowingDimBg] = new Vector4(0.80f, 0.80f, 0.80f, 0.20f);
            colors[(int)ImGuiCol.ModalWindowDimBg] = new Vector4(0.16f, 0.16f, 0.16f, 0.73f);

            style.WindowRounding = 6;
            style.FrameRounding = 6;
            style.PopupRounding = 6;
        }

        private static void DrawToggle(bool enabled, bool hovered, Vector2 pos, Vector2 size)
        {
            var drawList = ImGui.GetWindowDrawList();

            float radius = size.Y * 0.5f;
            float rounding = size.Y * 0.25f;
            float slotHalfHeight = size.Y * 0.5f;

            var background = hovered ? ImGui.GetColorU32(enabled ? ImGuiCol.FrameBgActive : ImGuiCol.FrameBgHovered) : ImGui.GetColorU32(enabled ? ImGuiCol.CheckMark : ImGuiCol.FrameBg);

            var paddingMid = new Vector2(pos.X + radius + (enabled ? 1 : 0) * (size.X - radius * 2), pos.Y + size.Y / 2);
            var sizeMin = new Vector2(pos.X, paddingMid.Y - slotHalfHeight);
            var sizeMax = new Vector2(pos.X + size.X, paddingMid.Y + slotHalfHeight);
            drawList.AddRectFilled(sizeMin, sizeMax, background, rounding);

            var offs = new Vector2(radius * 0.8f, radius * 0.8f);
            drawList.AddRectFilled(paddingMid - offs, paddingMid + offs, ImGui.GetColorU32(ImGuiCol.SliderGrab), rounding);
        }

        private static bool Toggle(string str_id, ref bool v, Vector2 size)
        {
            ImGui.PushStyleColor(ImGuiCol.Button, ImGui.GetColorU32(new Vector4()));

            var style = ImGui.GetStyle();

            ImGui.PushID(str_id);
            bool status = ImGui.Button("###toggle_button", size);
            if (status)
            {
                v = !v;
            }
            ImGui.PopID();

            var maxRect = ImGui.GetItemRectMax();
            var toggleSize = new Vector2(size.X - 8, size.Y - 8);
            var togglePos = new Vector2(maxRect.X - toggleSize.X - style.FramePadding.X, maxRect.Y - toggleSize.Y - style.FramePadding.Y);
            DrawToggle(v, ImGui.IsItemHovered(), togglePos, toggleSize);

            ImGui.PopStyleColor();

            return status;
        }

        public void Run()
        {
            m_biosPath = string.Empty;

            VeldridStartup.CreateWindowAndGraphicsDevice(new WindowCreateInfo(50, 50, 556 + 320, 410 + 150, WindowState.Normal, $"CerbiosTool - {m_version}"), new GraphicsDeviceOptions(true, null, true, ResourceBindingModel.Improved, true, true), VeldridStartup.GetPlatformDefaultBackend(), out m_window, out m_graphicsDevice);
           
            m_window.Resizable = false;

            m_controller = new ImGuiController(m_graphicsDevice, m_graphicsDevice.MainSwapchain.Framebuffer.OutputDescription, m_window.Width, m_window.Height);

            SetXboxTheme();

            m_biosFilePicker = new PathPicker
            {
                Mode = PathPicker.PickerMode.File,
                ButtonName = "Open"
            };     

            m_okDialog = new();

            m_config = Config.LoadConfig();

            m_window.Resized += () =>
            {
                m_graphicsDevice.MainSwapchain.Resize((uint)m_window.Width, (uint)m_window.Height);
                m_controller.WindowResized(m_window.Width, m_window.Height);
            };

            m_commandList = m_graphicsDevice.ResourceFactory.CreateCommandList();

            while (m_window.Exists)
            {
                InputSnapshot snapshot = m_window.PumpEvents();
                if (!m_window.Exists)
                {
                    break;
                }
                m_controller.Update(1f / 60f, snapshot);

                RenderUI();

                m_commandList.Begin();
                m_commandList.SetFramebuffer(m_graphicsDevice.MainSwapchain.Framebuffer);
                m_commandList.ClearColorTarget(0, new RgbaFloat(0.0f, 0.0f, 0.0f, 1f));
                m_controller.Render(m_graphicsDevice, m_commandList);
                m_commandList.End();
                m_graphicsDevice.SubmitCommands(m_commandList);
                m_graphicsDevice.SwapBuffers(m_graphicsDevice.MainSwapchain);
            }

            m_graphicsDevice.WaitForIdle();
            m_controller.Dispose();
            m_commandList.Dispose();
            m_graphicsDevice.Dispose();
        }

        private void RenderUI()
        {
            if (m_window == null ||
                m_controller == null ||
                m_biosFilePicker == null ||
                m_okDialog == null)
            {
                return;
            }

            if (m_biosFilePicker.Render() && !m_biosFilePicker.Cancelled)
            {
                m_biosPath = Path.Combine(m_biosFilePicker.SelectedFolder, m_biosFilePicker.SelectedFile);
                m_biosLoaded = BiosUtility.LoadBiosComfig(m_biosPath, ref m_config, ref m_biosData); 
            }

            m_okDialog.Render();

            ImGui.Begin("Main", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize);
            ImGui.SetWindowSize(new Vector2(m_window.Width, m_window.Height));
            ImGui.SetWindowPos(new Vector2(0, 0), ImGuiCond.Always);

            if (!m_biosLoaded)
            {
                ImGui.BeginDisabled();
            }

            ImGui.Spacing();

            string[] udmaModes = new string[] { "UDMA 2", "UDMA 3", "UDMA 4", "UDMA 5" };

            var udmaMode = (int)m_config.UDMAMode - 2;
            ImGui.Text("UDMA Mode:");
            ImGui.PushItemWidth(200);
            ImGui.Combo("##udmaMode", ref udmaMode, udmaModes, udmaModes.Length);            
            ImGui.PopItemWidth();
            m_config.UDMAMode = (uint)udmaMode + 2;

            var splashBackground = Config.RGBToVector3(m_config.SplashBackground);
            ImGui.Text("Splash Background:");
            ImGui.PushItemWidth(200);
            ImGui.ColorEdit3("##splashBackground", ref splashBackground, ImGuiColorEditFlags.DisplayHex);
            ImGui.PopItemWidth();
            m_config.SplashBackground = Config.Vector3ToRGB(splashBackground);

            var splashCerbiosText = Config.RGBToVector3(m_config.SplashCerbiosText);
            ImGui.Text("Splash Cerbios Text:");
            ImGui.PushItemWidth(200);
            ImGui.ColorEdit3("##splashCerbiosText", ref splashCerbiosText, ImGuiColorEditFlags.DisplayHex);
            ImGui.PopItemWidth();
            m_config.SplashCerbiosText = Config.Vector3ToRGB(splashCerbiosText);

            var splashSafeModeText = Config.RGBToVector3(m_config.SplashSafeModeText);
            ImGui.Text("Splash SafeMode Text:");
            ImGui.PushItemWidth(200);
            ImGui.ColorEdit3("##splashSafeModeText", ref splashSafeModeText, ImGuiColorEditFlags.DisplayHex);
            ImGui.PopItemWidth();
            m_config.SplashSafeModeText = Config.Vector3ToRGB(splashSafeModeText);

            var splashLogo1 = Config.RGBToVector3(m_config.SplashLogo1);
            ImGui.Text("Splash Logo1:");
            ImGui.PushItemWidth(200);
            ImGui.ColorEdit3("##splashLogo1", ref splashLogo1, ImGuiColorEditFlags.DisplayHex);
            ImGui.PopItemWidth();
            m_config.SplashLogo1 = Config.Vector3ToRGB(splashLogo1);

            var splashLogo2 = Config.RGBToVector3(m_config.SplashLogo2);
            ImGui.Text("Splash Logo2:");
            ImGui.PushItemWidth(200);
            ImGui.ColorEdit3("##splashLogo2", ref splashLogo2, ImGuiColorEditFlags.DisplayHex);
            ImGui.PopItemWidth();
            m_config.SplashLogo2 = Config.Vector3ToRGB(splashLogo2);

            var splashLogo3 = Config.RGBToVector3(m_config.SplashLogo3);
            ImGui.Text("Splash Logo3:");
            ImGui.PushItemWidth(200);
            ImGui.ColorEdit3("##splashLogo3", ref splashLogo3, ImGuiColorEditFlags.DisplayHex);
            ImGui.PopItemWidth();
            m_config.SplashLogo3 = Config.Vector3ToRGB(splashLogo3);

            if (!m_biosLoaded)
            {
                ImGui.EndDisabled();
            }

            var startPos = new Vector2(220, 26);
            var size = new Vector2(640, 480);
            var drawList = ImGui.GetWindowDrawList();
            drawList.AddRectFilled(startPos, startPos + size, ImGui.ColorConvertFloat4ToU32(Config.RGBToVector4(m_config.SplashBackground)));
            drawList.AddRect(startPos, startPos + size, ImGui.ColorConvertFloat4ToU32(new Vector4(0.5f, 0.5f, 0.5f, 0.5f)));
            ImGui.SetCursorPos(startPos);
            ImGui.Image(m_controller.Logo1Texture, size, Vector2.Zero, Vector2.One, Config.RGBToVector4(m_config.SplashLogo1));
            ImGui.SetCursorPos(startPos);
            ImGui.Image(m_controller.Logo2Texture, size, Vector2.Zero, Vector2.One, Config.RGBToVector4(m_config.SplashLogo2));
            ImGui.SetCursorPos(startPos);
            ImGui.Image(m_controller.Logo3Texture, size, Vector2.Zero, Vector2.One, Config.RGBToVector4(m_config.SplashLogo3));
            ImGui.SetCursorPos(startPos);
            ImGui.Image(m_controller.CerbiosTextTexture, size, Vector2.Zero, Vector2.One, Config.RGBToVector4(m_config.SplashCerbiosText));
            ImGui.SetCursorPos(startPos);
            ImGui.Image(m_controller.SafeModeTextTexture, size, Vector2.Zero, Vector2.One, Config.RGBToVector4(m_config.SplashSafeModeText));

            ImGui.SetCursorPosY(m_window.Height - 40);

            if (ImGui.Button("Open", new Vector2(75, 30)))
            {
                m_biosFilePicker.ShowModal(Directory.GetCurrentDirectory());
            }

            if (File.Exists(m_biosPath))
            {
                ImGui.SameLine();

                if (ImGui.Button("Default", new Vector2(75, 30)))
                {
                    m_config.SetDefaults();
                }

                ImGui.SameLine();

                if (ImGui.Button("Save", new Vector2(75, 30)))
                {
                    BiosUtility.SaveBiosComfig(m_config, m_biosPath, m_biosData);
                }
            }

            var message = "Coded by EqUiNoX - Team Resurgent";
            var messageSize = ImGui.CalcTextSize(message);
            ImGui.SetCursorPos(new Vector2(m_window.Width - messageSize.X - 10, m_window.Height - messageSize.Y - 10));
            ImGui.Text(message);
            ImGui.SetCursorPos(new Vector2(m_window.Width - messageSize.X - 10, m_window.Height - messageSize.Y - 10));
            if (ImGui.InvisibleButton("##credits", messageSize))
            {
                // do nothing for now
            }

            ImGui.End();
        }
    }
}
