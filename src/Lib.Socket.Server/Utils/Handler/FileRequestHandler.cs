using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Lib.Socket.Server.Models;

namespace Lib.Socket.Server.Utils.Handler
{
    /// <summary>
    /// Request Handler
    /// </summary>
    public class FileRequestHandler : IRequestHandler
    {
        private const string UploadFolder = "Upload";
        private const string Eof = "<EOF>";
        private const int FileNameReservedBytesLen = 4; // Reserved bytes length of file name
        private string uploadDir = string.Empty; // Server saved file path

        /// <summary>
        /// Constructor
        /// </summary>
        public FileRequestHandler()
        {
            var rootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            this.uploadDir = Path.Combine(rootPath, UploadFolder);

            // If the upload directory doesn't exist, create it
            System.IO.Directory.CreateDirectory(this.uploadDir);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Handle client request
        /// </summary>
        /// <param name="state">State object</param>
        public async Task HandleAsync(IStateObject state)
        {
            string data = state.Content.ToString();

            // Get UploadFileInfo object
            var uploadFileInfo = await this.GetUploadFileInfoAsync(data);

            // Save upload file
            await this.SaveFileAsync(uploadFileInfo);
        }

        /// <summary>
        /// Get upload file information
        /// </summary>
        /// <param name="data">The received data</param>
        /// <returns>UploadFileInfo object</returns>
        private async Task<UploadFileInfo> GetUploadFileInfoAsync(string data)
        {
            var ufInfo = new UploadFileInfo();

            var dataBytes = Encoding.ASCII.GetBytes(data);
            var dataBytesLen = dataBytes.Length;

            // File name's length
            var tmp = Encoding.ASCII.GetString(dataBytes, 0, FileNameReservedBytesLen);
            ufInfo.FileNameLen = Convert.ToInt16(Encoding.ASCII.GetString(dataBytes, 0, 4));

            // File name
            ufInfo.FileName = Encoding.ASCII.GetString(dataBytes, FileNameReservedBytesLen, ufInfo.FileNameLen);

            // File content
            var eofLen = Encoding.ASCII.GetBytes(Eof).Length;
            ufInfo.FileContent = Encoding.UTF8.GetString(
                dataBytes,
                FileNameReservedBytesLen + ufInfo.FileNameLen,
                dataBytesLen - FileNameReservedBytesLen - ufInfo.FileNameLen - eofLen);

            // File full path to be saved on server
            ufInfo.SavedFullPath = Path.Combine(this.uploadDir, ufInfo.FileName);

            return await Task.FromResult(ufInfo);
        }

        /// <summary>
        /// Save file
        /// </summary>
        /// <param name="ufInfo">Upload file information</param>
        private async Task SaveFileAsync(UploadFileInfo ufInfo)
        {
            #region Create an empty file if not exist

            _ = await this.CreateFileAsync(ufInfo.SavedFullPath);
            #endregion

            #region Write content to file

            var clientData = Encoding.ASCII.GetBytes(ufInfo.FileContent);

            int eofLen = Encoding.ASCII.GetBytes(Eof).Length;

            using BinaryWriter bWrite = new BinaryWriter(File.Open(ufInfo.SavedFullPath, FileMode.Create));
            bWrite.Write(clientData, 0, clientData.Length);
            bWrite.Close();
            #endregion

            await Task.CompletedTask;
        }

        /// <summary>
        /// Create file if not exist
        /// </summary>
        /// <param name="fileFullPath">Full file path</param>
        /// <returns>True(File created successfully)/False(File already exists)</returns>
        private async Task<bool> CreateFileAsync(string fileFullPath)
        {
            bool isAreadyExist = false;

            if (File.Exists(fileFullPath))
                isAreadyExist = true;
            else
                File.Create(fileFullPath).Close();

            return await Task.FromResult(!isAreadyExist);
        }
    }
}
