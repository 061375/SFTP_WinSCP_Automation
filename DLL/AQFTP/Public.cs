using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AQFTP
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
        string SshHostKeyFingerprint { get; set; }
    }
    public class FtpParams : Params
    {
        public string SFTPServer { get; set; }
        public string SFTPServerUserName { get; set; }
        public string SFTPServerPassword { get; set; }
        public string FtpServer { get; set; }
        public string FtpServerUserName { get; set; }
        public string FtpServerPassword { get; set; }
        public int SFTPport { get; set; }
        public bool sftp { get; set; }
        public string SshHostKeyFingerprint { get; set; }
    }
    internal class Public
    {
        public static string SFTPServer { get; set; }
        public static string SFTPServerUserName { get; set; }
        public static string SFTPServerPassword { get; set; }
        public static string FtpServer { get; set; }
        public static string FtpServerUserName { get; set; }
        public static string FtpServerPassword { get; set; }
        public static int SFTPport { get; set; }
        public static string SshHostKeyFingerprint { get; set; }
    }
}
