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
            nws.Flags = OpenTK.Windowing.Common.ContextFlags.Debug;
            nws.IsFullscreen = false;
            nws.NumberOfSamples = 0; // FSAA
            nws.Title = "Mein OpenGL-Projekt";
            nws.WindowBorder = OpenTK.Windowing.Common.WindowBorder.Resizable;

            ApplicationWindow w = new ApplicationWindow(gws, nws);
            w.Run();
            w.Dispose();
        }
    }
}
