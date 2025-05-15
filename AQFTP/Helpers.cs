using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace AQ_FTP
{
    internal class Helpers
    {
        private bool ConsoleLog = false;
        private string LogPath = null;
        private bool Log = false;
        private bool ErFullStack = true;
        private bool EmailErrors = true;
        private string errorEmails { get; set; }
        private string ApplicationName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        public Helpers(string appname = null)
        {
            if(string.IsNullOrEmpty(appname))
                ApplicationName = appname;
        }
        public void SetLog(bool s) { Log = s; }
        public void SetLogPath(string s){LogPath = s;}
        public void SetConsoleLog(bool s) { ConsoleLog = s; }
        public void SetErFullStack(bool s) { ErFullStack = s;}
        public void SetEmailErrors(bool s) { EmailErrors = s;}
        public void SeterrorEmails(string s) { errorEmails = s; }

        public void WriteLog(string line, string path = null, bool datetime = true, string ext = ".log")
        {
            if (ConsoleLog)
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy h:mm tt") + " WriteLog");
            if (!Log)
                return;
            try
            {

                StringBuilder _line = new StringBuilder();
                if (ConsoleLog)
                    Console.WriteLine(line);
                if (null == path)
                {
                    if (LogPath == null)
                    {
                        path = System.IO.Directory.GetCurrentDirectory();
                    }
                    else
                    {
                        path = LogPath;
                    }
                }
                path += "/" + ApplicationName + (Constants.Testing ? "_TESTING" : "") + "_log" + ext;

                if (datetime)
                    _line.Append("\r\n" + DateTime.Now.ToString("MM/dd/yyyy h:mm:ss:ff tt") + " " + line);
                else
                    _line.Append("\r\n" + line);

                System.IO.File.AppendAllText(path, _line.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(line);
            }

        }
        public void LogError(string error, string path = null, bool fatal = false, bool ip = true, string warnemail = null)
        {
            if(ConsoleLog)
                Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy h:mm tt") + " LogError");
            try
            {
                if (!Log)
                    return;

                StringBuilder _out = new StringBuilder();

                if (string.IsNullOrEmpty(path))
                {
                    if (LogPath == null)
                    {
                        path = System.IO.Directory.GetCurrentDirectory();
                    }
                    else
                    {
                        path = LogPath;
                    }
                }
                // Get call stack
                System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();

                _out.Append("<pre>\r\n" + DateTime.Now.ToString("MM/dd/yyyy h:mm tt") + "\r\n");
                if (!ErFullStack)
                {
                    _out.Append("Method:: " + stackTrace.GetFrame(1).GetMethod().Name + "\r\n");
                }
                else
                {
                    System.Diagnostics.StackFrame[] t = stackTrace.GetFrames();
                    _out.Append("Method:: ");
                    for (int i = 0; i < t.Length; i++)
                    {
                        _out.Append(t[i].GetMethod().Name + " <-- ");
                    }
                    _out.Append("\r\n");
                }
                _out.Append("Error:: " + error + "\r\n");
                _out.Append("\r\n");
                _out.Append("</pre>");

                path += "/" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + (Constants.Testing ? " TESTING" : "") + "_ERROR.log";
                System.IO.File.AppendAllText(path, _out.ToString());


                if (EmailErrors && string.IsNullOrEmpty(errorEmails))
                        SendEmail(warnemail == null ? errorEmails : warnemail, "Error :: Application =  " + ApplicationName, _out.ToString().Replace(System.Environment.NewLine, "<br>"));


                WriteLog("!!! ERROR: An error occured: ( see error log ) !!!");
            }
            catch (Exception e)
            {
                SendEmail(errorEmails, "AQHelpers Error", e.ToString());
                Console.WriteLine("Helpers :: LogError :: " + e.Message);
                Console.WriteLine(error);
                if (fatal)
                {
                    WriteLog("The error was fatal and the program was shut-down");
                    System.Environment.Exit(1);
                }
            }
            if (fatal)
            {
                WriteLog("The error was fatal and the program was shut-down");
                System.Environment.Exit(1);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        public void SendEmail(string to, string subject, string message, List<string> attachments = null)
        {

            try
            {
                WriteLog("Attempting to send an email");
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                System.Net.Mail.SmtpClient SmtpServer = new System.Net.Mail.SmtpClient(Constants.SMTP);

                mail.From = new System.Net.Mail.MailAddress(Constants.EmailFrom);
                mail.To.Add(to);
                mail.Subject = subject + (Constants.Testing ? " TESTING" : "");
                mail.Body = message;
                mail.IsBodyHtml = true;

                if (null != attachments)
                {
                    foreach (string attachment in attachments)
                    {
                        // Create  the file attachment for this email message.
                        Attachment data = new Attachment(attachment, MediaTypeNames.Application.Octet);
                        // Add time stamp information for the file.
                        ContentDisposition disposition = data.ContentDisposition;
                        disposition.CreationDate = System.IO.File.GetCreationTime(attachment);
                        disposition.ModificationDate = System.IO.File.GetLastWriteTime(attachment);
                        disposition.ReadDate = System.IO.File.GetLastAccessTime(attachment);
                        // Add the file attachment to this email message.
                        mail.Attachments.Add(data);
                    }
                }
                SmtpServer.Port = Constants.EmailPort;
                SmtpServer.Credentials = new System.Net.NetworkCredential(Constants.EmailCredentialsUser, Constants.EmailCredentialsPass);
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);

                WriteLog("Email Sent Successfully");
            }
            catch (Exception ex)
            {
                WriteLog("Error Sending Email:");
                WriteLog(ex.ToString());
            }
        }
        
        public bool GetToBool(string text)
        {
            switch (text.ToLower())
            {
                case "y":
                case "yes":
                case "true":
                    return true;
                    break;
                case "no":
                case "n":
                case "false":
                    return false;
                    break;
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public int GetToInt32(string text, string _else = "0")
        {
            if (string.IsNullOrEmpty(text)) text = _else;
            return Convert.ToInt32(IsNumericThen(text, _else));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public int GetToInt32(char text, char _else = '0')
        {
            return Convert.ToInt32(IsNumericThen(text, _else));
        }
        /// <summary>
        /// Tries to determine if a string could be converted to an integer or a decimal, ...
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool IsNumeric(string text)
        {
            double _out;
            return double.TryParse(text, out _out);
        }
        public bool IsNumeric(char text)
        {
            double _out;
            return double.TryParse(text.ToString(), out _out);
        }
        /// <summary>
        /// if is not numeric then return else
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string IsNumericThen(string text, string _then = "0")
        {
            if (IsNumeric(text))
                return text;
            return _then;
        }
        /// <summary>
        /// if is not numeric then return else
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public char IsNumericThen(char text, char _then = '0')
        {
            if (IsNumeric(text))
                return text;
            return _then;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="place"></param>
        /// <returns></returns>
        public decimal GetToDecimal(string text, int place = -1)
        {
            decimal result = Convert.ToDecimal(IsNumericThen(text));
            if (place < 0)
            {
                return result;
            }
            return Math.Round(result, place);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public double GetToDouble(string text)
        {
            return Convert.ToDouble(IsNumericThen(text));
        }
    }
}
