using System;
using System.Threading.Tasks;
using Lib.Socket.Server.Models;

namespace Lib.Socket.Server.Utils.Handler
{
    /// <summary>
    /// Message request hander
    /// </summary>
    public class MsgRequestHandler : IRequestHandler
    {
        private const string Eof = "<EOF>";

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
            string msg = await this.GetMessageAsync(data);

            // Console out the received message
            Console.WriteLine($"\n{msg}");
        }

        /// <summary>
        /// Extract the message
        /// </summary>
        /// <param name="data">The received data</param>
        /// <returns>Message</returns>
        private async Task<string> GetMessageAsync(string data)
        {
            int eofStart = data.IndexOf(Eof);

            var msg = eofStart switch
            {
                -1 => data,
                0 => string.Empty,
                _ => data.Substring(0, eofStart)
            };

            return await Task.FromResult(msg);
        }
    }
}
