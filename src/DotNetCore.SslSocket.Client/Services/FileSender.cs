using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Lib.Socket.Client.Utils;

namespace DotNetCore.SslSocket.Client.Services
{
    /// <summary>
    /// File sender
    /// </summary>
    public class FileSender : SslSocketClient
    {
        private const string TestFileName = "Demo.txt";

        /// <summary>
        /// Send file
        /// </summary>
        public async Task SendFileAsync()
        {
            // Get file bytes
            var rootPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var filePath = Path.Combine(rootPath, "Files", TestFileName);
            var byteData = UploadFileConverter.ToFormattedBytes(filePath);

            // Send
            await base.SendAsync(byteData);
        }
    }
}
