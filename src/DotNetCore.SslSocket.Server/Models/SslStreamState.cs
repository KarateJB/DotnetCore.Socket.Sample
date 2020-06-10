using System.Net.Security;
using System.Text;
using Lib.Socket.Server.Models;

namespace DotNetCore.SslSocket.Server.Models
{
    /// <summary>
    /// Socket state object
    /// </summary>
    public class SslStreamState : IStateObject
    {
        /// <summary>
        /// Client Socket
        /// </summary>
        public SslStream SslStream = null;

        private const int FixedBufferSize = 1024;

        /// <summary>
        /// Constructor
        /// </summary>
        public SslStreamState()
        {
            this.Buffer = new byte[this.BufferSize];
            this.Content = new StringBuilder();
        }

        /// <summary>
        /// Size of receive buffer
        /// </summary>
        public int BufferSize
        {
            get { return FixedBufferSize; }
        }

        /// <summary>
        /// Receive buffer
        /// </summary>
        public byte[] Buffer { get; set; }

        /// <summary>
        /// Received data string
        /// </summary>
        public StringBuilder Content { get; set; }
    }
}
