using System.Text;

namespace Lib.Socket.Server.Models
{
    /// <summary>
    /// Interface of State object
    /// </summary>
    public interface IStateObject
    {
        /// <summary>
        /// Size of receive buffer
        /// </summary>
        int BufferSize { get; }

        /// <summary>
        /// Receive buffer
        /// </summary>
        byte[] Buffer { get; set; }

        /// <summary>
        /// Received data string
        /// </summary>
        StringBuilder Content { get; set; }
    }
}
