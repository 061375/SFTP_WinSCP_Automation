using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
                Libs.Helpers.SeterrorEmails(Constants.EmailAddresses);
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
                AQFTP.Libs.Helpers.SeterrorEmails(Constants.EmailAddresses);
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

                if (Constants.Loop)
                {
                    StartLoop();
                }
                else
                {
                    Tasks tasks = new Tasks();
                    if (tasks.Check())
                    {
                        tasks.Upload();
                        tasks.Download();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error during transfer: " + e.Message);
                Libs.Helpers.LogError(e.ToString());

            }

        }
        private static Mutex mutex = null;
        static void StartLoop()
        {
            // make sur eonly one instance of service is running
            const string appName = "AQ_FTP";
            bool createdNew;
            mutex = new Mutex(true, appName, out createdNew);
            if (!createdNew)
            {
                return;
            }


            int interval = Constants.LoopTime; // default is 30 minutes

            TimerCallback callback = new TimerCallback(Run);
            Timer t = new Timer(callback, null, 0, interval);
            // loop here forever
            for (; ; )
            {
                // add a sleep for 100 mSec to reduce CPU usage
                Thread.Sleep(100);
                GC.KeepAlive(t);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        static public void Run(Object o)
        {       

            Tasks tasks = new Tasks();
            if (tasks.Check())
            {
                tasks.Upload();
                tasks.Download();
            }

            #region Hidden
            GC.Collect();
            #endregion
        }
    }
}
