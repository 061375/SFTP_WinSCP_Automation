using AQHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinSCP;

namespace AQFTP
{
    public class Set : FTP
    {
        public Set(Params _params) : base(_params) { }
        public bool RemoveFile(string remotePath)
        {
            Libs.Helpers.LogMethod(remotePath);
            using (Session session = new Session())
            {
                // Will continuously report progress of transfer
                session.FileTransferProgress += SessionFileTransferProgress;
                // Connect
                session.Open(sessionOptions);

                try
                {
                    Libs.Helpers.WriteLog("Exists " + session.FileExists(remotePath));
                    if (session.FileExists(remotePath))
                    {
                        Libs.Helpers.WriteLog("deleting " + remotePath + " ...");
                        session.RemoveFile(remotePath);
                        return true;
                    }
                    return false;
                }
                catch (Exception e)
                {
                    Libs.Helpers.LogError(e.ToString());
                    return false;
                }
                finally
                {
                    // Terminate line after the last file (if any)
                    if (_lastFileName != null)
                    {
                        Console.WriteLine();
                    }
                }
            }
        }
        /// <summary>
        /// Removes every file (recursively) from the specified remote directory.
        /// </summary>
        /// <param name="remotePath">
        /// The remote folder whose contents you want to delete.
        /// e.g. "/incoming/data".  All files under that tree will be removed.
        /// </param>
        public void RemoveAllFiles(string remotePath)
        {
            using (var session = new Session())
            {
                session.Open(sessionOptions);

                // Build a recursive wildcard mask, e.g. "/incoming/data/**"
                string mask = remotePath.TrimEnd('/', '\\') + "/**";

                Console.WriteLine($"Removing all files matching '{mask}'...");

                // Remove all files; directories remain
                RemovalOperationResult result = session.RemoveFiles(mask);

                // Throws on any failure
                result.Check();

                // Use .Removals, not .Transfers
                Console.WriteLine($"\nRemoved {result.Removals.Count} files from '{remotePath}'.");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="remotePath"></param>
        /// <param name="date"></param>
        public void RemoveFilesByDate(string remotePath, DateTime date)
        {
            using (var session = new Session())
            {
                session.Open(sessionOptions);

                // 1) Enumerate all files (recursive) under remotePath
                var candidates = session
                    .EnumerateRemoteFiles(remotePath, null, EnumerationOptions.AllDirectories)
                    .Where(f => !f.IsDirectory && f.LastWriteTime.Date == date.Date)
                    .ToList();

                if (candidates.Count == 0)
                {
                    Console.WriteLine($"No files found on {date:yyyy-MM-dd} in '{remotePath}'.");
                    return;
                }

                Console.WriteLine($"Found {candidates.Count} file(s) on {date:yyyy-MM-dd}:");
                foreach (var f in candidates)
                    Console.WriteLine("  " + f.FullName);

                // 2) Remove each matching file
                int removed = 0;
                foreach (var f in candidates)
                {
                    Console.Write($"Removing {f.FullName}… ");
                    var result = session.RemoveFiles(f.FullName);
                    result.Check();                         // throws if failure
                    Console.WriteLine("OK");
                    removed += result.Removals.Count;       // normally 1
                }

                Console.WriteLine($"\nRemoved {removed} file(s) from '{remotePath}'.");
            }
        }
        public bool UploadFile(string remotePath, string localPath, bool delete = false)
        {
            using (Session session = new Session())
            {
                // Will continuously report progress of transfer
                session.FileTransferProgress += SessionFileTransferProgress;
                // Upload files
                TransferOptions transferOptions = new TransferOptions();
                transferOptions.TransferMode = TransferMode.Binary;
                // Connect
                session.Open(sessionOptions);

                try
                {
                    session.PutFileToDirectory(localPath, remotePath, delete, transferOptions);
                    return true;
                }
                catch(Exception e)
                {
                    Libs.Helpers.LogError(e.ToString());
                    return false;
                }
                finally
                {
                    // Terminate line after the last file (if any)
                    if (_lastFileName != null)
                    {
                        Console.WriteLine();
                    }
                }
            }
        }
        public bool RemoveDirectory(string remotePath)
        {
            Libs.Helpers.LogMethod(remotePath);
            using (Session session = new Session())
            {
                // Will continuously report progress of transfer
                session.FileTransferProgress += SessionFileTransferProgress;
                // Connect
                session.Open(sessionOptions);

                try
                {
                    // check that the directory is empty
                    if (session.FileExists(remotePath))
                    {
                        RemoteDirectoryInfo files = session.ListDirectory(remotePath);
                        if (files.Files.Count() < 2)
                        {
                            Console.WriteLine("removing directory " + remotePath);
                            // yes then delete
                            session.RemoveFiles(remotePath);
                            return true;
                        }
                    }
                    return false;
                }
                catch (Exception e)
                {
                    Libs.Helpers.LogError(e.ToString());
                    return false;
                }
                finally
                {
                    // Terminate line after the last file (if any)
                    if (_lastFileName != null)
                    {
                        Console.WriteLine();
                    }
                }
            }
        }
        public bool DeleteFile(string _file)
        {
            Libs.Helpers.LogMethod();
            if (File.Exists(_file))
            {
                try
                {
                    File.Delete(_file);
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("The process failed: {0}", e.ToString());
                    Libs.Helpers.LogError(e.ToString(), null, false);
                    return false;
                }
            }
            return false;
        }
        public bool DeleteFilesInDirectory(string dir = null, double deletebefore = 0, string filter = "*", bool nameonly = false)
        {
            
            if (null == dir) dir = Directory.GetCurrentDirectory();
            bool errors = false;
            string[] files = Get.FilesInDirectory(dir, filter, nameonly);
            foreach (string _file in files)
            {
                if (File.Exists(_file))
                {
                    try
                    {
                        if (deletebefore < 0)
                        {
                            FileInfo fi = new FileInfo(_file);
                            if (fi.LastAccessTime < DateTime.Now.AddHours(deletebefore))
                                File.Delete(_file);
                        }
                        else
                        {
                            File.Delete(_file);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("The process failed: {0}", e.ToString());
                        Libs.Helpers.LogError(e.ToString(), null, false);
                        errors = true;
                    }
                }
            }
            return errors;
        }
    }
}
