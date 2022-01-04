using System;
using OpenTK.Windowing.Desktop;

namespace MomentShadowMappingTest
{
    class Program
    {
        static void Main(string[] args)
        {
            GameWindowSettings gws = new GameWindowSettings();
            gws.IsMultiThreaded = false;
            gws.RenderFrequency = 0;
            gws.UpdateFrequency = 0;
            NativeWindowSettings nws = new NativeWindowSettings();
            nws.Flags = OpenTK.Windowing.Common.ContextFlags.Default;
            nws.IsFullscreen = false;
            nws.Size = new OpenTK.Mathematics.Vector2i(1280, 720);
            nws.NumberOfSamples = 0; // FSAA
            nws.Title = "Mein OpenGL-Projekt";
            nws.WindowBorder = OpenTK.Windowing.Common.WindowBorder.Resizable;
            

            ApplicationWindow w = new ApplicationWindow(gws, nws);
            w.VSync = OpenTK.Windowing.Common.VSyncMode.Off;
            w.Run();
            w.Dispose();
        }
    }
}
