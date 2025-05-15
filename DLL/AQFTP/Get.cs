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
    public class Get : FTP
    {
        static string NormalizeRoot(string p)
            => Path.GetFullPath(p)
            .TrimEnd(Path.DirectorySeparatorChar)
                + Path.DirectorySeparatorChar;
        public Get(Params _params) : base(_params) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        public bool FileExists(string remotePath)
        {
            using (Session session = new Session())
            {
                // Will continuously report progress of transfer
                session.FileTransferProgress += SessionFileTransferProgress;
                // Connect
                session.Open(sessionOptions);

                try
                {
                    return session.FileExists(remotePath);
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
        /// Returns the full remote paths of all files under <paramref name="remotePath"/>.
        /// </summary>
        public List<string> ListRemoteFiles(string remotePath)
        {
            var result = new List<string>();
            using (var session = new Session())
            {
                session.Open(sessionOptions);
                // Enumerate all files (recursively)
                var files = session.EnumerateRemoteFiles(
                    remotePath,
                    "*",
                    EnumerationOptions.AllDirectories
                );

                foreach (var f in files)
                {
                    if (!f.IsDirectory)
                        result.Add(f.FullName);
                }
            }
            return result;
        }
        /// <summary>
        /// Downloads all files (recursively) from <paramref name="remotePath"/> into <paramref name="localPath"/>.
        /// </summary>
        public bool DownloadAllFiles(string remotePath, string localPath)
        {
            using (var session = new Session())
            {
                try
                {
                    localPath = NormalizeRoot(localPath);

                    session.FileTransferProgress += SessionFileTransferProgress;
                    session.Open(sessionOptions);

                    // Make sure the local folder exists
                    Directory.CreateDirectory(localPath);

                    var transferOptions = new TransferOptions
                    {
                        TransferMode = TransferMode.Binary
                    };

                    // Build a recursive mask: “/path/to/dir/**”
                    var mask = remotePath.TrimEnd('/', '\\') + "/**";

                    Console.WriteLine($"Downloading all with mask “{mask}” → {localPath}");
                    var result = session.GetFiles(mask, localPath, false, transferOptions);

                    // Throws if any single transfer failed
                    result.Check();

                    Console.WriteLine($"\nDownload complete: {result.Transfers.Count} files.");
                    return result.Transfers.Count > 0;
                }
                catch (Exception e)
                {
                    Libs.Helpers.LogError(e.ToString());
                    return false;
                }
            }
        }

        /// <summary>
        /// Downloads only the files whose full remote paths are in <paramref name="filesToDownload"/>.
        /// </summary>
        /// <param name="remoteBase">Base remote folder (will be prefixed to each file name if needed).</param>
        /// <param name="localPath">Local target folder.</param>
        /// <param name="filesToDownload">Collection of remote‐relative paths or full remote paths.</param>
        public bool DownloadFiles(string remoteBase, string localPath, IEnumerable<string> filesToDownload)
        {
            var list = filesToDownload as IList<string> ?? filesToDownload.ToList();
            if (!list.Any())
                return false;

            localPath = NormalizeRoot(localPath);

            using (var session = new Session())
            {
                session.FileTransferProgress += SessionFileTransferProgress;
                session.Open(sessionOptions);

                Directory.CreateDirectory(localPath);

                var transferOptions = new TransferOptions { TransferMode = TransferMode.Binary };
                int successCount = 0;

                foreach (var file in list)
                {
                    var remoteFilePath = file.StartsWith(NormalizeRoot(remoteBase), StringComparison.OrdinalIgnoreCase)
                        ? file
                        : RemotePath.Combine(remoteBase, file);

                    var fileName = Path.GetFileName(remoteFilePath);
                    var localFilePath = Path.Combine(localPath, fileName);

                    Console.WriteLine($"Downloading {remoteFilePath} → {localFilePath}");
                    var transfer = session.GetFiles(remoteFilePath, localFilePath, false, transferOptions);

                    if (transfer.Failures.Any())
                    {
                        foreach (SessionRemoteException ex in transfer.Failures)
                            Libs.Helpers.LogError($"Download failed: {ex.Message}");
                        continue;
                    }

                    successCount += transfer.Transfers.Count;
                }

                Console.WriteLine($"\nSelected downloads complete: {successCount} of {list.Count} files.");
                return successCount == list.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="remotePath"></param>
        /// <param name="localPath"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        public bool DirectoryExists(string remotePath, string localPath, bool create = false)
        {
            localPath = NormalizeRoot(localPath);
            using (Session session = new Session())
            {
                // Connect
                session.Open(sessionOptions);

                // Enumerate files and directories to upload
                IEnumerable<FileSystemInfo> fileInfos =
                    new DirectoryInfo(localPath).EnumerateFileSystemInfos(
                        "*", SearchOption.AllDirectories);

                foreach (FileSystemInfo fileInfo in fileInfos)
                {
                    string remoteFilePath =
                        RemotePath.TranslateLocalPathToRemote(
                            fileInfo.FullName, localPath, remotePath);
                    if (fileInfo.Attributes.HasFlag(FileAttributes.Directory))
                    {
                        // Create remote subdirectory, if it does not exist yet
                        if (!session.FileExists(remotePath))
                        {
                            if (create)
                            {
                                session.CreateDirectory(remotePath);
                                return true;
                            }
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        public List<string> RemoteFilePaths(string remotePath)
        {
            List<string> _return = new List<string>();
            using (Session session = new Session())
            {

                // Connect
                session.Open(sessionOptions);
                IEnumerable<RemoteFileInfo> _files = session.EnumerateRemoteFiles(remotePath, null, EnumerationOptions.AllDirectories);
                foreach (RemoteFileInfo _file in _files)
                {
                    _return.Add(_file.FullName);
                }
            }
            return _return;
        }
        public static string[] FilesInDirectory(string _path, string filter = "*", bool nameonly = false)
        {
            Libs.Helpers.LogMethod(_path);
            _path = NormalizeRoot(_path);

            string[] files = null;
            try
            {
                if (filter.IndexOf(",") > -1)
                {
                    string[] filters = filter.Split(',');
                    files = System.IO.Directory
                    .GetFiles(_path)
                    .Where(file => filters.Any(file.ToLower().EndsWith))
                    .ToArray();
                }
                else
                {
                    files = System.IO.Directory.GetFiles(_path, filter);
                }
                if (nameonly)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        files[i] = Path.GetFileName(_path);
                    }
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Libs.Helpers.LogError(e.ToString());
            }
            catch (System.IO.DirectoryNotFoundException e)
            {
                Libs.Helpers.LogError(e.ToString());
            }
            catch (System.Exception excpt)
            {
                Libs.Helpers.LogError(excpt.ToString());
            }
            if (files != null)
                Libs.Helpers.LogMethod(files.Count().ToString());

            return files;
        }
    }
}
