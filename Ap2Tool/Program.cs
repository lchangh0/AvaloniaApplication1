﻿using ApCommon;
using Avalonia;
using GenLib;
using System;
using System.IO;

namespace Ap2Tool
{
    internal sealed class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            string strWorkDir = CGenLib.GetWorkDir();
            CSolutionGlobal.WorkDir = strWorkDir;
            string strLogDir = Path.Combine(strWorkDir, "log");
            if (!Directory.Exists(strLogDir))
                Directory.CreateDirectory(strLogDir);
            CSolutionGlobal.LogDir = strLogDir;

            BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
    }
}
