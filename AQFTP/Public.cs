using AQFTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AQ_FTP
{
    public interface Params
    {
        string SFTPServer { get; set; }
        string SFTPServerUserName { get; set; }
        string SFTPServerPassword { get; set; }
        string FtpServer { get; set; }
        string FtpServerUserName { get; set; }
        string FtpServerPassword { get; set; }
        int SFTPport { get; set; }
        bool sftp { get; set; }
    }
    internal class Public
    {
        public static string ConfigPath { get; set; }
        public static AQFTP.Get Get { get; set; }
        public static AQFTP.Set Set { get; set; }
        public static string EDIIN { get; set; }
        public static string EDIOUT { get; set;}
    }
}
