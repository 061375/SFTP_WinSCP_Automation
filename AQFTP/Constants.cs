using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Specialized;

namespace AQ_FTP
{
    internal static class Constants
    {
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


        private static readonly KeyValueConfigurationCollection _settings;
        static Constants()
        {
            var map = new ExeConfigurationFileMap
            {
                ExeConfigFilename = Public.ConfigPath == null 
                ?
                System.IO.Directory.GetCurrentDirectory() + "\\AQ_FTP.exe.config"
                :
                Public.ConfigPath
            };

            var cfg = ConfigurationManager
                        .OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            
            _settings = cfg.AppSettings.Settings;

            Testing = Libs.Helpers.GetToBool(_settings["Testing"]?.Value);

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
        }
    }
}
