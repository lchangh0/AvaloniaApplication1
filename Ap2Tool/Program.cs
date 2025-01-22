using ApCommon;
using Avalonia;
using GenLib;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ap2Tool
{
    internal sealed class Program
    {
        static Mutex? _mutex;
        static CLogger _logger;
        public static CLogger Logger { get { return _logger; } }
        

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            #region 중복실행 방지
            _mutex = new Mutex(initiallyOwned: true, "Ap2Tool", out bool createdNew);
            if (!createdNew)
            {
                Console.WriteLine("Ap2Tool is already running.");
                return;
            }
            #endregion

            string strWorkDir = CGenLib.GetWorkDir();
            CSolutionGlobal.WorkDir = strWorkDir;
            string strLogDir = Path.Combine(strWorkDir, "log");
            if (!Directory.Exists(strLogDir))
                Directory.CreateDirectory(strLogDir);
            CSolutionGlobal.LogDir = strLogDir;

            CApp.ConfigNLog(strLogDir, NLog.LogLevel.Trace);

            _logger = new CLogger("Ap2Tool");
            _logger.Log("Program Start ----------");
            _logger.Log(CApp.GetAppInfo());
            _logger.Log($"RunDir={strWorkDir}");

            if (ConfigurationManager.AppSettings.Count == 0)
            {
                CAppConfig.CreateAppConfigFile();
            }

            var builder = BuildAvaloniaApp();
            builder.StartWithClassicDesktopLifetime(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();

    }
}
