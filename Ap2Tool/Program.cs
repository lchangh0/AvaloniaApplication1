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
        public static string VERSION
        {
            get
            {
                return CVersionLib.GetProductVersion();
            }
        }

        

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static async Task Main(string[] args)
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

            ConfigNLog(strLogDir, NLog.LogLevel.Trace);

            _logger = new CLogger("Ap2Tool");
            _logger.Log("Program Start ----------");
            _logger.Log(GetAppInfo());
            _logger.Log($"RunDir={strWorkDir}");

            if (ConfigurationManager.AppSettings.Count == 0)
            {
                AppConfig.CreateAppConfigFile();
            }

            m_strConfigFilePath = Path.Combine(CSolutionGlobal.WorkDir, CConst.CONFIG_FILE);
            m_strLogDir = CSolutionGlobal.LogDir;

            await LoadAndInitConfigAsync(m_strConfigFilePath);

            // 리소스파일을 로드한다.
            string strResourceFileName = m_config.GetValue<string>(EConfigId.ResourceFileName);
            LoadResourceFile(strResourceFileName, m_config);

            BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();


        static CConfig m_config;
        public static CConfig Config
        {
            get { return m_config; }
        }
        static string m_strConfigFilePath;
        static string m_strCofigJsonLoaded;
        static string m_strLogDir;

        static async Task<bool> LoadAndInitConfigAsync(string strConfigFilePath)
        {
            m_config = new CConfig();
            StringBuilder sbLog = new StringBuilder();
            m_config.BuildTree(sbLog);

            if (File.Exists(strConfigFilePath) == false)
            {
                string strMsg = CResource.GetString(CResource.IDS_MSG_CONFIG_FILE_NOT_FOUND_CONTINUE,
                    "Config file is not found.\r\nDo you want to proceed with defaults?");
                string strCaption = CResource.GetString(CResource.IDS_ERROR, "Error");

                if (await MessageBox.ShowAsync(strMsg, strCaption, MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    return false;
                }
                m_strCofigJsonLoaded = CJson.GetJsonStr(m_config);
                await SaveConfigJsonAsync();
            }
            else
            {
                string strJson = CGenLib.ReadFileAllText(strConfigFilePath);

                CConfig configFile = CJson.ParseJsonStr<CConfig>(strJson);

                if (configFile == null)
                {
                    await MessageBox.ShowAsync("Json File Load Failure");
                    return false;
                }

                StringBuilder sbLog2 = new StringBuilder();
                configFile.RebuildNodeDictionary(sbLog2);
                configFile.AdjustForConsistency();
                m_config.UpdateValueAndComment(configFile);
                m_strCofigJsonLoaded = CJson.GetJsonStr(m_config);
            }
            return true;
        }


        static async Task<bool> SaveConfigJsonAsync(bool bNeedCompareToPopup = false)
        {
            string strConfigJson = CJson.GetJsonStr(m_config);

            if (bNeedCompareToPopup)
            {
                if (strConfigJson == m_strCofigJsonLoaded)
                {
                    return false;
                }
                else
                {
                    if (string.IsNullOrEmpty(m_strCofigJsonLoaded) != true)
                    {
                        string strMsg = CResource.GetString(CResource.IDS_MSG_SAVE_MODIFICATION_CONFIRM,
                            "A modification has taken place.\r\nDo you want to save the changes?");
                        string strCaption = CResource.GetString(CResource.IDS_WARNING, "Warning");

                        if (await MessageBox.ShowAsync(strMsg, strCaption, MessageBoxButtons.YesNo) != DialogResult.Yes)
                        {
                            return false;
                        }
                    }
                }
            }

            if (m_config != null)
            {
                if (m_strConfigFilePath == m_strConfigFilePath)
                {
                    // 저장하기 전 파일 백업

                    if (Directory.Exists(m_strLogDir) == false)
                        Directory.CreateDirectory(m_strLogDir);

                    if (strConfigJson != m_strCofigJsonLoaded)
                    {
                        string DatetimeNow = DateTime.Now.ToString("yyyyMMddhhmmss");
                        string strConfigFilePathBack = Path.Combine(m_strLogDir, "config.json." + DatetimeNow + ".e.txt");

                        if (CGenLib.WriteFileAllText(strConfigFilePathBack, m_strCofigJsonLoaded, Encoding.UTF8, true) == false)
                        {
                            await MessageBox.ShowAsync("Backup Fail", "Error");
                        }
                    }
                }

                // 파일변경시간 및 체크섬을 설정
                m_config.SetFileChanged();
                strConfigJson = CJson.GetJsonStr(m_config);

                if (CGenLib.WriteFileAllText(m_strConfigFilePath, strConfigJson, Encoding.UTF8, bFlushToDisk: true) == false)
                {
                    await MessageBox.ShowAsync("Config.json Write Fail", "Error");
                    return false;
                }
                m_strCofigJsonLoaded = strConfigJson;

                return true;
            }
            return false;
        }

        public static string GetAppInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Ver {0}", VERSION);
            return sb.ToString();
        }

        public static void ConfigNLog(string strLogDir, NLog.LogLevel logLevel)
        {
            var config = new NLog.Config.LoggingConfiguration();

            var targetFileApp = new NLog.Targets.FileTarget("logfileApp")
            {
                FileName = Path.Combine(strLogDir, "Ap2Tool.${date:format=yyyyMMdd}.log"),
                Encoding = Encoding.UTF8,
            };
            var targetFileLogger = new NLog.Targets.FileTarget("logfileLogger")
            {
                FileName = Path.Combine(strLogDir, "${logger}.${date:format=yyyyMMdd}.log"),
                Encoding = Encoding.UTF8,
            };

            var targetConsole = new NLog.Targets.ConsoleTarget("logConsole");
            var targetNull = new NLog.Targets.NullTarget();

            config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Info, targetNull, loggerNamePattern: "Microsoft.*", final: true);
            config.AddRule(logLevel, NLog.LogLevel.Fatal, targetFileLogger, loggerNamePattern: "cst", final: true);
            config.AddRule(logLevel, NLog.LogLevel.Fatal, targetFileLogger, loggerNamePattern: "mcu", final: true);
            config.AddRule(logLevel, NLog.LogLevel.Fatal, targetFileLogger, loggerNamePattern: "prt", final: true);
            config.AddRule(logLevel, NLog.LogLevel.Fatal, targetFileLogger, loggerNamePattern: "dis", final: true);
            config.AddRule(logLevel, NLog.LogLevel.Fatal, targetFileLogger, loggerNamePattern: "tur", final: true);
            config.AddRule(logLevel, NLog.LogLevel.Fatal, targetFileLogger, loggerNamePattern: "Ap", final: true);
            config.AddRule(logLevel, NLog.LogLevel.Fatal, targetFileLogger, loggerNamePattern: "alarm", final: true);
            config.AddRule(logLevel, NLog.LogLevel.Fatal, targetFileLogger, loggerNamePattern: "AtmsS", final: true);
            config.AddRule(logLevel, NLog.LogLevel.Fatal, targetFileLogger, loggerNamePattern: "AtmsR", final: true);
            config.AddRule(logLevel, NLog.LogLevel.Fatal, targetFileLogger, loggerNamePattern: "AtmsApiController", final: true);
            config.AddRule(logLevel, NLog.LogLevel.Fatal, targetFileLogger, loggerNamePattern: "UiApiController", final: true);
            config.AddRule(logLevel, NLog.LogLevel.Fatal, targetFileLogger, loggerNamePattern: "visAp", final: true);
            config.AddRule(logLevel, NLog.LogLevel.Fatal, targetFileLogger, loggerNamePattern: "visIns", final: true);
            config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, targetConsole, loggerNamePattern: "*");
            config.AddRule(logLevel, NLog.LogLevel.Fatal, targetFileApp, loggerNamePattern: "*");

            NLog.LogManager.Configuration = config;
        }

        public static bool LoadResourceFile(string strFileName, CConfig config)
        {
            string strResourceFile = Path.Combine(CSolutionGlobal.WorkDir, strFileName);
            if (!CResource.LoadResourceFile(strResourceFile))
                return false;

            string strConfigResourceFilePath = Path.Combine(CSolutionGlobal.WorkDir, $"config.resource.{CResource.Lang}.txt");
            if (CConfigLib.LoadConfigResourceFile(strConfigResourceFilePath,
                out Dictionary<string, string> dictResource,
                out string strErrMsg))
                config.UpdateResource(dictResource);
            else
                config.ClearResource();

            return true;
        }

    }
}
