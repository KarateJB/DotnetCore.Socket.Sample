using System.Text;
using Lib.Socket.Server.Models;

namespace DotNetCore.Socket.Server.Models
{
    /// <summary>
    /// Socket state object
    /// </summary>
    public class SocketState : IStateObject
    {
        /// <summary>
        /// Client Socket
        /// </summary>
        public System.Net.Sockets.Socket WorkSocket = null;

        private const int FixedBufferSize = 1024;

        /// <summary>
        /// Constructor
        /// </summary>
        public SocketState()
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
