using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;

namespace AQ_FTP
{
    internal static class Constants
    {
        public static int LoopTime = 1800000;
        public static bool Loop = false;
        public static bool Testing = false;
        public static bool ConsoleLog = true;
        public static string LogPath;
        public static bool LogEvents = true;
        public static bool EmailUpdates;
        public static bool EmailErrors;
        public static string EmailAddresses;
        public static bool UseSFTP;
        public static string FtpServer;
        public static string FtpServerUserName;
        public static string FtpServerPassword;
        public static string FtpUploadPath;
        public static string SFTPServer;
        public static string SFTPServerUserName;
        public static string SFTPServerPassword;
        public static string SFTPUploadPath;
        public static string SshHostKeyFingerprint;
        public static readonly int SFTPport;

        public static string TESTEDIIN;
        public static string TESTEDIOUT;
        public static string EDIIN;
        public static string EDIOUT;
        public static string Inbound;
        public static string Outbound;

        public static string SMTP;
        public static string EmailFrom;
        public static int EmailPort;
        public static string EmailCredentialsUser;
        public static string EmailCredentialsPass;

        // We hold a reference to AppSettings so that we can inspect Count, etc.
        private static readonly KeyValueConfigurationCollection _settings;

        static Constants()
        {
            // 1) Figure out which config file path we will try to load.
            //    Either Public.ConfigPath (if non-null), otherwise default to
            //    "[current directory]\AQ_FTP.exe.config"
            string configFilePath = (Public.ConfigPath == null)
                                    ? Path.Combine(Directory.GetCurrentDirectory(), "AQ_FTP.exe.config")
                                    : Public.ConfigPath;

            // 2) If the file does not exist on disk, immediately throw an exception.
            if (!File.Exists(configFilePath))
            {
                throw new ConfigurationErrorsException(
                    $"Configuration file not found: '{configFilePath}'. " +
                    "Please ensure the .config file is present and readable before running.");
            }

            // 3) Map and open the configuration
            var map = new ExeConfigurationFileMap { ExeConfigFilename = configFilePath };
            Configuration cfg = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);

            // 4) If <appSettings> is present but contains zero keys, treat it as "empty" and error out.
            KeyValueConfigurationCollection keys = cfg.AppSettings.Settings;
            if (keys == null || keys.Count == 0)
            {
                throw new ConfigurationErrorsException(
                    $"Configuration file '{configFilePath}' contains no <appSettings> entries. " +
                    "Please populate at least one setting in <appSettings>.");
            }

            // 5) At this point we know config file exists AND there is at least one setting.
            _settings = keys;

            // 6) Now read each setting (same as before)
            Testing = Libs.Helpers.GetToBool(_settings["Testing"]?.Value);
            Loop = Libs.Helpers.GetToBool(_settings["Loop"]?.Value);
            LoopTime = Libs.Helpers.GetToInt32(_settings["LoopTime"]?.Value);
            EmailUpdates = Libs.Helpers.GetToBool(_settings["EmailUpdates"]?.Value);
            EmailErrors = Libs.Helpers.GetToBool(_settings["EmailErrors"]?.Value);
            EmailAddresses = _settings["EmailAddresses"]?.Value;

            UseSFTP = Libs.Helpers.GetToBool(_settings["UseSFTP"]?.Value);

            FtpServer = _settings["FtpServer"]?.Value;
            FtpServerUserName = _settings["FtpServerUserName"]?.Value;
            FtpServerPassword = _settings["FtpServerPassword"]?.Value;
            FtpUploadPath = _settings["FtpUploadPath"]?.Value;
            SFTPServer = _settings["SFTPServer"]?.Value;
            SFTPServerUserName = _settings["SFTPServerUserName"]?.Value;
            SFTPServerPassword = _settings["SFTPServerPassword"]?.Value;
            SshHostKeyFingerprint = _settings["SshHostKeyFingerprint"]?.Value;
            SFTPport = Libs.Helpers.GetToInt32(_settings["SFTPport"]?.Value);

            TESTEDIIN = _settings["TESTEDIIN"]?.Value;
            TESTEDIOUT = _settings["TESTEDIOUT"]?.Value;
            EDIIN = _settings["EDIIN"]?.Value;
            EDIOUT = _settings["EDIOUT"]?.Value;
            Inbound = _settings["Inbound"]?.Value;
            Outbound = _settings["Outbound"]?.Value;

            LogEvents = Libs.Helpers.GetToBool(_settings["LogEvents"]?.Value);
            ConsoleLog = Libs.Helpers.GetToBool(_settings["ConsoleLog"]?.Value);
            LogPath = _settings["LogPath"]?.Value;

            EmailCredentialsPass = _settings["EmailCredentialsPass"]?.Value;
            EmailCredentialsUser = _settings["EmailCredentialsUser"]?.Value;
            EmailFrom = _settings["EmailFrom"]?.Value;
            SMTP = _settings["SMTP"]?.Value;
            EmailPort = Libs.Helpers.GetToInt32(_settings["EmailPort"]?.Value);

            // 7) If LogPath was empty or missing, default it to current directory
            if (string.IsNullOrEmpty(LogPath))
            {
                LogPath = Directory.GetCurrentDirectory();
            }
        }
    }
}
