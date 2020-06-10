namespace Lib.Socket.Server.Models
{
    /// <summary>
    /// Upload file information
    /// </summary>
    public class UploadFileInfo
    {
        /// <summary>
        /// File name's length
        /// </summary>
        public int FileNameLen { get; set; }

        /// <summary>
        /// File name
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// File content
        /// </summary>
        public string FileContent { get; set; }

        /// <summary>
        /// Saved full path on server
        /// </summary>
        public string SavedFullPath { get; set; }
    }
}
