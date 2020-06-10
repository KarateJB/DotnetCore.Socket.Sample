using System;
using System.Threading.Tasks;
using Lib.Socket.Server.Models;

namespace Lib.Socket.Server.Utils.Handler
{
    /// <summary>
    /// Request handler interface
    /// </summary>
    public interface IRequestHandler : IDisposable
    {
        /// <summary>
        /// Handle request
        /// </summary>
        /// <param name="state">State object</param>
        Task HandleAsync(IStateObject state);
    }
}
