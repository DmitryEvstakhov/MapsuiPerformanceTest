using System;
using Avalonia;
using Avalonia.ReactiveUI;

namespace AvaloniaMapsUiTest
{
    internal sealed class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                //.With(new X11PlatformOptions
                //{
                //    RenderingMode = new[] { X11RenderingMode.Egl }
                //}) 

                //.With(new Win32PlatformOptions
                //{
                //    RenderingMode = new[] { Win32RenderingMode.AngleEgl },
                //    CompositionMode = new[]
                //    {
                //        Win32CompositionMode.WinUIComposition,
                //        Win32CompositionMode.DirectComposition,
                //        Win32CompositionMode.LowLatencyDxgiSwapChain
                //    }
                //    ,
                //})

                //.With(new SkiaOptions
                //{
                //    MaxGpuResourceSizeBytes = 1024l * 1024 * 1024 * 4
                //})
                .WithInterFont()
                .LogToTrace()
                .UseReactiveUI();
    }
}
