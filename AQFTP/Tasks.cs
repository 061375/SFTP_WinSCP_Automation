using AQFTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AQ_FTP
{
    internal class Tasks
    {
        public Tasks()
        {
            // Instantiate your concrete Params class:
            var @params = new FtpParams
            {
                sftp = Constants.UseSFTP,

                FtpServer = Constants.FtpServer,
                FtpServerUserName = Constants.FtpServerUserName,
                FtpServerPassword = Constants.FtpServerPassword,

                SFTPServer = Constants.SFTPServer,
                SFTPServerUserName = Constants.SFTPServerUserName,
                SFTPServerPassword = Constants.SFTPServerPassword,
                SFTPport = Constants.SFTPport,
                SshHostKeyFingerprint = Constants.SshHostKeyFingerprint,
            };


            Public.Get = new Get(@params);
            Public.Set = new Set(@params);
        }
        public bool Check()
        {
            Public.EDIIN = Constants.Testing ? Constants.TESTEDIIN : Constants.EDIIN;
            Public.EDIOUT = Constants.Testing ? Constants.TESTEDIOUT : Constants.EDIOUT;
            Console.WriteLine();
            // Check remote directory(s) exists
            if (!Libs.Helpers.GetDirectoryExists(Public.EDIIN))
            {
                Console.WriteLine($"Remote folder '{Public.EDIIN}' not found.");
                Libs.Helpers.LogError($"Remote folder '{Public.EDIIN}' not found.");
                return false;
            }
            if (!Libs.Helpers.GetDirectoryExists(Public.EDIOUT))
            {
                Console.WriteLine($"Remote folder '{Public.EDIOUT}' not found.");
                Libs.Helpers.LogError($"Remote folder '{Public.EDIOUT}' not found.");
                return false;
            }
            return true;
        }
        public bool Download()
        {
            try
            {
                Libs.Helpers.LogMethod();
                ///
                ///
                /// DOWNLOAD -->
                /// 
                ///
                List<string> results = Public.Get.ListRemoteFiles(Constants.Inbound);
                if (results.Count > 0)
                {
                    // if files do exist then log what is intended to download
                    foreach (string result in results)
                    {
                        Libs.Helpers.WriteLog(result);
                    }
                    // attempt to download files
                    if (Public.Get.DownloadAllFiles(Constants.Inbound, Public.EDIIN))
                    {
                        // if successful then delete the remote files
                        Public.Set.RemoveAllFiles(Constants.Inbound);
                    }
                    Libs.Helpers.SendEmail(Constants.EmailAddresses, $"AQFTP Downloaded {results.Count} Files", "");
                }
                else
                {
                    Libs.Helpers.WriteLog("No files found in remote folder ...nothing to do");
                    Libs.Helpers.SendEmail(Constants.EmailAddresses, "No files found in remote folder ...nothing to do", "");
                }

                // for debugging
                Libs.Helpers.SendEmail(Constants.EmailAddresses, $"AQFTP Downloaded {results.Count} Files", "");
                ///
                ///
                /// <-- DOWNLOAD
                /// 
                ///
                return true;
            }
            catch (Exception e)
            {
                Libs.Helpers.LogError(e.ToString());
                return false;
            }
        }
        public bool Upload()
        {
            try
            {
                Libs.Helpers.LogMethod();
                ///
                ///
                /// UPLOAD -->
                /// 
                ///
                ///
                int upfailed = 0;
                string[] files = AQFTP.Get.FilesInDirectory(Public.EDIOUT);
                if (files.Length > 0)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        if (!Public.Set.UploadFile(Constants.Outbound, files[i]))
                        {
                            upfailed++;
                        }
                        else
                        {
                            Public.Set.DeleteFile(files[i]);
                        }
                    }
                    int success = files.Length - upfailed;
                    Libs.Helpers.SendEmail(Constants.EmailAddresses, $"AQFTP Uploaded {success} Files", "");
                }
                else
                {
                    Libs.Helpers.WriteLog("No files found in local folder ...nothing to do");
                    Libs.Helpers.SendEmail(Constants.EmailAddresses, "No files found in local folder ... nothing to do", "");
                }
                ///
                /// <-- UPLOAD
                /// 
                ///
                return true;
            }
            catch (Exception e)
            {
                Libs.Helpers.LogError(e.ToString());
                return false;
            }
        }
    }
}
