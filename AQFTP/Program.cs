using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AQFTP;
namespace AQ_FTP
{
    internal class Program
    {
        static void Main(string[] args)
        {
           try {

                if (args.Length > 0)
                {
                    Public.ConfigPath = args[0];
                }

                Libs.Helpers.SetEmailErrors(Constants.EmailErrors);
                Libs.Helpers.SetConsoleLog(Constants.ConsoleLog);
                Libs.Helpers.SetLog(Constants.LogEvents);
                Libs.Helpers.SetLogPath(Constants.LogPath);
                Libs.Helpers.SetEmailParams(new AQHelpers.EmailParams
                {
                    SMTP = Constants.SMTP,
                    EmailFrom = Constants.EmailFrom,
                    EmailCredentialsUser = Constants.EmailCredentialsUser,
                    EmailCredentialsPass = Constants.EmailCredentialsPass,
                    EmailPort = Constants.EmailPort,
                });
                AQFTP.Libs.Helpers.SetEmailErrors(Constants.EmailErrors);
                AQFTP.Libs.Helpers.SetConsoleLog(Constants.ConsoleLog);
                AQFTP.Libs.Helpers.SetLog(Constants.LogEvents);
                AQFTP.Libs.Helpers.SetLogPath(Constants.LogPath);
                AQFTP.Libs.Helpers.SetEmailParams(new AQHelpers.EmailParams
                {
                    SMTP = Constants.SMTP,
                    EmailFrom = Constants.EmailFrom,
                    EmailCredentialsUser = Constants.EmailCredentialsUser,
                    EmailCredentialsPass = Constants.EmailCredentialsPass,
                    EmailPort = Constants.EmailPort,
                });

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
               

                AQFTP.Get get = new Get(@params);
                AQFTP.Set set = new Set(@params);


                string EDIIN = Constants.Testing ? Constants.TESTEDIIN : Constants.EDIIN;
                string EDIOUT = Constants.Testing ? Constants.TESTEDIOUT : Constants.EDIOUT;

                // Check remote directory(s) exists
                if (!get.DirectoryExists(Constants.Inbound, EDIIN))
                {
                    Console.WriteLine($"Remote folder '{Constants.Inbound} {EDIIN}' not found.");
                    Libs.Helpers.LogError($"Remote folder '{Constants.Inbound} {EDIIN}' not found.");
                    return;
                }
                if (!get.DirectoryExists(Constants.Outbound, EDIOUT))
                {
                    Console.WriteLine($"Remote folder '{Constants.Outbound} {EDIOUT}' not found.");
                    Libs.Helpers.LogError($"Remote folder '{Constants.Outbound} {EDIOUT}' not found.");
                    return;
                }
                // make sure the directory exists

                ///
                ///
                /// DOWNLOAD -->
                /// 
                ///
                List<string> results = get.ListRemoteFiles(Constants.Inbound);
                if (results.Count > 0)
                {
                    // if files do exist then log what is intended to download
                    foreach (string result in results)
                    {
                        Libs.Helpers.WriteLog(result);
                    }
                    // attempt to download files
                    if (get.DownloadAllFiles(Constants.Inbound, EDIIN))
                    {
                        // if successful then delete the remote files
                        set.RemoveAllFiles(Constants.Inbound);
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

                ///
                ///
                /// UPLOAD -->
                /// 
                ///
                ///
                int upfailed = 0;
                string[] files = AQFTP.Get.FilesInDirectory(EDIOUT);
                if (files.Length > 0)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        if(!set.UploadFile(Constants.Outbound, files[i]))
                        {
                            upfailed++;
                        }
                        else
                        {
                            set.DeleteFile(files[i]);
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
            }
            catch (Exception e)
            {
                Console.WriteLine("Error during transfer: " + e.Message);
                Libs.Helpers.LogError(e.ToString());

            }

        }
    }
}
