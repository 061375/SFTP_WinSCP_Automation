using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinSCP;

namespace AQFTP
{
    public class FTP
    {
        public SessionOptions sessionOptions;
        public FTP(Params _params)
        {
            Public.SFTPServer = _params?.SFTPServer;
            Public.SFTPServerUserName = _params?.SFTPServerUserName;
            Public.SFTPServerPassword = _params?.SFTPServerPassword;
            
            Public.FtpServer = _params?.FtpServer;
            Public.FtpServerUserName = _params?.FtpServerUserName;
            Public.FtpServerPassword = _params?.FtpServerPassword;
            Public.SshHostKeyFingerprint = _params?.SshHostKeyFingerprint;
            if (_params.sftp)
            {
                Public.SFTPport = _params.SFTPport;
                // Setup session options
                sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = Public.SFTPServer,
                    UserName = Public.SFTPServerUserName,
                    Password = Public.SFTPServerPassword,
                    PortNumber = Public.SFTPport,
                    SshHostKeyFingerprint = _params.SshHostKeyFingerprint
                };
            }
            else
            {
                // Setup session options
                sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Ftp,
                    HostName = Public.FtpServer,
                    UserName = Public.FtpServerUserName,
                    Password = Public.FtpServerPassword
                };
            }

        }
        public static void SessionFileTransferProgress(
        object sender, FileTransferProgressEventArgs e)
        {
            // New line for every new file
            if ((_lastFileName != null) && (_lastFileName != e.FileName))
            {
                Console.WriteLine();
            }

            // Print transfer progress
            Console.Write("\r{0} ({1:P0})", e.FileName, e.FileProgress);

            // Remember a name of the last file reported
            _lastFileName = e.FileName;
        }

        public static string _lastFileName;
    }
}
