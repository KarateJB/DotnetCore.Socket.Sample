using System.Text;
using System.Threading.Tasks;

namespace DotNetCore.SslSocket.Client.Services
{
    /// <summary>
    /// Message sender
    /// </summary>
    public class MsgSender : SslSocketClient
    {
        private const string CLIENT_MSG = "Hello, there.";

        /// <summary>
        /// Send file
        /// </summary>
        public async Task SendMsgAsync()
        {
            // Encode the data string into a byte array
            byte[] byteData = Encoding.ASCII.GetBytes($"{CLIENT_MSG}<EOF>");

            // Send
            await base.SendAsync(byteData);
        }
    }
}
