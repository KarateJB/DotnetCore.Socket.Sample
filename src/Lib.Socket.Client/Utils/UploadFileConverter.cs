using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Lib.Socket.Client.Utils
{
    /// <summary>
    /// Upload file converter
    /// </summary>
    public class UploadFileConverter
    {
        private static string Eof = "<EOF>";
        private static int FileNameReservedBytesLen = 4; // Reserved bytes length of file name

        /// <summary>
        /// To fomatted bytes
        /// </summary>
        /// <param name="filePath">File full path</param>
        /// <returns>Bytes</returns>
        public static byte[] ToFormattedBytes(string filePath)
        {
            byte[] clientData = null;

            // Get file name
            string fileName = Path.GetFileName(filePath);

            // Get file name as bytes
            byte[] fileNameBytes = Encoding.ASCII.GetBytes(fileName);

            // Get file name's lenght as bytes
            byte[] fileNameLenBytes = Encoding.ASCII.GetBytes(fileNameBytes.Length.ToString());

            // Get file data as bytes
            byte[] fileContentBytes = File.ReadAllBytes(filePath);

            // EOF
            byte[] eofBytes = Encoding.ASCII.GetBytes(Eof);

            // Define clientData capacity: [File name length][File name][File content][EOF]
            clientData = new byte[FileNameReservedBytesLen + fileNameBytes.Length + fileContentBytes.Length + eofBytes.Length];

            // Write clientData as
            // [File name length][File name][File content][EOF]
            fileNameLenBytes.CopyTo(clientData, 0);
            fileNameBytes.CopyTo(clientData, FileNameReservedBytesLen);
            fileContentBytes.CopyTo(clientData, FileNameReservedBytesLen + fileNameBytes.Length);
            eofBytes.CopyTo(clientData, FileNameReservedBytesLen + fileNameBytes.Length + fileContentBytes.Length);

            return clientData;
        }
    }
}
